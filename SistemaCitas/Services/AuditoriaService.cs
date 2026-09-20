using System.Security.Claims;
using SistemaCitas.Models;
using SistemaCitas.Repositories;

namespace SistemaCitas.Services;

public class AuditoriaService : IAuditoriaService
{
    private readonly IAuditoriaRepositorio _auditorias;
    private readonly IHttpContextAccessor _http;

    public AuditoriaService(IAuditoriaRepositorio auditorias, IHttpContextAccessor http)
    {
        _auditorias = auditorias;
        _http = http;
    }

    public async Task RegistrarAsync(string entidad, string accion, string? clave, string? valorAnterior, string? valorNuevo)
    {
        try
        {
            var usuarioId = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _auditorias.AgregarAsync(new Auditoria
            {
                UsuarioId = usuarioId,
                Entidad = entidad,
                Accion = accion,
                ClaveRegistro = Cortar(clave, 60),
                ValorAnterior = Cortar(valorAnterior, 1000),
                ValorNuevo = Cortar(valorNuevo, 1000),
                Fecha = DateTime.Now
            });
            await _auditorias.GuardarCambiosAsync();
        }
        catch
        {
            // La auditoria nunca debe impedir que la operacion principal (ya guardada) termine bien.
        }
    }

    public Task<IEnumerable<Auditoria>> UltimosAsync(int cantidad) => _auditorias.UltimosAsync(cantidad);

    private static string? Cortar(string? texto, int maximo) =>
        texto == null ? null : (texto.Length <= maximo ? texto : texto.Substring(0, maximo));
}
