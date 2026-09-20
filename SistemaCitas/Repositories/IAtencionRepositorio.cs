using SistemaCitas.Models;

namespace SistemaCitas.Repositories;

public interface IAtencionRepositorio : IRepositorio<AtencionMedica>
{
    // Historial clinico de un paciente (RF-14): atenciones con cita, doctor, triaje, diagnosticos y receta.
    // Las mas recientes primero.
    Task<IEnumerable<AtencionMedica>> HistorialDePacienteAsync(int idPaciente);
}
