using SistemaCitas.Models;

namespace SistemaCitas.Service
{
    public interface IIpressService
    {
        Task<List<Ipress>> ListarAsync();
        Task<Ipress?> ObtenerPorIdAsync(int id);
        Task<(bool Exito, string? Error)> CrearAsync(Ipress ipress);
        Task<(bool Exito, string? Error)> ActualizarAsync(Ipress ipress);
        Task<(bool Exito, string? Error)> InhabilitarAsync(int id);
        Task<(bool Exito, string? Error)> HabilitarAsync(int id);
        Task<List<UnidadEjecutora>> ListarUnidadesEjecutorasAsync();
        Task<List<Distrito>> ListarDistritosAsync();
    }
}
