using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaCitas.ViewModels;

public class RegisterViewModel
{
    // ---------- Datos personales (van a la tabla PACIENTE) ----------

    [Required(ErrorMessage = "Seleccione el tipo de documento")]
    [Display(Name = "Tipo de documento")]
    public int IdTipoDocumento { get; set; } = 1;   // 1 = DNI

    // El formato concreto (largo y si admite letras) lo valida el servidor
    // segun el tipo elegido, con Helpers/Documento.Validar
    [Required(ErrorMessage = "Ingrese su número de documento")]
    [StringLength(15, MinimumLength = 6, ErrorMessage = "El número de documento no es válido")]
    [Display(Name = "Número de documento")]
    public string Dni { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese sus nombres")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+([ '-][A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+)*$", ErrorMessage = "Los nombres solo pueden tener letras y espacios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 60 letras")]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese su apellido paterno")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+([ '-][A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+)*$", ErrorMessage = "El apellido paterno solo puede tener letras y espacios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 60 letras")]
    [Display(Name = "Apellido paterno")]
    public string ApellidoPaterno { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese su apellido materno")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+([ '-][A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+)*$", ErrorMessage = "El apellido materno solo puede tener letras y espacios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 60 letras")]
    [Display(Name = "Apellido materno")]
    public string ApellidoMaterno { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese su fecha de nacimiento")]
    [DataType(DataType.Date)]
    // Cuenta web: solo mayores de edad (Ley 29733). Los menores se registran en Admision.
    [FechaNacimientoValida(EdadMinima = 18,
        ErrorMessage = "La fecha no es válida: debe ser pasada y corresponder a una persona mayor de 18 años")]
    [Display(Name = "Fecha de nacimiento")]
    public DateOnly? FechaNacimiento { get; set; }

    [Required(ErrorMessage = "Seleccione su sexo")]
    [RegularExpression("^[MF]$", ErrorMessage = "Seleccione M o F")]
    [Display(Name = "Sexo")]
    public string Sexo { get; set; } = null!;

    [RegularExpression(@"^9\d{8}$", ErrorMessage = "El teléfono debe tener 9 dígitos y empezar con 9")]
    [Display(Name = "Teléfono (opcional)")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "Seleccione su plan de seguro")]
    [Display(Name = "Plan de seguro")]
    public int? IdPlanSeguro { get; set; }

    [Required(ErrorMessage = "Seleccione su distrito")]
    [Display(Name = "Distrito")]
    public string? IdDistrito { get; set; }

    // ---------- Datos de la cuenta (van a la tabla de usuarios) ----------

    // Opcional: sirve para recuperar la contrasena sin ir a ventanilla.
    // Si no lo registra, el restablecimiento se hace en Admision con su DNI fisico.
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
    [Display(Name = "Correo electrónico (opcional)")]
    public string? Correo { get; set; }

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
    public IEnumerable<SelectListItem> TiposDocumento { get; set; } = new List<SelectListItem>();

    [ValidateNever]
    public IEnumerable<SelectListItem> PlanesSeguro { get; set; } = new List<SelectListItem>();

    [ValidateNever]
    public IEnumerable<SelectListItem> Distritos { get; set; } = new List<SelectListItem>();
}