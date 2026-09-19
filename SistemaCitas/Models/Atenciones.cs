using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCitas.Models;

public enum EstadoCita { Pendiente = 1, Cancelado = 2, Asistido = 3, Falto = 4 }   // RF-10

[Table("CITA")]
public class Cita
{
    [Key]
    public int IdCita { get; set; }

    public int IdPaciente { get; set; }
    public Paciente Paciente { get; set; } = null!;

    public int IdDoctor { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public int IdIpress { get; set; }
    public Ipress Ipress { get; set; } = null!;

    public DateOnly FechaCita { get; set; }
    public TimeOnly HoraCita { get; set; }
    public DateTime? HoraLlegada { get; set; }            // para el tiempo de espera

    public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;

    [Column(TypeName = "decimal(5,2)")]
    public decimal PuntajeAlAgendar { get; set; }         // puntaje del paciente al reservar

    [Required, StringLength(15)]
    public string Canal { get; set; } = "Web";            // Web / Presencial (RF-09)

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public Triaje? Triaje { get; set; }
    public AtencionMedica? AtencionMedica { get; set; }
}

[Table("TRIAJE")]
public class Triaje
{
    [Key]
    public int IdTriaje { get; set; }

    public int IdCita { get; set; }                       // indice unico -> 1 a 1
    public Cita Cita { get; set; } = null!;

    [Column(TypeName = "decimal(5,2)")] public decimal PesoKg { get; set; }
    [Column(TypeName = "decimal(5,2)")] public decimal TallaCm { get; set; }
    [Column(TypeName = "decimal(4,1)")] public decimal TemperaturaC { get; set; }

    [StringLength(10)]
    public string? PresionArterial { get; set; }          // "120/80"
    public int? FrecuenciaCardiaca { get; set; }
    public int? SaturacionO2 { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}

// MAESTRO del maestro-detalle (RF-13)
[Table("ATENCION_MEDICA")]
public class AtencionMedica
{
    [Key]
    public int IdAtencion { get; set; }

    public int IdCita { get; set; }                       // indice unico -> 1 a 1
    public Cita Cita { get; set; } = null!;

    public DateTime FechaHoraAtencion { get; set; } = DateTime.Now;

    [Required, StringLength(500)]
    public string MotivoConsulta { get; set; } = null!;

    [StringLength(1000)]
    public string? Sintomatologia { get; set; }

    [StringLength(1000)]
    public string? Observaciones { get; set; }

    public ICollection<AtencionDiagnostico> Diagnosticos { get; set; } = new List<AtencionDiagnostico>();
    public ICollection<DetalleReceta> Receta { get; set; } = new List<DetalleReceta>();
}

[Table("CIE10")]
public class Cie10
{
    [Key, StringLength(6)]
    public string CodigoCie10 { get; set; } = null!;      // "J00"

    [Required, StringLength(200)]
    public string Descripcion { get; set; } = null!;
}

// N-N entre ATENCION_MEDICA y CIE10 (PK compuesta)
[Table("ATENCION_DIAGNOSTICO")]
public class AtencionDiagnostico
{
    public int IdAtencion { get; set; }
    public AtencionMedica AtencionMedica { get; set; } = null!;

    [StringLength(6)]
    public string CodigoCie10 { get; set; } = null!;
    public Cie10 Cie10 { get; set; } = null!;

    [Required, StringLength(1)]
    public string TipoDiagnostico { get; set; } = "D";    // P presuntivo, D definitivo, R repetitivo
}

[Table("MEDICAMENTO")]
public class Medicamento
{
    [Key]
    public int IdMedicamento { get; set; }

    [Required, StringLength(120)]
    public string Nombre { get; set; } = null!;

    [StringLength(60)]
    public string? Presentacion { get; set; }             // "Tableta 500 mg"

    public bool Activo { get; set; } = true;
}

// DETALLE del maestro-detalle (RF-13)
[Table("DETALLE_RECETA")]
public class DetalleReceta
{
    [Key]
    public int IdDetalleReceta { get; set; }

    public int IdAtencion { get; set; }
    public AtencionMedica AtencionMedica { get; set; } = null!;

    public int IdMedicamento { get; set; }
    public Medicamento Medicamento { get; set; } = null!;

    public int Cantidad { get; set; }

    [Required, StringLength(200)]
    public string Indicaciones { get; set; } = null!;     // "1 tableta cada 8 horas por 5 dias"
}