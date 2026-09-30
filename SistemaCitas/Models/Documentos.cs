using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCitas.Models;

// Catalogo de documentos de identidad aceptados (RENIEC / Migraciones / MINSA).
// Cada tipo define su propio formato, y la combinacion Tipo + Numero es la que
// identifica de forma unica a una persona: un DNI 12345678 y un pasaporte 12345678
// son dos personas distintas.
[Table("TIPO_DOCUMENTO")]
public class TipoDocumento
{
    [Key]
    public int IdTipoDocumento { get; set; }

    // Codigo corto que forma parte del usuario de acceso (ej. "DNI-71000001")
    [Required, StringLength(3)]
    public string Codigo { get; set; } = null!;

    [Required, StringLength(60)]
    public string Nombre { get; set; } = null!;

    public int LongitudMinima { get; set; }
    public int LongitudMaxima { get; set; }

    // true = solo digitos (DNI, CNV); false = admite letras (carne de extranjeria, pasaporte)
    public bool SoloNumeros { get; set; } = true;

    public bool Activo { get; set; } = true;

    public ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();
}