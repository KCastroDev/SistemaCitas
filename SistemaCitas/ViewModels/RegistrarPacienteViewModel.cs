using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaCitas.ViewModels;

// Formulario de Admision (CU-01): registra a un paciente presencialmente.
// Es parecido a RegisterViewModel, pero SIN correo ni contrasena:
// el paciente registrado en ventanilla todavia no tiene cuenta web.
public class RegistrarPacienteViewModel
{
    [Required(ErrorMessage = "Ingrese el DNI")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener 8 dígitos")]
    [Display(Name = "DNI")]
    public string Dni { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese los nombres")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+([ '-][A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+)*$", ErrorMessage = "Los nombres solo pueden tener letras y espacios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 60 letras")]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese el apellido paterno")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+([ '-][A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+)*$", ErrorMessage = "El apellido paterno solo puede tener letras y espacios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 60 letras")]
    [Display(Name = "Apellido paterno")]
    public string ApellidoPaterno { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese el apellido materno")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+([ '-][A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+)*$", ErrorMessage = "El apellido materno solo puede tener letras y espacios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 60 letras")]
    [Display(Name = "Apellido materno")]
    public string ApellidoMaterno { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese la fecha de nacimiento")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de nacimiento")]
    public DateOnly? FechaNacimiento { get; set; }

    [Required(ErrorMessage = "Seleccione el sexo")]
    [RegularExpression("^[MF]$", ErrorMessage = "Seleccione M o F")]
    [Display(Name = "Sexo")]
    public string Sexo { get; set; } = null!;

    [RegularExpression(@"^9\d{8}$", ErrorMessage = "El teléfono debe tener 9 dígitos y empezar con 9")]
    [Display(Name = "Teléfono (opcional)")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "Seleccione el plan de seguro")]
    [Display(Name = "Plan de seguro")]
    public int? IdPlanSeguro { get; set; }

    [Required(ErrorMessage = "Seleccione el distrito")]
    [Display(Name = "Distrito")]
    public string? IdDistrito { get; set; }

    // RF-02: el encargado debe validar el DNI fisico en ventanilla
    [DebeSerVerdadero(ErrorMessage = "Debe confirmar que verificó el DNI físico del paciente")]
    [Display(Name = "Verifiqué el DNI físico del paciente")]
    public bool DniVerificado { get; set; }

    // Listas para los menus desplegables (se llenan desde la BD)
    [ValidateNever]
    public IEnumerable<SelectListItem> PlanesSeguro { get; set; } = new List<SelectListItem>();

    [ValidateNever]
    public IEnumerable<SelectListItem> Distritos { get; set; } = new List<SelectListItem>();
}
