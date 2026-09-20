using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitas.Data;
using SistemaCitas.Services;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Controllers;

// CU-04 (Gestionar doctores y horarios), CU-05 (validar CMP simulado).
// Solo el Administrador puede entrar; si no ha iniciado sesion, lo manda al Login.
[Authorize(Roles = Roles.Administrador)]
public class DoctoresController : Controller
{
    // El controlador solo habla con el servicio (capa de reglas de negocio).
    private readonly IDoctorService _service;

    public DoctoresController(IDoctorService service) => _service = service;

    // GET: Doctores
    public async Task<IActionResult> Index(string? buscar)
    {
        ViewData["Buscar"] = buscar;
        return View(await _service.ListarAsync(buscar));
    }

    // GET: Doctores/Create
    public async Task<IActionResult> Create()
    {
        var vm = new DoctorViewModel();
        await _service.CargarListasAsync(vm);
        return View(vm);
    }

    // POST: Doctores/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DoctorViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var resultado = await _service.RegistrarAsync(vm);
            if (resultado.Exito)
            {
                TempData["Mensaje"] = "Doctor registrado. Ahora valide su CMP y asígnele sus horarios.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, resultado.Error!);
        }

        await _service.CargarListasAsync(vm);
        return View(vm);
    }

    // POST: valida el CMP (simulado)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ValidarCmp(int id)
    {
        var resultado = await _service.ValidarCmpAsync(id);
        if (resultado.Exito) TempData["Mensaje"] = "CMP validado: el doctor figura como Habilitado.";
        else TempData["Error"] = resultado.Error;
        return RedirectToAction(nameof(Index));
    }

    // POST: baja logica / reactivar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id)
    {
        var resultado = await _service.CambiarEstadoAsync(id);
        if (resultado.Exito) TempData["Mensaje"] = "Estado del doctor actualizado.";
        else TempData["Error"] = resultado.Error;
        return RedirectToAction(nameof(Index));
    }

    // GET: Doctores/Horarios/5
    public async Task<IActionResult> Horarios(int id)
    {
        var doctor = await _service.ObtenerAsync(id);
        if (doctor == null) return NotFound();
        return View(new HorariosDoctorViewModel { Doctor = doctor });
    }

    // POST: agrega un bloque de horario (aplica RC-02)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarHorario(int id, [Bind(Prefix = "Nuevo")] HorarioViewModel nuevo)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m)) ?? "Revise los datos del horario.";
            return RedirectToAction(nameof(Horarios), new { id });
        }

        var resultado = await _service.AgregarHorarioAsync(id, nuevo);
        if (resultado.Exito) TempData["Mensaje"] = "Horario agregado correctamente.";
        else TempData["Error"] = resultado.Error;
        return RedirectToAction(nameof(Horarios), new { id });
    }

    // POST: quita un bloque de horario
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuitarHorario(int id, int idHorario)
    {
        var resultado = await _service.QuitarHorarioAsync(idHorario);
        if (resultado.Exito) TempData["Mensaje"] = "Horario eliminado.";
        else TempData["Error"] = resultado.Error;
        return RedirectToAction(nameof(Horarios), new { id });
    }
}
