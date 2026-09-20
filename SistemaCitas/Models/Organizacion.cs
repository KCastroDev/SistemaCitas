using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

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
    public string NivelAtencion { get; set; } = null!;

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
    public string Nombre { get; set; } = null!;

    public ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();
}

[Table("TIPO_CONTRATO")]
public class TipoContrato
{
    [Key]
    public int IdTipoContrato { get; set; }

    [Required, StringLength(40)]
    public string Nombre { get; set; } = null!;

    public int HorasSemanales { get; set; }

    public ICollection<Doctor> Doctores { get; set; } = new List<Doctor>();
}

[Table("ESPECIALIDAD")]
public class Especialidad
{
    [Key]
    public int IdEspecialidad { get; set; }

    [Required(ErrorMessage = "El nombre de la especialidad es obligatorio")]
    [StringLength(80, MinimumLength = 4, ErrorMessage = "El nombre debe tener entre 4 y 80 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
    [Display(Name = "Nombre de la especialidad")]
    [Remote(action: "NombreDisponible", controller: "Especialidades", AdditionalFields = nameof(IdEspecialidad))]
    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; } = true;

    public ICollection<DoctorEspecialidad> DoctorEspecialidades { get; set; } = new List<DoctorEspecialidad>();
}