using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCitas.Models;

// RNF-01: todo cambio critico genera un registro de auditoria
[Table("AUDITORIA")]
public class Auditoria
{
    [Key]
    public int IdAuditoria { get; set; }

    public string? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    [Required, StringLength(60)]
    public string Entidad { get; set; } = null!;          // "Cita", "Doctor"

    [Required, StringLength(20)]
    public string Accion { get; set; } = null!;           // Crear / Editar / Eliminar

    [StringLength(60)]
    public string? ClaveRegistro { get; set; }

    [StringLength(1000)] public string? ValorAnterior { get; set; }
    [StringLength(1000)] public string? ValorNuevo { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
}

// Hace auditable el algoritmo de prioridad (restriccion etica del punto 4.2)
[Table("HISTORIAL_PUNTAJE")]
public class HistorialPuntaje
{
    [Key]
    public int IdHistorialPuntaje { get; set; }

    public int IdPaciente { get; set; }
    public Paciente Paciente { get; set; } = null!;

    public int? IdCita { get; set; }
    public Cita? Cita { get; set; }

    [Column(TypeName = "decimal(5,2)")] public decimal PuntajeAnterior { get; set; }
    [Column(TypeName = "decimal(5,2)")] public decimal PuntajeNuevo { get; set; }

    [Required, StringLength(200)]
    public string Motivo { get; set; } = null!;           // "Inasistencia", "Restablecido por admision"

    public string? RegistradoPorId { get; set; }
    public Usuario? RegistradoPor { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
}