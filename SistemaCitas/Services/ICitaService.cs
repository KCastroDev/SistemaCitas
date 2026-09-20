using SistemaCitas.Models;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Services;

// Un cupo (hora) de la agenda de un doctor. Si no esta disponible, "Motivo" dice por que.
public record CupoDto(TimeOnly Hora, bool Disponible, string? Motivo);

// Reglas de negocio de las citas (RF-07 a RF-11 / CU-06, CU-07, CU-08).
public interface ICitaService
{
    Task<Paciente?> ObtenerPacienteAsync(int idPaciente);
    Task<Paciente?> ObtenerPacientePorUsuarioAsync(string usuarioId);

    // Arma la pantalla de busqueda: especialidades, doctores y cupos segun lo elegido
    Task<BuscarCuposViewModel> ArmarBusquedaAsync(Paciente paciente, int? idEspecialidad, int? idDoctor, DateOnly? fecha, bool presencial);

    // RF-07 + RF-08 (web) / RF-09 (presencial)
    Task<ResultadoOperacion> AgendarAsync(int idPaciente, int idDoctor, DateOnly fecha, TimeOnly hora, string canal, bool dniVerificado = false);

    // Cupos de un doctor en una fecha (los usa el endpoint JSON de integracion, RNF-03)
    Task<List<CupoDto>> ObtenerCuposAsync(int idDoctor, DateOnly fecha);

    Task<IEnumerable<Cita>> ListarCitasPacienteAsync(int idPaciente);
    Task<IEnumerable<Cita>> ListarCitasPorFechaAsync(DateOnly fecha);

    // El paciente cancela su propia cita (no se penaliza)
    Task<ResultadoOperacion> CancelarAsync(int idCita, int idPaciente);

    // RF-10 + RF-11: cambia el estado; si el paciente "Falto", baja su porcentaje de importancia
    Task<ResultadoOperacion> CambiarEstadoAsync(int idCita, EstadoCita nuevoEstado, string usuarioId);
}
