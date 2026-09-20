using System.Linq.Expressions;

namespace SistemaCitas.Repositories;

// Patron Repository (Semana 4, bloque 7): aisla el acceso a datos
// para que los controladores y servicios no dependan de EF Core.
public interface IRepositorio<T> where T : class
{
    Task<IEnumerable<T>> ObtenerTodosAsync();
    Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> filtro);
    Task<T?> ObtenerPorIdAsync(object id);
    Task<bool> ExisteAsync(Expression<Func<T, bool>> filtro);
    Task AgregarAsync(T entidad);
    void Actualizar(T entidad);
    void Eliminar(T entidad);
    Task<int> GuardarCambiosAsync();
}