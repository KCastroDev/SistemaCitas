using SistemaCitas.Models;
using SistemaCitas.Repositories;
using SistemaCitas.Service;

namespace SistemaCitas.Services;

public class IpressService : IIpressService
{
    private static readonly string[] NivelesValidos = { "I", "II", "III" };
    private readonly IIpressRepository _repository;

    public IpressService(IIpressRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Ipress>> ListarAsync() => _repository.GetAllAsync();

    public Task<Ipress?> ObtenerPorIdAsync(int id) => _repository.GetByIdAsync(id);

    public Task<List<UnidadEjecutora>> ListarUnidadesEjecutorasAsync() => _repository.GetUnidadesEjecutorasAsync();

    public Task<List<Distrito>> ListarDistritosAsync() => _repository.GetDistritosAsync();

    public async Task<(bool Exito, string? Error)> CrearAsync(Ipress ipress)
    {
        var error = await ValidarAsync(ipress, esNuevo: true);
        if (error is not null) return (false, error);

        ipress.Activo = true;
        await _repository.AddAsync(ipress);
        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> ActualizarAsync(Ipress ipress)
    {
        var error = await ValidarAsync(ipress, esNuevo: false);
        if (error is not null) return (false, error);

        await _repository.UpdateAsync(ipress);
        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> InhabilitarAsync(int id)
    {
        var ipress = await _repository.GetByIdAsync(id);
        if (ipress is null) return (false, "La IPRESS no existe.");

        ipress.Activo = false;
        await _repository.UpdateAsync(ipress);
        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> HabilitarAsync(int id)
    {
        var ipress = await _repository.GetByIdAsync(id);
        if (ipress is null) return (false, "La IPRESS no existe.");

        ipress.Activo = true;
        await _repository.UpdateAsync(ipress);
        return (true, null);
    }

    // RF-04 (Validación Normativa) aplicada aquí: nivel de atención válido y RENIPRESS único
    private async Task<string?> ValidarAsync(Ipress ipress, bool esNuevo)
    {
        if (!NivelesValidos.Contains(ipress.NivelAtencion))
            return "El nivel de atención debe ser I, II o III.";

        var idExcluir = esNuevo ? (int?)null : ipress.IdIpress;
        if (await _repository.ExisteCodigoRenipressAsync(ipress.CodigoRenipress, idExcluir))
            return $"Ya existe una IPRESS registrada con el código RENIPRESS '{ipress.CodigoRenipress}'.";

        return null;
    }
}