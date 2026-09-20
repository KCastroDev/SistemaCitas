using SistemaCitas.Models;

namespace SistemaCitas.Services;

// RNF-01: todo cambio critico genera un registro de auditoria (quien, cuando, que cambio).
public interface IAuditoriaService
{
    // Guarda un registro. El usuario se toma de la sesion actual (si la hay).
    Task RegistrarAsync(string entidad, string accion, string? clave, string? valorAnterior, string? valorNuevo);

    Task<IEnumerable<Auditoria>> UltimosAsync(int cantidad);
}
