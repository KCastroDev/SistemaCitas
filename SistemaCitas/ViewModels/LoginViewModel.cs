using System.ComponentModel.DataAnnotations;

namespace SistemaCitas.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ingrese su correo electrónico")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
    [StringLength(100, ErrorMessage = "El correo admite hasta 100 caracteres")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese su contraseña")]
    [StringLength(100, ErrorMessage = "La contraseña admite hasta 100 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = null!;

    [Display(Name = "Recordarme")]
    public bool Recordarme { get; set; }
}