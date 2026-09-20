using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SistemaCitas.Data;

namespace SistemaCitas.Repositories;

// Implementacion generica: sirve para cualquier entidad del modelo.
public class Repositorio<T> : IRepositorio<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repositorio(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> ObtenerTodosAsync() =>
        await _dbSet.AsNoTracking().ToListAsync();

    public async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> filtro) =>
        await _dbSet.AsNoTracking().Where(filtro).ToListAsync();

    public async Task<T?> ObtenerPorIdAsync(object id) =>
        await _dbSet.FindAsync(id);

    public async Task<bool> ExisteAsync(Expression<Func<T, bool>> filtro) =>
        await _dbSet.AnyAsync(filtro);

    public async Task AgregarAsync(T entidad) =>
        await _dbSet.AddAsync(entidad);

    public void Actualizar(T entidad) => _dbSet.Update(entidad);

    public void Eliminar(T entidad) => _dbSet.Remove(entidad);

    public async Task<int> GuardarCambiosAsync() =>
        await _context.SaveChangesAsync();
}