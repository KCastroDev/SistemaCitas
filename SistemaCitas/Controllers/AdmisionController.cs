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

    public AdmisionController(
        IPacienteService pacienteService,
        UserManager<Usuario> userManager,
        IRepositorio<PlanSeguro> planes,
        IRepositorio<Distrito> distritos)
    {
        _pacienteService = pacienteService;
        _userManager = userManager;
        _planes = planes;
        _distritos = distritos;
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
