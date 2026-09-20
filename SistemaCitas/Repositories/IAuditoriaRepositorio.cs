using SistemaCitas.Models;

namespace SistemaCitas.Repositories;

public interface IAuditoriaRepositorio : IRepositorio<Auditoria>
{
    // Los ultimos registros de auditoria (mas recientes primero), con el usuario que hizo el cambio
    Task<IEnumerable<Auditoria>> UltimosAsync(int cantidad);
}
