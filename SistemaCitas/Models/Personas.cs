using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SistemaCitas.Models;

public class Usuario : IdentityUser
{
    [Required, StringLength(120)]
    public string Nombres { get; set; } = null!;

    public int? IdIpress { get; set; }                    // IPRESS donde trabaja (admision/doctor)
    public Ipress? Ipress { get; set; }

    public bool Activo { get; set; } = true;
}

[Table("PACIENTE")]
public class Paciente
{
    [Key]
    public int IdPaciente { get; set; }

    [Required, StringLength(8)]
    public string Dni { get; set; } = null!;              // indice unico

    [Required, StringLength(60)]
    public string Nombres { get; set; } = null!;

    [Required, StringLength(60)]
    public string ApellidoPaterno { get; set; } = null!;

    [Required, StringLength(60)]
    public string ApellidoMaterno { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }         // la edad se calcula

    [Required, StringLength(1)]
    public string Sexo { get; set; } = null!;             // M / F

    [StringLength(15)]
    public string? Telefono { get; set; }

    // RF-01 / RF-08 / RF-11: puntaje de importancia
    [Column(TypeName = "decimal(5,2)")]
    public decimal PorcentajeImportancia { get; set; } = 100m;

    public int IdPlanSeguro { get; set; }
    public PlanSeguro PlanSeguro { get; set; } = null!;

    [StringLength(6)]
    public string IdDistrito { get; set; } = null!;
    public Distrito Distrito { get; set; } = null!;

    public string? UsuarioId { get; set; }                // opcional: solo si usa el portal
    public Usuario? Usuario { get; set; }

    public string? RegistradoPorId { get; set; }          // digitador que lo registro (RF-02)
    public Usuario? RegistradoPor { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;              // baja logica

    public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    public ICollection<HistorialPuntaje> HistorialPuntajes { get; set; } = new List<HistorialPuntaje>();

    [NotMapped]
    public string NombreCompleto => $"{ApellidoPaterno} {ApellidoMaterno}, {Nombres}";
}

[Table("DOCTOR")]
public class Doctor
{
    [Key]
    public int IdDoctor { get; set; }

    [Required, StringLength(10)]
    public string Cmp { get; set; } = null!;              // colegiatura, indice unico

    [Required, StringLength(8)]
    public string Dni { get; set; } = null!;

    [Required, StringLength(60)]
    public string Nombres { get; set; } = null!;

    [Required, StringLength(60)]
    public string ApellidoPaterno { get; set; } = null!;

    [Required, StringLength(60)]
    public string ApellidoMaterno { get; set; } = null!;

    // RF-05: validacion de habilitacion CMP (simulada en esta fase)
    [Required, StringLength(20)]
    public string EstadoHabilitacion { get; set; } = "No verificado";  // Habilitado / No habilitado
    public DateTime? FechaValidacionCmp { get; set; }

    public bool Activo { get; set; } = true;

    public int IdIpress { get; set; }
    public Ipress Ipress { get; set; } = null!;

    public int IdTipoContrato { get; set; }
    public TipoContrato TipoContrato { get; set; } = null!;

    public string? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public ICollection<DoctorEspecialidad> DoctorEspecialidades { get; set; } = new List<DoctorEspecialidad>();
    public ICollection<HorarioDoctor> Horarios { get; set; } = new List<HorarioDoctor>();
    public ICollection<Cita> Citas { get; set; } = new List<Cita>();
}

// N-N entre DOCTOR y ESPECIALIDAD (PK compuesta)
[Table("DOCTOR_ESPECIALIDAD")]
public class DoctorEspecialidad
{
    public int IdDoctor { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public int IdEspecialidad { get; set; }
    public Especialidad Especialidad { get; set; } = null!;

    [StringLength(10)]
    public string? Rne { get; set; }                      // registro de especialista
}

[Table("HORARIO_DOCTOR")]
public class HorarioDoctor
{
    [Key]
    public int IdHorario { get; set; }

    public int IdDoctor { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public int DiaSemana { get; set; }                    // 1 = lunes --- 7 = domingo
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public int CuposPorHora { get; set; } = 4;            // cupos limitados (RF-07)
    public bool Activo { get; set; } = true;
}