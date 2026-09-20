using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaCitas.Data;
using SistemaCitas.Models;
using SistemaCitas.Repositories;
using SistemaCitas.Services;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Controllers;

// Panel de Admision (informe: PR-03 / CU-01 y CU-08).
// Solo entra el personal de Admision (el Administrador tambien, para supervisar y probar).
[Authorize(Roles = Roles.Admision + "," + Roles.Administrador)]
public class AdmisionController : Controller
{
    private readonly IPacienteService _pacienteService;
    private readonly UserManager<Usuario> _userManager;
    private readonly IRepositorio<PlanSeguro> _planes;
    private readonly IRepositorio<Distrito> _distritos;
    private readonly ICitaService _citaService;

    public AdmisionController(
        IPacienteService pacienteService,
        UserManager<Usuario> userManager,
        IRepositorio<PlanSeguro> planes,
        IRepositorio<Distrito> distritos,
        ICitaService citaService)
    {
        _pacienteService = pacienteService;
        _userManager = userManager;
        _planes = planes;
        _distritos = distritos;
        _citaService = citaService;
    }

    // Lista y busca pacientes (por DNI, nombres o apellidos)
    [HttpGet]
    public async Task<IActionResult> Index(string? buscar)
    {
        ViewData["Buscar"] = buscar;
        var pacientes = await _pacienteService.BuscarAsync(buscar);
        return View(pacientes);
    }

    // CU-01: muestra el formulario vacio
    [HttpGet]
    public async Task<IActionResult> Registrar()
    {
        var modelo = new RegistrarPacienteViewModel();
        await CargarListasAsync(modelo);
        return View(modelo);
    }

    // CU-01: recibe el formulario
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(RegistrarPacienteViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(modelo);
            return View(modelo);
        }

        // Pasamos los datos del formulario a un Paciente
        var paciente = new Paciente
        {
            Dni = modelo.Dni,
            Nombres = modelo.Nombres.Trim(),
            ApellidoPaterno = modelo.ApellidoPaterno.Trim(),
            ApellidoMaterno = modelo.ApellidoMaterno.Trim(),
            FechaNacimiento = modelo.FechaNacimiento!.Value,
            Sexo = modelo.Sexo,
            Telefono = modelo.Telefono,
            IdPlanSeguro = modelo.IdPlanSeguro!.Value,
            IdDistrito = modelo.IdDistrito!
        };

        // Las reglas de negocio las aplica el servicio
        var resultado = await _pacienteService.RegistrarPresencialAsync(paciente, _userManager.GetUserId(User)!);

        if (!resultado.Exito)
        {
            ModelState.AddModelError(string.Empty, resultado.Error!);
            await CargarListasAsync(modelo);
            return View(modelo);
        }

        TempData["Mensaje"] = $"Paciente {paciente.Nombres} {paciente.ApellidoPaterno} registrado correctamente.";
        return RedirectToAction(nameof(Index), new { buscar = modelo.Dni });
    }

    // ---------------------------------------------------------------
    // Citas presenciales y control del dia (CU-08, RF-09, RF-10, RF-11)
    // ---------------------------------------------------------------

    // RF-09: Admision agenda una cita para un paciente que esta en ventanilla
    [HttpGet]
    public async Task<IActionResult> Agendar(int idPaciente, int? idEspecialidad, int? idDoctor, DateOnly? fecha)
    {
        var paciente = await _citaService.ObtenerPacienteAsync(idPaciente);
        if (paciente == null) return NotFound();

        var modelo = await _citaService.ArmarBusquedaAsync(paciente, idEspecialidad, idDoctor, fecha, presencial: true);
        return View(modelo);
    }

    // RF-09 + RC-03: confirma la cita presencial (exige marcar que se verifico el DNI fisico)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reservar(int idPaciente, int idDoctor, DateOnly fecha, TimeOnly hora, int? idEspecialidad, bool dniVerificado)
    {
        var resultado = await _citaService.AgendarAsync(idPaciente, idDoctor, fecha, hora, "Presencial", dniVerificado);

        if (!resultado.Exito)
        {
            TempData["Error"] = resultado.Error;
            return RedirectToAction(nameof(Agendar), new { idPaciente, idEspecialidad, idDoctor, fecha = fecha.ToString("yyyy-MM-dd") });
        }

        TempData["Mensaje"] = $"Cita presencial registrada para el {fecha:dd/MM/yyyy} a las {hora:HH:mm}.";
        return RedirectToAction(nameof(Citas), new { fecha = fecha.ToString("yyyy-MM-dd") });
    }

    // RF-10: lista las citas de un dia para marcar Asistio / Falto / Cancelar
    [HttpGet]
    public async Task<IActionResult> Citas(DateOnly? fecha)
    {
        var dia = fecha ?? DateOnly.FromDateTime(DateTime.Now);
        var modelo = new CitasDelDiaViewModel
        {
            Fecha = dia,
            Citas = await _citaService.ListarCitasPorFechaAsync(dia)
        };
        return View(modelo);
    }

    // RF-10 + RF-11: cambia el estado de la cita (si "Falto", baja la importancia del paciente)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstadoCita(int id, EstadoCita estado, DateOnly fecha)
    {
        var resultado = await _citaService.CambiarEstadoAsync(id, estado, _userManager.GetUserId(User)!);

        if (resultado.Exito)
            TempData["Mensaje"] = estado == EstadoCita.Falto
                ? "Inasistencia registrada: se redujo la importancia del paciente (RF-11)."
                : $"Cita marcada como {estado}.";
        else
            TempData["Error"] = resultado.Error;

        return RedirectToAction(nameof(Citas), new { fecha = fecha.ToString("yyyy-MM-dd") });
    }

    private async Task CargarListasAsync(RegistrarPacienteViewModel modelo)
    {
        var planes = await _planes.ObtenerTodosAsync();
        var distritos = await _distritos.ObtenerTodosAsync();

        modelo.PlanesSeguro = planes
            .OrderBy(p => p.Nombre)
            .Select(p => new SelectListItem(p.Nombre, p.IdPlanSeguro.ToString()));

        modelo.Distritos = distritos
            .OrderBy(d => d.Nombre)
            .Select(d => new SelectListItem(d.Nombre, d.IdDistrito));
    }
}
