using SistemaCitas.Models;

namespace SistemaCitas.Repositories
{
    public interface IIpressRepository
    {
        Task<List<Ipress>> GetAllAsync();
        Task<Ipress?> GetByIdAsync(int id);
        Task<bool> ExisteCodigoRenipressAsync(string codigo, int? idExcluir = null);
        Task AddAsync(Ipress ipress);
        Task UpdateAsync(Ipress ipress);
        Task<List<UnidadEjecutora>> GetUnidadesEjecutorasAsync();
        Task<List<Distrito>> GetDistritosAsync();
    }
}
