using Microsoft.EntityFrameworkCore;
using SistemaCitas.Data;
using SistemaCitas.Models;

namespace SistemaCitas.Repositories;

public class HistorialPuntajeRepositorio : Repositorio<HistorialPuntaje>, IHistorialPuntajeRepositorio
{
    public HistorialPuntajeRepositorio(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<HistorialPuntaje>> UltimosAsync(int cantidad) =>
        await _dbSet.AsNoTracking()
            .Include(h => h.Paciente)
            .Include(h => h.RegistradoPor)
            .OrderByDescending(h => h.Fecha)
            .Take(cantidad)
            .ToListAsync();
}
