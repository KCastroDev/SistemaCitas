using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaCitas.Models;

namespace SistemaCitas.ViewModels;

// "Mi agenda" del doctor (RF-12)
public class AgendaViewModel
{
    public Doctor Doctor { get; set; } = null!;
    public DateOnly Fecha { get; set; }
    public List<Cita> PorAtender { get; set; } = new();
    public List<Cita> Atendidas { get; set; } = new();
}

// Una fila de la receta (detalle del maestro-detalle, RF-13)
public class RecetaItemViewModel
{
    public int? IdMedicamento { get; set; }

    [Range(1, 99, ErrorMessage = "La cantidad debe estar entre 1 y 99")]
    public int? Cantidad { get; set; }

    [StringLength(200, ErrorMessage = "Las indicaciones admiten hasta 200 caracteres")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9\s.,;:()¿?¡!/%°+\-'""]+$", ErrorMessage = "Las indicaciones tienen caracteres no permitidos")]
    public string? Indicaciones { get; set; }
}

// Formulario de atencion medica: triaje + consulta + diagnostico + receta (RF-13)
public class AtencionViewModel
{
    public int IdCita { get; set; }

    // Solo para mostrar en pantalla (no se envian desde el formulario)
    public Cita? Cita { get; set; }
    public Paciente? Paciente { get; set; }

    // --- Triaje / observaciones ---
    [Required(ErrorMessage = "El peso es obligatorio")]
    [Range(1, 300, ErrorMessage = "El peso debe estar entre 1 y 300 kg")]
    [Display(Name = "Peso (kg)")]
    public decimal? PesoKg { get; set; }

    [Required(ErrorMessage = "La talla es obligatoria")]
    [Range(30, 250, ErrorMessage = "La talla debe estar entre 30 y 250 cm")]
    [Display(Name = "Talla (cm)")]
    public decimal? TallaCm { get; set; }

    [Required(ErrorMessage = "La temperatura es obligatoria")]
    [Range(30, 45, ErrorMessage = "La temperatura debe estar entre 30 y 45 °C")]
    [Display(Name = "Temperatura (°C)")]
    public decimal? TemperaturaC { get; set; }

    [RegularExpression(@"^\d{2,3}/\d{2,3}$", ErrorMessage = "La presión arterial debe tener el formato 120/80")]
    [Display(Name = "Presión arterial")]
    public string? PresionArterial { get; set; }

    [Range(30, 250, ErrorMessage = "La frecuencia cardiaca debe estar entre 30 y 250")]
    [Display(Name = "Frecuencia cardiaca (lpm)")]
    public int? FrecuenciaCardiaca { get; set; }

    [Range(50, 100, ErrorMessage = "La saturación de oxígeno debe estar entre 50 y 100")]
    [Display(Name = "Saturación de O2 (%)")]
    public int? SaturacionO2 { get; set; }

    // --- Consulta ---
    [Required(ErrorMessage = "El motivo de consulta es obligatorio")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "El motivo debe tener entre 5 y 500 caracteres")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9\s.,;:()¿?¡!/%°+\-'""]+$", ErrorMessage = "El motivo tiene caracteres no permitidos")]
    [Display(Name = "Motivo de consulta")]
    public string MotivoConsulta { get; set; } = "";

    [StringLength(1000, ErrorMessage = "Máximo 1000 caracteres")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9\s.,;:()¿?¡!/%°+\-'""]+$", ErrorMessage = "La sintomatología tiene caracteres no permitidos")]
    [Display(Name = "Sintomatología / padecimientos")]
    public string? Sintomatologia { get; set; }

    [StringLength(1000, ErrorMessage = "Máximo 1000 caracteres")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9\s.,;:()¿?¡!/%°+\-'""]+$", ErrorMessage = "Las observaciones tienen caracteres no permitidos")]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    // --- Diagnostico (CIE-10) ---
    [Required(ErrorMessage = "Elija un diagnóstico")]
    [Display(Name = "Diagnóstico (CIE-10)")]
    public string? CodigoCie10 { get; set; }

    [Required]
    [RegularExpression("^[PDR]$", ErrorMessage = "Tipo de diagnóstico no válido")]
    [Display(Name = "Tipo de diagnóstico")]
    public string TipoDiagnostico { get; set; } = "D";

    // --- Receta (hasta 3 medicamentos) ---
    public List<RecetaItemViewModel> Receta { get; set; } = new();

    // Listas desplegables
    public List<SelectListItem> Diagnosticos { get; set; } = new();
    public List<SelectListItem> Medicamentos { get; set; } = new();
}

// Historial clinico de un paciente (RF-14)
public class HistorialPacienteViewModel
{
    public Paciente Paciente { get; set; } = null!;
    public IEnumerable<AtencionMedica> Atenciones { get; set; } = new List<AtencionMedica>();
}
