using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitas.Data;
using SistemaCitas.Services;

namespace SistemaCitas.Controllers;

// Estadisticas y auditoria (informe: RF-15 / CU-10). Solo para el Administrador.
[Authorize(Roles = Roles.Administrador)]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboard;

    public DashboardController(IDashboardService dashboard)
    {
        _dashboard = dashboard;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var modelo = await _dashboard.ObtenerAsync();
        return View(modelo);
    }

    // RF-15: usuarios activos con su rol
    [HttpGet]
    public async Task<IActionResult> Usuarios()
    {
        var modelo = await _dashboard.ListarUsuariosActivosAsync();
        return View(modelo);
    }
}
