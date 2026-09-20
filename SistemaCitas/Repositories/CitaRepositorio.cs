using Microsoft.EntityFrameworkCore;
using SistemaCitas.Data;
using SistemaCitas.Models;

namespace SistemaCitas.Repositories;

public class CitaRepositorio : Repositorio<Cita>, ICitaRepositorio
{
    public CitaRepositorio(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Cita>> ListarPorPacienteAsync(int idPaciente) =>
        await _dbSet.AsNoTracking()
            .Include(c => c.Ipress)
            .Include(c => c.Doctor).ThenInclude(d => d.DoctorEspecialidades).ThenInclude(de => de.Especialidad)
            .Where(c => c.IdPaciente == idPaciente)
            .OrderByDescending(c => c.FechaCita).ThenByDescending(c => c.HoraCita)
            .ToListAsync();

    public async Task<IEnumerable<Cita>> ListarPorFechaAsync(DateOnly fecha) =>
        await _dbSet.AsNoTracking()
            .Include(c => c.Paciente)
            .Include(c => c.Ipress)
            .Include(c => c.Doctor).ThenInclude(d => d.DoctorEspecialidades).ThenInclude(de => de.Especialidad)
            .Where(c => c.FechaCita == fecha)
            .OrderBy(c => c.HoraCita)
            .ToListAsync();

    public async Task<HashSet<TimeOnly>> HorasOcupadasAsync(int idDoctor, DateOnly fecha)
    {
        var horas = await _dbSet.AsNoTracking()
            .Where(c => c.IdDoctor == idDoctor && c.FechaCita == fecha && c.Estado != EstadoCita.Cancelado)
            .Select(c => c.HoraCita)
            .ToListAsync();
        return horas.ToHashSet();
    }

    public async Task<Cita?> ObtenerPorCupoAsync(int idDoctor, DateOnly fecha, TimeOnly hora) =>
        await _dbSet.FirstOrDefaultAsync(c => c.IdDoctor == idDoctor && c.FechaCita == fecha && c.HoraCita == hora);

    public async Task<Cita?> ObtenerConDetalleAsync(int idCita) =>
        await _dbSet
            .Include(c => c.Paciente)
            .Include(c => c.Doctor)
            .Include(c => c.Ipress)
            .FirstOrDefaultAsync(c => c.IdCita == idCita);
}
