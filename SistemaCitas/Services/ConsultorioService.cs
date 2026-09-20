using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaCitas.Models;
using SistemaCitas.Repositories;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Services;

public class ConsultorioService : IConsultorioService
{
    private const int FilasReceta = 3;

    private readonly ICitaRepositorio _citas;
    private readonly IAtencionRepositorio _atenciones;
    private readonly IRepositorio<Doctor> _doctores;
    private readonly IRepositorio<Paciente> _pacientes;
    private readonly IRepositorio<Cie10> _cie10;
    private readonly IRepositorio<Medicamento> _medicamentos;

    public ConsultorioService(
        ICitaRepositorio citas,
        IAtencionRepositorio atenciones,
        IRepositorio<Doctor> doctores,
        IRepositorio<Paciente> pacientes,
        IRepositorio<Cie10> cie10,
        IRepositorio<Medicamento> medicamentos)
    {
        _citas = citas;
        _atenciones = atenciones;
        _doctores = doctores;
        _pacientes = pacientes;
        _cie10 = cie10;
        _medicamentos = medicamentos;
    }

    public async Task<Doctor?> ObtenerDoctorPorUsuarioAsync(string usuarioId)
    {
        var lista = await _doctores.BuscarAsync(d => d.UsuarioId == usuarioId && d.Activo);
        return lista.FirstOrDefault();
    }

    // ---------------------------------------------------------------
    // RF-12: agenda
    // ---------------------------------------------------------------
    public async Task<AgendaViewModel> ArmarAgendaAsync(Doctor doctor, DateOnly fecha)
    {
        var citas = (await _citas.ListarPorDoctorAsync(doctor.IdDoctor, fecha)).ToList();

        return new AgendaViewModel
        {
            Doctor = doctor,
            Fecha = fecha,
            PorAtender = citas.Where(c => c.AtencionMedica == null).ToList(),
            Atendidas = citas.Where(c => c.AtencionMedica != null).ToList()
        };
    }

    // ---------------------------------------------------------------
    // RF-13: atencion medica
    // ---------------------------------------------------------------

    // Una cita se puede atender si es del doctor, esta Pendiente o ya llego (Asistido),
    // todavia no tiene atencion y su fecha es hoy o anterior.
    private static string? ValidarCita(Cita? cita, int idDoctor)
    {
        if (cita == null || cita.IdDoctor != idDoctor)
            return "La cita no existe o no pertenece a su agenda.";

        if (cita.Estado != EstadoCita.Pendiente && cita.Estado != EstadoCita.Asistido)
            return $"La cita está en estado {cita.Estado} y no se puede atender.";

        if (cita.AtencionMedica != null)
            return "Esta cita ya fue atendida.";

        if (cita.FechaCita > DateOnly.FromDateTime(DateTime.Now))
            return "Solo se pueden atender citas del día de hoy o de días anteriores.";

        return null;
    }

    public async Task<(AtencionViewModel? Modelo, string? Error)> PrepararAtencionAsync(int idDoctor, int idCita)
    {
        var cita = await _citas.ObtenerParaAtencionAsync(idCita);
        var error = ValidarCita(cita, idDoctor);
        if (error != null) return (null, error);

        var modelo = new AtencionViewModel { IdCita = idCita };
        await CompletarAsync(modelo);
        return (modelo, null);
    }

    public async Task CompletarAsync(AtencionViewModel modelo)
    {
        var cita = await _citas.ObtenerParaAtencionAsync(modelo.IdCita);
        modelo.Cita = cita;
        modelo.Paciente = cita?.Paciente;

        var diagnosticos = await _cie10.ObtenerTodosAsync();
        modelo.Diagnosticos = diagnosticos
            .OrderBy(c => c.CodigoCie10)
            .Select(c => new SelectListItem($"{c.CodigoCie10} - {c.Descripcion}", c.CodigoCie10))
            .ToList();

        var medicamentos = await _medicamentos.BuscarAsync(m => m.Activo);
        modelo.Medicamentos = medicamentos
            .OrderBy(m => m.Nombre)
            .Select(m => new SelectListItem(
                m.Presentacion == null ? m.Nombre : $"{m.Nombre} ({m.Presentacion})",
                m.IdMedicamento.ToString()))
            .ToList();

        while (modelo.Receta.Count < FilasReceta)
            modelo.Receta.Add(new RecetaItemViewModel());
    }

    public async Task<ResultadoOperacion> RegistrarAtencionAsync(int idDoctor, AtencionViewModel modelo)
    {
        var cita = await _citas.ObtenerParaAtencionAsync(modelo.IdCita);
        var error = ValidarCita(cita, idDoctor);
        if (error != null) return ResultadoOperacion.Falla(error);

        if (!await _cie10.ExisteAsync(c => c.CodigoCie10 == modelo.CodigoCie10))
            return ResultadoOperacion.Falla("El diagnóstico seleccionado no existe.");

        // Presion arterial (opcional): "sistolica/diastolica" con rangos razonables
        if (!string.IsNullOrWhiteSpace(modelo.PresionArterial))
        {
            var partes = modelo.PresionArterial.Split('/');
            if (partes.Length != 2 || !int.TryParse(partes[0], out var sistolica) || !int.TryParse(partes[1], out var diastolica))
                return ResultadoOperacion.Falla("La presión arterial debe tener el formato 120/80.");

            if (sistolica < 60 || sistolica > 260 || diastolica < 30 || diastolica > 160 || sistolica <= diastolica)
                return ResultadoOperacion.Falla("La presión arterial no es válida: sistólica de 60 a 260, diastólica de 30 a 160 y la sistólica mayor que la diastólica.");
        }

        // Receta: solo cuentan las filas que el doctor lleno; las filas a medias se rechazan
        var filas = modelo.Receta
            .Where(r => r.IdMedicamento != null || r.Cantidad != null || !string.IsNullOrWhiteSpace(r.Indicaciones))
            .ToList();

        foreach (var fila in filas)
        {
            if (fila.IdMedicamento == null || fila.Cantidad == null || string.IsNullOrWhiteSpace(fila.Indicaciones))
                return ResultadoOperacion.Falla("Cada medicamento de la receta necesita medicamento, cantidad e indicaciones.");

            var idMedicamento = fila.IdMedicamento.Value;
            if (!await _medicamentos.ExisteAsync(m => m.IdMedicamento == idMedicamento && m.Activo))
                return ResultadoOperacion.Falla("Uno de los medicamentos elegidos no existe.");
        }

        if (filas.Select(f => f.IdMedicamento).Distinct().Count() != filas.Count)
            return ResultadoOperacion.Falla("No repita el mismo medicamento en la receta.");

        var ahora = DateTime.Now;

        cita!.Triaje = new Triaje
        {
            PesoKg = modelo.PesoKg!.Value,
            TallaCm = modelo.TallaCm!.Value,
            TemperaturaC = modelo.TemperaturaC!.Value,
            PresionArterial = string.IsNullOrWhiteSpace(modelo.PresionArterial) ? null : modelo.PresionArterial.Trim(),
            FrecuenciaCardiaca = modelo.FrecuenciaCardiaca,
            SaturacionO2 = modelo.SaturacionO2,
            FechaRegistro = ahora
        };

        var atencion = new AtencionMedica
        {
            FechaHoraAtencion = ahora,
            MotivoConsulta = modelo.MotivoConsulta.Trim(),
            Sintomatologia = Limpiar(modelo.Sintomatologia),
            Observaciones = Limpiar(modelo.Observaciones)
        };

        atencion.Diagnosticos.Add(new AtencionDiagnostico
        {
            CodigoCie10 = modelo.CodigoCie10!,
            TipoDiagnostico = modelo.TipoDiagnostico
        });

        foreach (var fila in filas)
        {
            atencion.Receta.Add(new DetalleReceta
            {
                IdMedicamento = fila.IdMedicamento!.Value,
                Cantidad = fila.Cantidad!.Value,
                Indicaciones = fila.Indicaciones!.Trim()
            });
        }

        cita.AtencionMedica = atencion;

        // Si el paciente aun figuraba como Pendiente, atenderlo significa que asistio
        if (cita.Estado == EstadoCita.Pendiente)
        {
            cita.Estado = EstadoCita.Asistido;
            cita.HoraLlegada ??= ahora;
        }

        try
        {
            await _citas.GuardarCambiosAsync();
        }
        catch (DbUpdateException)
        {
            return ResultadoOperacion.Falla("No se pudo guardar la atención. Es posible que esta cita ya haya sido atendida.");
        }

        return ResultadoOperacion.Ok();
    }

    private static string? Limpiar(string? texto) =>
        string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();

    // ---------------------------------------------------------------
    // RF-14: historial clinico
    // ---------------------------------------------------------------
    public async Task<HistorialPacienteViewModel?> ObtenerHistorialAsync(int idDoctor, int idPaciente)
    {
        // Privacidad: el doctor solo ve el historial de pacientes que tienen o tuvieron una cita con el
        if (!await _citas.ExisteAsync(c => c.IdDoctor == idDoctor && c.IdPaciente == idPaciente))
            return null;

        var paciente = await _pacientes.ObtenerPorIdAsync(idPaciente);
        if (paciente == null) return null;

        return new HistorialPacienteViewModel
        {
            Paciente = paciente,
            Atenciones = await _atenciones.HistorialDePacienteAsync(idPaciente)
        };
    }
}
