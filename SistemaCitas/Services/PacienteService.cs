using SistemaCitas.Models;
using SistemaCitas.Repositories;

namespace SistemaCitas.Services;

public class PacienteService : IPacienteService
{
    private readonly IRepositorio<Paciente> _pacientes;
    private readonly IRepositorio<PlanSeguro> _planes;
    private readonly IRepositorio<Distrito> _distritos;

    public PacienteService(
        IRepositorio<Paciente> pacientes,
        IRepositorio<PlanSeguro> planes,
        IRepositorio<Distrito> distritos)
    {
        _pacientes = pacientes;
        _planes = planes;
        _distritos = distritos;
    }

    public async Task<IEnumerable<Paciente>> BuscarAsync(string? texto)
    {
        IEnumerable<Paciente> lista;

        if (string.IsNullOrWhiteSpace(texto))
        {
            lista = await _pacientes.ObtenerTodosAsync();
        }
        else
        {
            var t = texto.Trim();
            lista = await _pacientes.BuscarAsync(p =>
                p.Dni.Contains(t) ||
                p.Nombres.Contains(t) ||
                p.ApellidoPaterno.Contains(t) ||
                p.ApellidoMaterno.Contains(t));
        }

        return lista
            .OrderBy(p => p.ApellidoPaterno)
            .ThenBy(p => p.ApellidoMaterno)
            .ThenBy(p => p.Nombres)
            .Take(100);
    }

    public async Task<ResultadoOperacion> RegistrarPresencialAsync(Paciente paciente, string registradoPorId)
    {
        // Regla 1: el DNI debe ser de 8 digitos
        if (paciente.Dni.Length != 8 || !paciente.Dni.All(char.IsDigit))
            return ResultadoOperacion.Falla("El DNI debe tener 8 dígitos");

        // Regla 2: no puede haber dos pacientes con el mismo DNI
        if (await _pacientes.ExisteAsync(p => p.Dni == paciente.Dni))
            return ResultadoOperacion.Falla("Ya existe un paciente registrado con este DNI");

        // Regla 3: fecha de nacimiento razonable
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        if (paciente.FechaNacimiento > hoy)
            return ResultadoOperacion.Falla("La fecha de nacimiento no puede ser futura");
        if (paciente.FechaNacimiento < hoy.AddYears(-120))
            return ResultadoOperacion.Falla("La fecha de nacimiento no es válida");

        // Regla 4: el plan de seguro y el distrito deben existir
        if (!await _planes.ExisteAsync(p => p.IdPlanSeguro == paciente.IdPlanSeguro))
            return ResultadoOperacion.Falla("El plan de seguro seleccionado no existe");
        if (!await _distritos.ExisteAsync(d => d.IdDistrito == paciente.IdDistrito))
            return ResultadoOperacion.Falla("El distrito seleccionado no existe");

        // Datos que decide el sistema (no el formulario):
        paciente.PorcentajeImportancia = 100m;   // todo paciente nuevo inicia con 100%
        paciente.RegistradoPorId = registradoPorId;   // quien lo registro en ventanilla
        paciente.UsuarioId = null;                    // aun no tiene cuenta web

        await _pacientes.AgregarAsync(paciente);
        await _pacientes.GuardarCambiosAsync();

        return ResultadoOperacion.Ok();
    }
}
