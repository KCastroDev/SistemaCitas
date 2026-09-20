using Microsoft.EntityFrameworkCore;
using SistemaCitas.Data;
using SistemaCitas.Models;

namespace SistemaCitas.Repositories;

public class EspecialidadRepositorio : Repositorio<Especialidad>, IEspecialidadRepositorio
{
    public EspecialidadRepositorio(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Especialidad>> ListarAsync(string? buscar)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
            query = query.Where(e => e.Nombre.Contains(buscar));

        return await query.OrderBy(e => e.Nombre).ToListAsync();
    }

    public async Task<IEnumerable<Especialidad>> ListarActivasAsync() =>
        await _dbSet.AsNoTracking()
                    .Where(e => e.Activo)
                    .OrderBy(e => e.Nombre)
                    .ToListAsync();

    public async Task<bool> NombreRepetidoAsync(string nombre, int idExcluir) =>
        await _dbSet.AnyAsync(e => e.Nombre == nombre && e.IdEspecialidad != idExcluir);

    public async Task<bool> TieneDoctoresAsync(int idEspecialidad) =>
        await _context.DoctorEspecialidades.AnyAsync(de => de.IdEspecialidad == idEspecialidad);
}