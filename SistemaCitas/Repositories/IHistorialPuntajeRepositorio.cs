using SistemaCitas.Models;

namespace SistemaCitas.Repositories;

public interface IHistorialPuntajeRepositorio : IRepositorio<HistorialPuntaje>
{
    // Los ultimos cambios de puntaje (mas recientes primero), con el paciente y quien los registro
    Task<IEnumerable<HistorialPuntaje>> UltimosAsync(int cantidad);
}
