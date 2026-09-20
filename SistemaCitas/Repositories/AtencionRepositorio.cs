using Microsoft.EntityFrameworkCore;
using SistemaCitas.Data;
using SistemaCitas.Models;

namespace SistemaCitas.Repositories;

public class AtencionRepositorio : Repositorio<AtencionMedica>, IAtencionRepositorio
{
    public AtencionRepositorio(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<AtencionMedica>> HistorialDePacienteAsync(int idPaciente) =>
        await _dbSet.AsNoTracking()
            .Include(a => a.Cita).ThenInclude(c => c.Doctor)
            .Include(a => a.Cita).ThenInclude(c => c.Triaje)
            .Include(a => a.Cita).ThenInclude(c => c.Ipress)
            .Include(a => a.Diagnosticos).ThenInclude(d => d.Cie10)
            .Include(a => a.Receta).ThenInclude(r => r.Medicamento)
            .Where(a => a.Cita.IdPaciente == idPaciente)
            .OrderByDescending(a => a.FechaHoraAtencion)
            .ToListAsync();
}
