using Microsoft.EntityFrameworkCore;
using SistemaCitas.Data;
using SistemaCitas.Models;

namespace SistemaCitas.Repositories;

public class DoctorRepositorio : Repositorio<Doctor>, IDoctorRepositorio
{
    public DoctorRepositorio(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Doctor>> ListarAsync(string? buscar)
    {
        var query = _dbSet.AsNoTracking()
            .Include(d => d.Ipress)
            .Include(d => d.TipoContrato)
            .Include(d => d.DoctorEspecialidades).ThenInclude(de => de.Especialidad)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var t = buscar.Trim();
            query = query.Where(d =>
                d.Dni.Contains(t) ||
                d.Cmp.Contains(t) ||
                d.Nombres.Contains(t) ||
                d.ApellidoPaterno.Contains(t) ||
                d.ApellidoMaterno.Contains(t));
        }

        return await query
            .OrderBy(d => d.ApellidoPaterno)
            .ThenBy(d => d.ApellidoMaterno)
            .ThenBy(d => d.Nombres)
            .ToListAsync();
    }

    public async Task<Doctor?> ObtenerConDetalleAsync(int id) =>
        await _dbSet
            .Include(d => d.Ipress)
            .Include(d => d.TipoContrato)
            .Include(d => d.DoctorEspecialidades).ThenInclude(de => de.Especialidad)
            .Include(d => d.Horarios)
            .FirstOrDefaultAsync(d => d.IdDoctor == id);
}
