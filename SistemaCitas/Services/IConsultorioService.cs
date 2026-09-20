using SistemaCitas.Models;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Services;

// Reglas del consultorio del doctor (RF-12, RF-13, RF-14 / CU-09)
public interface IConsultorioService
{
    // El doctor ligado a la cuenta con la que se inicio sesion
    Task<Doctor?> ObtenerDoctorPorUsuarioAsync(string usuarioId);

    // RF-12: citas del dia que le corresponden al doctor (por atender y ya atendidas)
    Task<AgendaViewModel> ArmarAgendaAsync(Doctor doctor, DateOnly fecha);

    // Prepara el formulario de atencion de una cita; si no corresponde atenderla devuelve el motivo
    Task<(AtencionViewModel? Modelo, string? Error)> PrepararAtencionAsync(int idDoctor, int idCita);

    // Vuelve a llenar los datos de pantalla (cita, paciente, listas) cuando el formulario tiene errores
    Task CompletarAsync(AtencionViewModel modelo);

    // RF-13: guarda triaje, atencion, diagnostico y receta, y marca la cita como Asistido
    Task<ResultadoOperacion> RegistrarAtencionAsync(int idDoctor, AtencionViewModel modelo);

    // RF-14: historial clinico. Solo de pacientes que tienen o tuvieron alguna cita con este doctor.
    Task<HistorialPacienteViewModel?> ObtenerHistorialAsync(int idDoctor, int idPaciente);
}
