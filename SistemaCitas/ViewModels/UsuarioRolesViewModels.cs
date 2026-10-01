using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaCitas.ViewModels;

// Fila de la lista de usuarios con su rol actual (pantalla de administracion de roles).
public class UsuarioRolViewModel
{
    public string Id { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Nombres { get; set; } = null!;
    public bool Activo { get; set; }
    public string Rol { get; set; } = "(sin rol)";
}

// Formulario para cambiar el rol de un usuario (pantalla de administracion de roles).
public class EditarRolViewModel
{
    public string Id { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Nombres { get; set; } = null!;
    public string? RolActual { get; set; }

    [Required(ErrorMessage = "Seleccione un rol")]
    [Display(Name = "Nuevo rol")]
    public string NuevoRol { get; set; } = null!;

    public IEnumerable<SelectListItem> Roles { get; set; } = Enumerable.Empty<SelectListItem>();
}
