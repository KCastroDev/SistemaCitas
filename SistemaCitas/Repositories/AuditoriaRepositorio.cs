using Microsoft.EntityFrameworkCore;
using SistemaCitas.Data;
using SistemaCitas.Models;

namespace SistemaCitas.Repositories;

public class AuditoriaRepositorio : Repositorio<Auditoria>, IAuditoriaRepositorio
{
    public AuditoriaRepositorio(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Auditoria>> UltimosAsync(int cantidad) =>
        await _dbSet.AsNoTracking()
            .Include(a => a.Usuario)
            .OrderByDescending(a => a.Fecha)
            .Take(cantidad)
            .ToListAsync();
}
