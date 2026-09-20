using System.ComponentModel.DataAnnotations;

namespace SistemaCitas.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ingrese su correo electrónico")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese su contraseña")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = null!;

    [Display(Name = "Recordarme")]
    public bool Recordarme { get; set; }
}