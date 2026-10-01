using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaCitas.Data;
using SistemaCitas.Models;
using SistemaCitas.Services;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Controllers;

// Pantalla para que el Administrador asigne o cambie el rol de acceso de cada usuario
// (Administrador, Admision, Doctor, Paciente, Digitador). Solo el Administrador puede entrar.
[Authorize(Roles = Roles.Administrador)]
public class UsuariosController : Controller
{
    private readonly UserManager<Usuario> _userManager;
    private readonly IAuditoriaService _auditoria;

    public UsuariosController(UserManager<Usuario> userManager, IAuditoriaService auditoria)
    {
        _userManager = userManager;
        _auditoria = auditoria;
    }

    // GET: Usuarios
    public async Task<IActionResult> Index(string? buscar)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
            query = query.Where(u => u.UserName!.Contains(buscar) || u.Nombres.Contains(buscar));

        var usuarios = await query.OrderBy(u => u.Nombres).ToListAsync();

        var lista = new List<UsuarioRolViewModel>();
        foreach (var usuario in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            lista.Add(new UsuarioRolViewModel
            {
                Id = usuario.Id,
                UserName = usuario.UserName ?? "",
                Nombres = usuario.Nombres,
                Activo = usuario.Activo,
                Rol = roles.FirstOrDefault() ?? "(sin rol)"
            });
        }

        ViewData["Buscar"] = buscar;
        return View(lista);
    }

    // GET: Usuarios/EditarRol/5
    public async Task<IActionResult> EditarRol(string? id)
    {
        if (id == null) return NotFound();

        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null) return NotFound();

        var rolesActuales = await _userManager.GetRolesAsync(usuario);

        var vm = new EditarRolViewModel
        {
            Id = usuario.Id,
            UserName = usuario.UserName ?? "",
            Nombres = usuario.Nombres,
            RolActual = rolesActuales.FirstOrDefault(),
            NuevoRol = rolesActuales.FirstOrDefault() ?? "",
            Roles = Roles.Todos.Select(r => new SelectListItem(r, r))
        };

        return View(vm);
    }

    // POST: Usuarios/EditarRol/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarRol(string id, EditarRolViewModel vm)
    {
        if (id != vm.Id) return NotFound();

        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null) return NotFound();

        if (string.IsNullOrWhiteSpace(vm.NuevoRol) || !Roles.Todos.Contains(vm.NuevoRol))
            ModelState.AddModelError(nameof(vm.NuevoRol), "Seleccione un rol válido.");

        // Evita que un administrador se quite a si mismo el rol de Administrador y se bloquee el acceso.
        if (usuario.Id == _userManager.GetUserId(User) && vm.NuevoRol != Roles.Administrador)
            ModelState.AddModelError(string.Empty, "No puede quitarse a sí mismo el rol de Administrador.");

        if (!ModelState.IsValid)
        {
            vm.UserName = usuario.UserName ?? "";
            vm.Nombres = usuario.Nombres;
            vm.RolActual = (await _userManager.GetRolesAsync(usuario)).FirstOrDefault();
            vm.Roles = Roles.Todos.Select(r => new SelectListItem(r, r));
            return View(vm);
        }

        var rolesActuales = await _userManager.GetRolesAsync(usuario);
        var rolAnterior = rolesActuales.FirstOrDefault();

        if (rolAnterior != vm.NuevoRol)
        {
            if (rolesActuales.Any())
                await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);

            await _userManager.AddToRoleAsync(usuario, vm.NuevoRol);

            await _auditoria.RegistrarAsync("Usuario", "Editar rol", usuario.UserName,
                rolAnterior ?? "(sin rol)", vm.NuevoRol);

            TempData["Mensaje"] = $"El rol de {usuario.Nombres} fue cambiado a {vm.NuevoRol}.";
        }
        else
        {
            TempData["Mensaje"] = "El usuario ya tenía ese rol asignado.";
        }

        return RedirectToAction(nameof(Index));
    }
}
