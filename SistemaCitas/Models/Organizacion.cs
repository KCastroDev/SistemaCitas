using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCitas.Models;

[Table("UNIDAD_EJECUTORA")]
public class UnidadEjecutora
{
    [Key]
    public int IdUnidadEjecutora { get; set; }

    [Required, StringLength(20)]
    public string Codigo { get; set; } = null!;

    [Required, StringLength(150)]
    public string Nombre { get; set; } = null!;

    public ICollection<Ipress> Ipress { get; set; } = new List<Ipress>();
}

[Table("IPRESS")]
public class Ipress
{
    [Key]
    public int IdIpress { get; set; }

    [Required, StringLength(20)]
    public string CodigoRenipress { get; set; } = null!;

    [Required, StringLength(150)]
    public string Nombre { get; set; } = null!;

    [Required, StringLength(3)]
    public string NivelAtencion { get; set; } = null!;    // I, II, III

    [StringLength(200)]
    public string? Direccion { get; set; }

    public bool Activo { get; set; } = true;

    public int IdUnidadEjecutora { get; set; }
    public UnidadEjecutora UnidadEjecutora { get; set; } = null!;

    [StringLength(6)]
    public string IdDistrito { get; set; } = null!;
    public Distrito Distrito { get; set; } = null!;

    public ICollection<Doctor> Doctores { get; set; } = new List<Doctor>();
}

[Table("PLAN_SEGURO")]
public class PlanSeguro
{
    [Key]
    public int IdPlanSeguro { get; set; }

    [Required, StringLength(60)]
    public string Nombre { get; set; } = null!;           // SIS Gratuito, Para Todos...

    public ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();
}

[Table("TIPO_CONTRATO")]
public class TipoContrato
{
    [Key]
    public int IdTipoContrato { get; set; }

    [Required, StringLength(40)]
    public string Nombre { get; set; } = null!;           // Honorarios, Part time, Full day

    public int HorasSemanales { get; set; }               // tope de horas para validar horarios

    public ICollection<Doctor> Doctores { get; set; } = new List<Doctor>();
}

[Table("ESPECIALIDAD")]
public class Especialidad
{
    [Key]
    public int IdEspecialidad { get; set; }

    [Required, StringLength(80)]
    public string Nombre { get; set; } = null!;

    public ICollection<DoctorEspecialidad> DoctorEspecialidades { get; set; } = new List<DoctorEspecialidad>();
}