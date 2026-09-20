using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaCitas.Data;
using SistemaCitas.Models;
using SistemaCitas.Services;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Controllers;

// Citas del paciente por la web (informe: CU-06 / CU-07, RF-07 y RF-08).
// Solo entran usuarios con rol Paciente. El paciente se toma SIEMPRE de la sesion,
// nunca de un dato del formulario (asi nadie puede agendar a nombre de otro).
[Authorize(Roles = Roles.Paciente)]
public class CitasController : Controller
{
    private readonly ICitaService _citaService;
    private readonly IPrioridadService _prioridad;
    private readonly UserManager<Usuario> _userManager;

    public CitasController(ICitaService citaService, IPrioridadService prioridad, UserManager<Usuario> userManager)
    {
        _citaService = citaService;
        _prioridad = prioridad;
        _userManager = userManager;
    }

    // Mis citas + mi porcentaje de importancia
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var paciente = await ObtenerPacienteActualAsync();
        if (paciente == null) return SinFicha();

        var modelo = new MisCitasViewModel
        {
            Paciente = paciente,
            Citas = await _citaService.ListarCitasPacienteAsync(paciente.IdPaciente),
            Aviso = _prioridad.Describir(paciente.PorcentajeImportancia),
            Bloqueado = _prioridad.EstaBloqueado(paciente.PorcentajeImportancia)
        };
        return View(modelo);
    }

    // Pantalla para agendar: especialidad -> doctor -> fecha -> hora
    [HttpGet]
    public async Task<IActionResult> Nueva(int? idEspecialidad, int? idDoctor, DateOnly? fecha)
    {
        var paciente = await ObtenerPacienteActualAsync();
        if (paciente == null) return SinFicha();

        var modelo = await _citaService.ArmarBusquedaAsync(paciente, idEspecialidad, idDoctor, fecha, presencial: false);
        return View(modelo);
    }

    // Confirma la reserva
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reservar(int idDoctor, DateOnly fecha, TimeOnly hora, int? idEspecialidad)
    {
        var paciente = await ObtenerPacienteActualAsync();
        if (paciente == null) return SinFicha();

        var resultado = await _citaService.AgendarAsync(paciente.IdPaciente, idDoctor, fecha, hora, "Web");

        if (!resultado.Exito)
        {
            TempData["Error"] = resultado.Error;
            return RedirectToAction(nameof(Nueva), new { idEspecialidad, idDoctor, fecha = fecha.ToString("yyyy-MM-dd") });
        }

        TempData["Mensaje"] = $"Cita confirmada para el {fecha:dd/MM/yyyy} a las {hora:HH:mm}.";
        return RedirectToAction(nameof(Index));
    }

    // El paciente cancela su cita (no se penaliza)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var paciente = await ObtenerPacienteActualAsync();
        if (paciente == null) return SinFicha();

        var resultado = await _citaService.CancelarAsync(id, paciente.IdPaciente);
        if (resultado.Exito) TempData["Mensaje"] = "La cita fue cancelada.";
        else TempData["Error"] = resultado.Error;

        return RedirectToAction(nameof(Index));
    }

    private async Task<Paciente?> ObtenerPacienteActualAsync()
    {
        var usuarioId = _userManager.GetUserId(User);
        return usuarioId == null ? null : await _citaService.ObtenerPacientePorUsuarioAsync(usuarioId);
    }

    // Un usuario con rol Paciente que no tiene ficha de paciente (no deberia pasar)
    private IActionResult SinFicha()
    {
        TempData["Error"] = "Su cuenta no tiene una ficha de paciente asociada. Acuda a Admisión.";
        return RedirectToAction("Index", "Home");
    }
}
