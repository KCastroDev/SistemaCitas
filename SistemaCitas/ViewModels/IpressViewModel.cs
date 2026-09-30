using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaCitas.ViewModels;

public class IpressViewModel
{
    public int IdIpress { get; set; }

    // Solo numeros: se acepta de 1 a 8 digitos y el sistema lo completa con ceros
    // a la izquierda hasta 8 (ej. "1" -> "00000001").
    [Required(ErrorMessage = "El código RENIPRESS es obligatorio")]
    [RegularExpression(@"^\d{1,8}$", ErrorMessage = "El código RENIPRESS solo admite números (máximo 8 dígitos)")]
    [Display(Name = "Código RENIPRESS")]
    public string CodigoRenipress { get; set; } = null!;

    // Valor que se guarda en la base de datos: siempre 8 digitos con ceros a la izquierda
    public string CodigoRenipressNormalizado =>
        (CodigoRenipress ?? string.Empty).Trim().PadLeft(8, '0');

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(150)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Seleccione el nivel de atención")]
    [Display(Name = "Nivel de atención")]
    public string NivelAtencion { get; set; } = null!;

    [StringLength(200)]
    public string? Direccion { get; set; }

    [Required(ErrorMessage = "Seleccione la Unidad Ejecutora")]
    [Display(Name = "Unidad Ejecutora")]
    public int IdUnidadEjecutora { get; set; }

    [Required(ErrorMessage = "Seleccione el distrito")]
    [Display(Name = "Distrito")]
    public string IdDistrito { get; set; } = null!;

    public bool Activo { get; set; } = true;

    public List<SelectListItem> UnidadesEjecutoras { get; set; } = new();
    public List<SelectListItem> Distritos { get; set; } = new();
    public List<SelectListItem> Niveles { get; set; } = new()
    {
        new SelectListItem("Nivel I", "I"),
        new SelectListItem("Nivel II", "II"),
        new SelectListItem("Nivel III", "III")
    };
}