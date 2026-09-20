using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaCitas.Models;
using SistemaCitas.Repositories;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Services;

public class DashboardService : IDashboardService
{
    private const int CantidadAuditoria = 30;

    private readonly IRepositorio<Paciente> _pacientes;
    private readonly IRepositorio<Doctor> _doctores;
    private readonly IRepositorio<Ipress> _ipress;
    private readonly IRepositorio<Cita> _citas;
    private readonly IHistorialPuntajeRepositorio _historial;
    private readonly IAuditoriaService _auditoria;
    private readonly UserManager<Usuario> _userManager;

    public DashboardService(
        IRepositorio<Paciente> pacientes,
        IRepositorio<Doctor> doctores,
        IRepositorio<Ipress> ipress,
        IRepositorio<Cita> citas,
        IHistorialPuntajeRepositorio historial,
        IAuditoriaService auditoria,
        UserManager<Usuario> userManager)
    {
        _pacientes = pacientes;
        _doctores = doctores;
        _ipress = ipress;
        _citas = citas;
        _historial = historial;
        _auditoria = auditoria;
        _userManager = userManager;
    }

    public async Task<DashboardViewModel> ObtenerAsync()
    {
        var pacientes = (await _pacientes.BuscarAsync(p => p.Activo)).ToList();
        var doctores = (await _doctores.BuscarAsync(d => d.Activo)).ToList();
        var ipress = (await _ipress.BuscarAsync(i => i.Activo)).ToList();
        var citas = (await _citas.ObtenerTodosAsync()).ToList();

        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var asistidas = citas.Count(c => c.Estado == EstadoCita.Asistido);
        var faltas = citas.Count(c => c.Estado == EstadoCita.Falto);

        return new DashboardViewModel
        {
            TotalPacientes = pacientes.Count,
            TotalDoctores = doctores.Count,
            DoctoresHabilitados = doctores.Count(d => d.EstadoHabilitacion == "Habilitado"),
            TotalIpress = ipress.Count,
            TotalUsuariosActivos = await _userManager.Users.CountAsync(u => u.Activo),

            CitasPendientes = citas.Count(c => c.Estado == EstadoCita.Pendiente),
            CitasAsistidas = asistidas,
            CitasFaltas = faltas,
            CitasCanceladas = citas.Count(c => c.Estado == EstadoCita.Cancelado),
            CitasHoy = citas.Count(c => c.FechaCita == hoy && c.Estado != EstadoCita.Cancelado),

            CitasWeb = citas.Count(c => c.Canal == "Web" && c.Estado != EstadoCita.Cancelado),
            CitasPresenciales = citas.Count(c => c.Canal == "Presencial" && c.Estado != EstadoCita.Cancelado),

            // Mismos umbrales que usa el agendado (ver PrioridadService)
            PacientesPrioridadAlta = pacientes.Count(p => p.PorcentajeImportancia >= PrioridadService.UmbralPrioritario),
            PacientesBloqueados = pacientes.Count(p => p.PorcentajeImportancia < PrioridadService.UmbralBloqueo),
            PacientesPrioridadNormal = pacientes.Count(p =>
                p.PorcentajeImportancia < PrioridadService.UmbralPrioritario &&
                p.PorcentajeImportancia >= PrioridadService.UmbralBloqueo),

            TasaInasistencia = (asistidas + faltas) == 0 ? 0m : Math.Round(faltas * 100m / (asistidas + faltas), 1),

            Auditoria = await _historial.UltimosAsync(CantidadAuditoria),
            AuditoriaGeneral = await _auditoria.UltimosAsync(CantidadAuditoria)
        };
    }

    public async Task<UsuariosActivosViewModel> ListarUsuariosActivosAsync()
    {
        var usuarios = await _userManager.Users
            .Where(u => u.Activo)
            .OrderBy(u => u.Nombres)
            .ToListAsync();

        var filas = new List<UsuarioFila>();
        foreach (var u in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(u);
            filas.Add(new UsuarioFila
            {
                Nombre = u.Nombres,
                Correo = u.Email ?? u.UserName ?? "-",
                Rol = roles.Count == 0 ? "Sin rol" : string.Join(", ", roles)
            });
        }

        return new UsuariosActivosViewModel { Usuarios = filas };
    }
}
