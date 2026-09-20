using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaCitas.Models;
using SistemaCitas.Repositories;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Services;

public class CitaService : ICitaService
{
    private const string Habilitado = "Habilitado";

    private readonly ICitaRepositorio _citas;
    private readonly IDoctorRepositorio _doctores;
    private readonly IRepositorio<Paciente> _pacientes;
    private readonly IRepositorio<Especialidad> _especialidades;
    private readonly IRepositorio<HistorialPuntaje> _historial;
    private readonly IPrioridadService _prioridad;

    public CitaService(
        ICitaRepositorio citas,
        IDoctorRepositorio doctores,
        IRepositorio<Paciente> pacientes,
        IRepositorio<Especialidad> especialidades,
        IRepositorio<HistorialPuntaje> historial,
        IPrioridadService prioridad)
    {
        _citas = citas;
        _doctores = doctores;
        _pacientes = pacientes;
        _especialidades = especialidades;
        _historial = historial;
        _prioridad = prioridad;
    }

    public Task<Paciente?> ObtenerPacienteAsync(int idPaciente) => _pacientes.ObtenerPorIdAsync(idPaciente);

    public async Task<Paciente?> ObtenerPacientePorUsuarioAsync(string usuarioId)
    {
        var lista = await _pacientes.BuscarAsync(p => p.UsuarioId == usuarioId);
        return lista.FirstOrDefault();
    }

    public Task<IEnumerable<Cita>> ListarCitasPacienteAsync(int idPaciente) => _citas.ListarPorPacienteAsync(idPaciente);

    public Task<IEnumerable<Cita>> ListarCitasPorFechaAsync(DateOnly fecha) => _citas.ListarPorFechaAsync(fecha);

    // ---------------------------------------------------------------
    // Busqueda de cupos (RF-07)
    // ---------------------------------------------------------------
    public async Task<BuscarCuposViewModel> ArmarBusquedaAsync(
        Paciente paciente, int? idEspecialidad, int? idDoctor, DateOnly? fecha, bool presencial)
    {
        var vm = new BuscarCuposViewModel
        {
            Paciente = paciente,
            IdEspecialidad = idEspecialidad,
            IdDoctor = idDoctor,
            Fecha = fecha,
            Presencial = presencial
        };

        var importancia = paciente.PorcentajeImportancia;

        // La asignacion presencial la decide Admision (RF-09): no se aplican las restricciones por importancia.
        if (presencial)
        {
            vm.Bloqueado = false;
            vm.AvisoPrioridad = "Asignación presencial: Admisión asigna la cita directamente, verificando el DNI físico del paciente.";
        }
        else
        {
            vm.Bloqueado = _prioridad.EstaBloqueado(importancia);
            vm.AvisoPrioridad = _prioridad.Describir(importancia);
        }

        var especialidades = await _especialidades.BuscarAsync(e => e.Activo);
        vm.Especialidades = especialidades
            .OrderBy(e => e.Nombre)
            .Select(e => new SelectListItem(e.Nombre, e.IdEspecialidad.ToString(), e.IdEspecialidad == idEspecialidad))
            .ToList();

        if (idEspecialidad == null) return vm;

        var doctores = (await _doctores.ListarAsync(null))
            .Where(d => d.Activo
                        && d.EstadoHabilitacion == Habilitado
                        && d.DoctorEspecialidades.Any(de => de.IdEspecialidad == idEspecialidad))
            .ToList();

        vm.Doctores = doctores
            .Select(d => new SelectListItem(
                $"{d.ApellidoPaterno} {d.ApellidoMaterno}, {d.Nombres} - {d.Ipress.Nombre}",
                d.IdDoctor.ToString(),
                d.IdDoctor == idDoctor))
            .ToList();

        var elegido = doctores.FirstOrDefault(d => d.IdDoctor == idDoctor);
        if (elegido == null)
        {
            vm.IdDoctor = null;
            return vm;
        }

        vm.DoctorSeleccionado = elegido;

        if (fecha != null && !vm.Bloqueado)
            vm.Cupos = await BuscarCuposAsync(elegido.IdDoctor, fecha.Value, importancia, !presencial);

        return vm;
    }

    // Genera las horas de un bloque de horario. Cada hora tiene "CuposPorHora" cupos,
    // asi que un cupo dura 60 / CuposPorHora minutos (ej. 4 cupos por hora = cada 15 minutos).
    private static List<TimeOnly> GenerarHoras(HorarioDoctor h)
    {
        var paso = TimeSpan.FromMinutes(Math.Max(1, 60 / Math.Max(1, h.CuposPorHora)));
        var horas = new List<TimeOnly>();
        for (var t = h.HoraInicio; ; t = t.Add(paso))
        {
            var fin = t.Add(paso);
            if (fin <= t || fin > h.HoraFin) break;   // "fin <= t" evita dar la vuelta pasada la medianoche
            horas.Add(t);
        }
        return horas;
    }

    // 1 = lunes ... 7 = domingo (igual que HorarioDoctor.DiaSemana)
    private static int DiaSemana(DateOnly fecha) =>
        fecha.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)fecha.DayOfWeek;

    private async Task<List<CupoDto>> BuscarCuposAsync(int idDoctor, DateOnly fecha, decimal importancia, bool aplicarPrioridad)
    {
        var lista = new List<CupoDto>();

        var doctor = await _doctores.ObtenerConDetalleAsync(idDoctor);
        if (doctor == null || !doctor.Activo || doctor.EstadoHabilitacion != Habilitado) return lista;

        var ahora = DateTime.Now;
        var hoy = DateOnly.FromDateTime(ahora);
        var horaActual = TimeOnly.FromDateTime(ahora);
        if (fecha < hoy) return lista;

        var ocupadas = await _citas.HorasOcupadasAsync(idDoctor, fecha);
        var diasMinimos = aplicarPrioridad ? _prioridad.DiasMinimosAnticipacion(importancia) : 0;
        var anticipacion = fecha.DayNumber - hoy.DayNumber;

        var bloques = doctor.Horarios
            .Where(h => h.Activo && h.DiaSemana == DiaSemana(fecha))
            .OrderBy(h => h.HoraInicio);

        foreach (var bloque in bloques)
        {
            foreach (var hora in GenerarHoras(bloque))
            {
                if (fecha == hoy && hora <= horaActual) continue;   // horas que ya pasaron no se muestran

                string? motivo = null;
                if (ocupadas.Contains(hora))
                    motivo = "Ocupado";
                else if (anticipacion < diasMinimos)
                    motivo = $"Reservado para pacientes con importancia de {PrioridadService.UmbralPrioritario:0.##}% o más";

                lista.Add(new CupoDto(hora, motivo == null, motivo));
            }
        }

        return lista;
    }

    // ---------------------------------------------------------------
    // Agendar (RF-07, RF-08, RF-09)
    // ---------------------------------------------------------------
    public async Task<ResultadoOperacion> AgendarAsync(
        int idPaciente, int idDoctor, DateOnly fecha, TimeOnly hora, string canal, bool dniVerificado = false)
    {
        var presencial = canal == "Presencial";

        // RC-03: en ventanilla se debe validar el DNI fisico antes de asignar la cita
        if (presencial && !dniVerificado)
            return ResultadoOperacion.Falla("RC-03: debe confirmar que verificó el DNI físico del paciente.");

        var paciente = await _pacientes.ObtenerPorIdAsync(idPaciente);
        if (paciente == null || !paciente.Activo)
            return ResultadoOperacion.Falla("El paciente no existe o está inactivo.");

        // RF-08: evaluar el porcentaje de importancia antes de confirmar (solo en el canal web)
        if (!presencial)
        {
            if (_prioridad.EstaBloqueado(paciente.PorcentajeImportancia))
                return ResultadoOperacion.Falla(_prioridad.Describir(paciente.PorcentajeImportancia));
        }

        var doctor = await _doctores.ObtenerConDetalleAsync(idDoctor);
        if (doctor == null || !doctor.Activo)
            return ResultadoOperacion.Falla("El doctor no existe o está inactivo.");

        if (doctor.EstadoHabilitacion != Habilitado)
            return ResultadoOperacion.Falla("El doctor no está habilitado por el CMP.");

        var ahora = DateTime.Now;
        var hoy = DateOnly.FromDateTime(ahora);
        if (fecha < hoy || (fecha == hoy && hora <= TimeOnly.FromDateTime(ahora)))
            return ResultadoOperacion.Falla("La fecha y hora de la cita deben ser futuras.");

        if (!presencial)
        {
            var minimo = _prioridad.DiasMinimosAnticipacion(paciente.PorcentajeImportancia);
            if (fecha.DayNumber - hoy.DayNumber < minimo)
                return ResultadoOperacion.Falla(
                    $"Con su importancia actual ({paciente.PorcentajeImportancia:0.##}%) solo puede reservar cupos con al menos {minimo} días de anticipación.");
        }

        // RC-02: la cita debe caer dentro del horario asignado al doctor
        var enAgenda = doctor.Horarios.Any(h => h.Activo && h.DiaSemana == DiaSemana(fecha) && GenerarHoras(h).Contains(hora));
        if (!enAgenda)
            return ResultadoOperacion.Falla("Ese horario no está dentro de la agenda del doctor.");

        // El paciente no puede tener dos citas a la misma hora
        if (await _citas.ExisteAsync(c => c.IdPaciente == idPaciente && c.FechaCita == fecha
                                          && c.HoraCita == hora && c.Estado == EstadoCita.Pendiente))
            return ResultadoOperacion.Falla("El paciente ya tiene otra cita pendiente a esa misma hora.");

        try
        {
            var existente = await _citas.ObtenerPorCupoAsync(idDoctor, fecha, hora);

            if (existente != null && existente.Estado != EstadoCita.Cancelado)
                return ResultadoOperacion.Falla("Ese cupo ya fue tomado por otro paciente. Elija otro horario.");

            if (existente != null)
            {
                // El cupo estaba cancelado: se reutiliza (el indice unico no permite duplicarlo)
                existente.IdPaciente = idPaciente;
                existente.IdIpress = doctor.IdIpress;
                existente.Estado = EstadoCita.Pendiente;
                existente.PuntajeAlAgendar = paciente.PorcentajeImportancia;
                existente.Canal = canal;
                existente.HoraLlegada = null;
                existente.FechaRegistro = ahora;
            }
            else
            {
                await _citas.AgregarAsync(new Cita
                {
                    IdPaciente = idPaciente,
                    IdDoctor = idDoctor,
                    IdIpress = doctor.IdIpress,
                    FechaCita = fecha,
                    HoraCita = hora,
                    Estado = EstadoCita.Pendiente,
                    PuntajeAlAgendar = paciente.PorcentajeImportancia,
                    Canal = canal,
                    FechaRegistro = ahora
                });
            }

            await _citas.GuardarCambiosAsync();
        }
        catch (DbUpdateException)
        {
            // Dos personas reservaron el mismo cupo al mismo tiempo: el indice unico lo impide
            return ResultadoOperacion.Falla("Ese cupo acaba de ser tomado por otro paciente. Elija otro horario.");
        }

        return ResultadoOperacion.Ok();
    }

    // ---------------------------------------------------------------
    // Cancelar (paciente) y cambiar estado (Admision) - RF-10 y RF-11
    // ---------------------------------------------------------------
    public async Task<ResultadoOperacion> CancelarAsync(int idCita, int idPaciente)
    {
        var cita = await _citas.ObtenerPorIdAsync(idCita);
        if (cita == null || cita.IdPaciente != idPaciente)
            return ResultadoOperacion.Falla("La cita no existe.");

        if (cita.Estado != EstadoCita.Pendiente)
            return ResultadoOperacion.Falla("Solo se pueden cancelar las citas pendientes.");

        cita.Estado = EstadoCita.Cancelado;
        await _citas.GuardarCambiosAsync();
        return ResultadoOperacion.Ok();
    }

    public async Task<ResultadoOperacion> CambiarEstadoAsync(int idCita, EstadoCita nuevoEstado, string usuarioId)
    {
        var cita = await _citas.ObtenerConDetalleAsync(idCita);
        if (cita == null) return ResultadoOperacion.Falla("La cita no existe.");

        if (cita.Estado != EstadoCita.Pendiente)
            return ResultadoOperacion.Falla($"La cita ya está en estado {cita.Estado}; solo se pueden cambiar las pendientes.");

        if (nuevoEstado == EstadoCita.Pendiente)
            return ResultadoOperacion.Falla("Estado no válido.");

        var hoy = DateOnly.FromDateTime(DateTime.Now);
        if ((nuevoEstado == EstadoCita.Asistido || nuevoEstado == EstadoCita.Falto) && cita.FechaCita > hoy)
            return ResultadoOperacion.Falla("Solo se puede marcar Asistió o Faltó desde el día de la cita.");

        cita.Estado = nuevoEstado;

        if (nuevoEstado == EstadoCita.Asistido)
            cita.HoraLlegada = DateTime.Now;

        // RF-11: recalculo de prioridad. Cada inasistencia resta puntos y queda registrada (auditable).
        if (nuevoEstado == EstadoCita.Falto)
        {
            var paciente = cita.Paciente;
            var anterior = paciente.PorcentajeImportancia;
            paciente.PorcentajeImportancia = _prioridad.AplicarPenalizacion(anterior);

            await _historial.AgregarAsync(new HistorialPuntaje
            {
                IdPaciente = paciente.IdPaciente,
                IdCita = cita.IdCita,
                PuntajeAnterior = anterior,
                PuntajeNuevo = paciente.PorcentajeImportancia,
                Motivo = "Inasistencia",
                RegistradoPorId = usuarioId,
                Fecha = DateTime.Now
            });
        }

        await _citas.GuardarCambiosAsync();
        return ResultadoOperacion.Ok();
    }
}
