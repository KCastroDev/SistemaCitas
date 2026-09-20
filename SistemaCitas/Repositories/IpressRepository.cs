using Microsoft.EntityFrameworkCore;
using SistemaCitas.Data;
using SistemaCitas.Models;

namespace SistemaCitas.Repositories;

public class IpressRepository : IIpressRepository
{
    private readonly AppDbContext _context;

    public IpressRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ipress>> GetAllAsync()
    {
        return await _context.Ipress
            .Include(i => i.UnidadEjecutora)
            .Include(i => i.Distrito)
            .OrderBy(i => i.Nombre)
            .ToListAsync();
    }

    public async Task<Ipress?> GetByIdAsync(int id)
    {
        return await _context.Ipress
            .Include(i => i.UnidadEjecutora)
            .Include(i => i.Distrito)
            .FirstOrDefaultAsync(i => i.IdIpress == id);
    }

    public async Task<bool> ExisteCodigoRenipressAsync(string codigo, int? idExcluir = null)
    {
        var query = _context.Ipress.Where(i => i.CodigoRenipress == codigo);

        if (idExcluir.HasValue)
        {
            query = query.Where(i => i.IdIpress != idExcluir.Value);
        }

        return await query.AnyAsync();
    }
    public async Task AddAsync(Ipress ipress)
    {
        _context.Ipress.Add(ipress);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Ipress ipress)
    {
        _context.Ipress.Update(ipress);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UnidadEjecutora>> GetUnidadesEjecutorasAsync()
    {
        return await _context.UnidadesEjecutoras.OrderBy(u => u.Nombre).ToListAsync();
    }

    public async Task<List<Distrito>> GetDistritosAsync()
    {
        return await _context.Distritos.OrderBy(d => d.Nombre).ToListAsync();
    }
}