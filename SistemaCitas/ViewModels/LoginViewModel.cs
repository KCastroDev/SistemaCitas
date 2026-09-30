using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaCitas.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Seleccione el tipo de documento")]
    [Display(Name = "Tipo de documento")]
    public int IdTipoDocumento { get; set; } = 1;   // 1 = DNI

    [Required(ErrorMessage = "Ingrese su número de documento")]
    [StringLength(15, MinimumLength = 6, ErrorMessage = "El número de documento no es válido")]
    [Display(Name = "Número de documento")]
    public string Documento { get; set; } = null!;

    // Se llena desde la base para pintar el menu desplegable
    [ValidateNever]
    public IEnumerable<SelectListItem> TiposDocumento { get; set; } = new List<SelectListItem>();

    [Required(ErrorMessage = "Ingrese su contraseña")]
    [StringLength(100, ErrorMessage = "La contraseña admite hasta 100 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = null!;

    [Display(Name = "Recordarme")]
    public bool Recordarme { get; set; }
}