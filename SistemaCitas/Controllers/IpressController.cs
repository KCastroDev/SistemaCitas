using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaCitas.Models;
using SistemaCitas.Service;
using SistemaCitas.Services;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Controllers;

public class IpressController : Controller
{
    private readonly IIpressService _service;

    public IpressController(IIpressService service)
    {
        _service = service;
    }

    // GET: Ipress
    public async Task<IActionResult> Index()
    {
        var lista = await _service.ListarAsync();
        return View(lista);
    }

    // GET: Ipress/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var ipress = await _service.ObtenerPorIdAsync(id);
        if (ipress is null) return NotFound();
        return View(ipress);
    }

    // GET: Ipress/Create
    public async Task<IActionResult> Create()
    {
        var vm = new IpressViewModel();
        await CargarCombosAsync(vm);
        return View(vm);
    }

    // POST: Ipress/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IpressViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await CargarCombosAsync(vm);
            return View(vm);
        }

        var ipress = new Ipress
        {
            CodigoRenipress = vm.CodigoRenipress,
            Nombre = vm.Nombre,
            NivelAtencion = vm.NivelAtencion,
            Direccion = vm.Direccion,
            IdUnidadEjecutora = vm.IdUnidadEjecutora,
            IdDistrito = vm.IdDistrito
        };

        var (exito, error) = await _service.CrearAsync(ipress);
        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            await CargarCombosAsync(vm);
            return View(vm);
        }

        TempData["Mensaje"] = "IPRESS registrada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Ipress/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var ipress = await _service.ObtenerPorIdAsync(id);
        if (ipress is null) return NotFound();

        var vm = new IpressViewModel
        {
            IdIpress = ipress.IdIpress,
            CodigoRenipress = ipress.CodigoRenipress,
            Nombre = ipress.Nombre,
            NivelAtencion = ipress.NivelAtencion,
            Direccion = ipress.Direccion,
            IdUnidadEjecutora = ipress.IdUnidadEjecutora,
            IdDistrito = ipress.IdDistrito,
            Activo = ipress.Activo
        };
        await CargarCombosAsync(vm);
        return View(vm);
    }

    // POST: Ipress/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, IpressViewModel vm)
    {
        if (id != vm.IdIpress) return NotFound();

        if (!ModelState.IsValid)
        {
            await CargarCombosAsync(vm);
            return View(vm);
        }

        var ipress = new Ipress
        {
            IdIpress = vm.IdIpress,
            CodigoRenipress = vm.CodigoRenipress,
            Nombre = vm.Nombre,
            NivelAtencion = vm.NivelAtencion,
            Direccion = vm.Direccion,
            IdUnidadEjecutora = vm.IdUnidadEjecutora,
            IdDistrito = vm.IdDistrito,
            Activo = vm.Activo
        };

        var (exito, error) = await _service.ActualizarAsync(ipress);
        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            await CargarCombosAsync(vm);
            return View(vm);
        }

        TempData["Mensaje"] = "IPRESS actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Ipress/Inhabilitar/5  (RF-12: no se elimina físicamente, se inhabilita)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inhabilitar(int id)
    {
        await _service.InhabilitarAsync(id);
        TempData["Mensaje"] = "IPRESS inhabilitada.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Ipress/Habilitar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Habilitar(int id)
    {
        await _service.HabilitarAsync(id);
        TempData["Mensaje"] = "IPRESS habilitada.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarCombosAsync(IpressViewModel vm)
    {
        var unidades = await _service.ListarUnidadesEjecutorasAsync();
        var distritos = await _service.ListarDistritosAsync();

        vm.UnidadesEjecutoras = unidades
            .Select(u => new SelectListItem(u.Nombre, u.IdUnidadEjecutora.ToString()))
            .ToList();

        vm.Distritos = distritos
            .Select(d => new SelectListItem(d.Nombre, d.IdDistrito))
            .ToList();
    }
}