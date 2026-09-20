using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaCitas.Models;

namespace SistemaCitas.ViewModels;

// Formulario para registrar un doctor (RF-04).
// Incluye los datos de su cuenta de acceso (correo y contrasena), que se crea con el rol "Doctor".
public class DoctorViewModel
{
    [Required(ErrorMessage = "Ingrese el DNI")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener 8 dígitos")]
    [Display(Name = "DNI")]
    public string Dni { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese el número de colegiatura (CMP)")]
    [RegularExpression(@"^\d{5,6}$", ErrorMessage = "El CMP debe tener 5 o 6 dígitos")]
    [Display(Name = "CMP (colegiatura)")]
    public string Cmp { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese los nombres")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+( [A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+)*$", ErrorMessage = "Los nombres solo pueden tener letras y espacios (sin números ni signos)")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 30 letras")]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese el apellido paterno")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+( [A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+)*$", ErrorMessage = "El apellido paterno solo puede tener letras y espacios (sin números ni signos)")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 20 letras")]
    [Display(Name = "Apellido paterno")]
    public string ApellidoPaterno { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese el apellido materno")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+( [A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+)*$", ErrorMessage = "El apellido materno solo puede tener letras y espacios (sin números ni signos)")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 20 letras")]
    [Display(Name = "Apellido materno")]
    public string ApellidoMaterno { get; set; } = null!;

    [Required(ErrorMessage = "Seleccione la IPRESS")]
    [Display(Name = "IPRESS")]
    public int? IdIpress { get; set; }

    [Required(ErrorMessage = "Seleccione el tipo de contrato")]
    [Display(Name = "Tipo de contrato")]
    public int? IdTipoContrato { get; set; }

    [Required(ErrorMessage = "Seleccione la especialidad")]
    [Display(Name = "Especialidad")]
    public int? IdEspecialidad { get; set; }

    [Required(ErrorMessage = "Ingrese el correo")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
    [StringLength(100)]
    [Display(Name = "Correo (usuario de acceso)")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese una contraseña")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = null!;

    // Listas de los menus desplegables (se llenan desde la BD, no se validan)
    [ValidateNever] public List<SelectListItem> Ipresses { get; set; } = new();
    [ValidateNever] public List<SelectListItem> Contratos { get; set; } = new();
    [ValidateNever] public List<SelectListItem> Especialidades { get; set; } = new();
}

// Formulario para agregar un bloque de horario a un doctor (RF-06).
public class HorarioViewModel
{
    [Range(1, 7, ErrorMessage = "Seleccione el día")]
    [Display(Name = "Día")]
    public int DiaSemana { get; set; } = 1;

    [Required(ErrorMessage = "Ingrese la hora de inicio")]
    [Display(Name = "Hora de inicio")]
    public TimeOnly? HoraInicio { get; set; }

    [Required(ErrorMessage = "Ingrese la hora de fin")]
    [Display(Name = "Hora de fin")]
    public TimeOnly? HoraFin { get; set; }

    [Range(1, 10, ErrorMessage = "Los cupos por hora deben estar entre 1 y 10")]
    [Display(Name = "Cupos por hora")]
    public int CuposPorHora { get; set; } = 4;
}

// Datos que necesita la pantalla de horarios de un doctor.
public class HorariosDoctorViewModel
{
    public Doctor Doctor { get; set; } = null!;
    public HorarioViewModel Nuevo { get; set; } = new();

    // Horas semanales ya asignadas vs las que permite su contrato (RC-02)
    public double HorasAsignadas =>
        Doctor.Horarios.Where(h => h.Activo).Sum(h => (h.HoraFin - h.HoraInicio).TotalHours);

    public int HorasPermitidas => Doctor.TipoContrato.HorasSemanales;
}
