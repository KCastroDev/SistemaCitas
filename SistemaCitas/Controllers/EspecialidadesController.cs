using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitas.Models;
using SistemaCitas.Repositories;
using SistemaCitas.Services;

namespace SistemaCitas.Controllers;

[Authorize(Roles = "Administrador")]   // solo el Administrador entra; si no inicio sesion, lo manda al Login
public class EspecialidadesController : Controller
{
    // El controlador ya NO conoce el DbContext: solo habla con el repositorio.
    private readonly IEspecialidadRepositorio _repo;
    private readonly IAuditoriaService _auditoria;

    public EspecialidadesController(IEspecialidadRepositorio repo, IAuditoriaService auditoria)
    {
        _repo = repo;
        _auditoria = auditoria;
    }

    // GET: Especialidades
    public async Task<IActionResult> Index(string? buscar)
    {
        ViewData["Buscar"] = buscar;
        return View(await _repo.ListarAsync(buscar));
    }

    // GET: Especialidades/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var especialidad = await _repo.ObtenerPorIdAsync(id);
        return especialidad == null ? NotFound() : View(especialidad);
    }

    // GET: Especialidades/Create
    public IActionResult Create() => View();

    // POST: Especialidades/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre")] Especialidad especialidad)
    {
        if (ModelState.IsValid)
        {
            await _repo.AgregarAsync(especialidad);
            await _repo.GuardarCambiosAsync();

            await _auditoria.RegistrarAsync("Especialidad", "Crear", especialidad.Nombre, null, especialidad.Nombre);

            TempData["Mensaje"] = "Especialidad registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(especialidad);
    }

    // GET: Especialidades/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var especialidad = await _repo.ObtenerPorIdAsync(id);
        return especialidad == null ? NotFound() : View(especialidad);
    }

    // POST: Especialidades/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdEspecialidad,Nombre,Activo")] Especialidad especialidad)
    {
        if (id != especialidad.IdEspecialidad) return NotFound();

        if (ModelState.IsValid)
        {
            // Traemos la entidad tal como esta rastreada por EF y le copiamos los valores
            // nuevos encima, en vez de llamar a Actualizar() con el objeto del formulario:
            // asi evitamos tener dos instancias con la misma clave primaria rastreadas a la
            // vez, y de paso nos queda el "antes" para la auditoria (RNF-01).
            var anterior = await _repo.ObtenerPorIdAsync(id);
            if (anterior == null) return NotFound();

            var nombreAnterior = anterior.Nombre;

            anterior.Nombre = especialidad.Nombre;
            anterior.Activo = especialidad.Activo;

            await _repo.GuardarCambiosAsync();

            await _auditoria.RegistrarAsync("Especialidad", "Editar", nombreAnterior, nombreAnterior, anterior.Nombre);

            TempData["Mensaje"] = "Especialidad actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(especialidad);
    }

    // POST: baja logica (no se elimina, se desactiva)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id)
    {
        var especialidad = await _repo.ObtenerPorIdAsync(id);
        if (especialidad == null) return NotFound();

        especialidad.Activo = !especialidad.Activo;
        _repo.Actualizar(especialidad);
        await _repo.GuardarCambiosAsync();

        await _auditoria.RegistrarAsync("Especialidad", "Editar", especialidad.Nombre,
            especialidad.Activo ? "Inactiva" : "Activa",
            especialidad.Activo ? "Activa" : "Inactiva");

        TempData["Mensaje"] = especialidad.Activo
            ? $"La especialidad '{especialidad.Nombre}' fue reactivada."
            : $"La especialidad '{especialidad.Nombre}' fue desactivada.";

        return RedirectToAction(nameof(Index));
    }

    // Validacion remota
    [AcceptVerbs("GET", "POST")]
    [AllowAnonymous]
    public async Task<IActionResult> NombreDisponible(string nombre, int idEspecialidad)
    {
        bool repetido = await _repo.NombreRepetidoAsync(nombre, idEspecialidad);
        return repetido
            ? Json($"La especialidad '{nombre}' ya esta registrada.")
            : Json(true);
    }
}