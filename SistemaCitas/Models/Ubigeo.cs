using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCitas.Models;

[Table("DEPARTAMENTO")]
public class Departamento
{
    [Key, StringLength(2)]
    public string IdDepartamento { get; set; } = null!;   // "numero puede ser :13"

    [Required, StringLength(60)]
    public string Nombre { get; set; } = null!;           // "que seria = a La Libertad"

    public ICollection<Provincia> Provincias { get; set; } = new List<Provincia>();
}

[Table("PROVINCIA")]
public class Provincia
{
    [Key, StringLength(4)]
    public string IdProvincia { get; set; } = null!;      // "1301"

    [Required, StringLength(60)]
    public string Nombre { get; set; } = null!;           // "Trujillo"

    [StringLength(2)]
    public string IdDepartamento { get; set; } = null!;
    public Departamento Departamento { get; set; } = null!;

    public ICollection<Distrito> Distritos { get; set; } = new List<Distrito>();
}

[Table("DISTRITO")]
public class Distrito
{
    [Key, StringLength(6)]
    public string IdDistrito { get; set; } = null!;       // ubigeo INEI: "130104"

    [Required, StringLength(60)]
    public string Nombre { get; set; } = null!;           // "Laredo"

    [StringLength(4)]
    public string IdProvincia { get; set; } = null!;
    public Provincia Provincia { get; set; } = null!;
}