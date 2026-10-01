using SistemaCitas.Models;
using SistemaCitas.Repositories;
using SistemaCitas.Service;

namespace SistemaCitas.Services;

public class IpressService : IIpressService
{
    private static readonly string[] NivelesValidos = { "I", "II", "III" };
    private readonly IIpressRepository _repository;
    private readonly IAuditoriaService _auditoria;

    public IpressService(IIpressRepository repository, IAuditoriaService auditoria)
    {
        _repository = repository;
        _auditoria = auditoria;
    }

    // Texto corto para la bitacora: codigo, nombre y nivel de la IPRESS
    private static string Describir(Ipress i) => $"{i.CodigoRenipress} - {i.Nombre} - Nivel {i.NivelAtencion}";

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

        await _auditoria.RegistrarAsync("Ipress", "Crear", ipress.CodigoRenipress, null, Describir(ipress));

        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> ActualizarAsync(Ipress ipress)
    {
        var error = await ValidarAsync(ipress, esNuevo: false);
        if (error is not null) return (false, error);

        // Traemos el registro tal como esta en la base (ya rastreado por EF) para poder
        // copiarle los valores nuevos encima: asi evitamos tener dos instancias con la
        // misma clave primaria rastreadas a la vez (eso hace fallar a Update), y de paso
        // nos queda el "antes" para la auditoria (RNF-01).
        var anterior = await _repository.GetByIdAsync(ipress.IdIpress);
        if (anterior is null) return (false, "La IPRESS no existe.");

        var descripcionAnterior = Describir(anterior);

        anterior.CodigoRenipress = ipress.CodigoRenipress;
        anterior.Nombre = ipress.Nombre;
        anterior.NivelAtencion = ipress.NivelAtencion;
        anterior.Direccion = ipress.Direccion;
        anterior.IdUnidadEjecutora = ipress.IdUnidadEjecutora;
        anterior.IdDistrito = ipress.IdDistrito;
        anterior.Activo = ipress.Activo;

        await _repository.UpdateAsync(anterior);

        await _auditoria.RegistrarAsync("Ipress", "Editar", anterior.CodigoRenipress, descripcionAnterior, Describir(anterior));

        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> InhabilitarAsync(int id)
    {
        var ipress = await _repository.GetByIdAsync(id);
        if (ipress is null) return (false, "La IPRESS no existe.");

        ipress.Activo = false;
        await _repository.UpdateAsync(ipress);

        await _auditoria.RegistrarAsync("Ipress", "Editar", ipress.CodigoRenipress, "Activo", "Inhabilitada");

        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> HabilitarAsync(int id)
    {
        var ipress = await _repository.GetByIdAsync(id);
        if (ipress is null) return (false, "La IPRESS no existe.");

        ipress.Activo = true;
        await _repository.UpdateAsync(ipress);

        await _auditoria.RegistrarAsync("Ipress", "Editar", ipress.CodigoRenipress, "Inhabilitada", "Activo");

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