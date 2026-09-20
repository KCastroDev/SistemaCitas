using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaCitas.ViewModels;

public class RegisterViewModel
{
    // ---------- Datos personales (van a la tabla PACIENTE) ----------

    [Required(ErrorMessage = "Ingrese su DNI")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener 8 dígitos")]
    [Display(Name = "DNI")]
    public string Dni { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese sus nombres")]
    [StringLength(60)]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese su apellido paterno")]
    [StringLength(60)]
    [Display(Name = "Apellido paterno")]
    public string ApellidoPaterno { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese su apellido materno")]
    [StringLength(60)]
    [Display(Name = "Apellido materno")]
    public string ApellidoMaterno { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese su fecha de nacimiento")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de nacimiento")]
    public DateOnly? FechaNacimiento { get; set; }

    [Required(ErrorMessage = "Seleccione su sexo")]
    [RegularExpression("^[MF]$", ErrorMessage = "Seleccione M o F")]
    [Display(Name = "Sexo")]
    public string Sexo { get; set; } = null!;

    [RegularExpression(@"^\d{9}$", ErrorMessage = "El teléfono debe tener 9 dígitos")]
    [Display(Name = "Teléfono (opcional)")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "Seleccione su plan de seguro")]
    [Display(Name = "Plan de seguro")]
    public int? IdPlanSeguro { get; set; }

    [Required(ErrorMessage = "Seleccione su distrito")]
    [Display(Name = "Distrito")]
    public string? IdDistrito { get; set; }

    // ---------- Datos de la cuenta (van a la tabla de usuarios) ----------

    [Required(ErrorMessage = "Ingrese su correo electrónico")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese una contraseña")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener mínimo 8 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = null!;

    [Required(ErrorMessage = "Confirme su contraseña")]
    [DataType(DataType.Password)]
    [Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarContrasena { get; set; } = null!;

    // Ley 29733: el registro exige aceptar los términos (RC-01)
    [DebeSerVerdadero(ErrorMessage = "Debe aceptar los términos y condiciones")]
    [Display(Name = "Acepto los términos y condiciones")]
    public bool AceptaTerminos { get; set; }

    // ---------- Listas para los menús desplegables (se llenan desde la BD) ----------

    [ValidateNever]
    public IEnumerable<SelectListItem> PlanesSeguro { get; set; } = new List<SelectListItem>();

    [ValidateNever]
    public IEnumerable<SelectListItem> Distritos { get; set; } = new List<SelectListItem>();
}