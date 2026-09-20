using SistemaCitas.Models;

namespace SistemaCitas.ViewModels;

// Datos del panel de estadisticas y auditoria del Administrador (RF-15)
public class DashboardViewModel
{
    // Totales generales
    public int TotalPacientes { get; set; }
    public int TotalDoctores { get; set; }
    public int DoctoresHabilitados { get; set; }
    public int TotalIpress { get; set; }
    public int TotalUsuariosActivos { get; set; }

    // Citas por estado (RF-10)
    public int CitasPendientes { get; set; }
    public int CitasAsistidas { get; set; }
    public int CitasFaltas { get; set; }
    public int CitasCanceladas { get; set; }
    public int CitasHoy { get; set; }
    public int TotalCitas => CitasPendientes + CitasAsistidas + CitasFaltas + CitasCanceladas;

    // Citas por canal (RF-07 web / RF-09 presencial)
    public int CitasWeb { get; set; }
    public int CitasPresenciales { get; set; }

    // Pacientes segun su porcentaje de importancia (RF-08)
    public int PacientesPrioridadAlta { get; set; }
    public int PacientesPrioridadNormal { get; set; }
    public int PacientesBloqueados { get; set; }

    // De las citas ya resueltas (Asistio o Falto), que porcentaje fue inasistencia
    public decimal TasaInasistencia { get; set; }

    // Auditoria: ultimos cambios de puntaje (RF-11)
    public IEnumerable<HistorialPuntaje> Auditoria { get; set; } = new List<HistorialPuntaje>();

    // Auditoria general (RNF-01): cambios criticos en doctores, horarios, citas y atenciones
    public IEnumerable<Auditoria> AuditoriaGeneral { get; set; } = new List<Auditoria>();
}

// Fila de la lista de usuarios activos (RF-15)
public class UsuarioFila
{
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";
    public string Rol { get; set; } = "";
}

public class UsuariosActivosViewModel
{
    public List<UsuarioFila> Usuarios { get; set; } = new();
}
