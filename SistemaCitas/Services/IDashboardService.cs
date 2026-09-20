using SistemaCitas.ViewModels;

namespace SistemaCitas.Services;

// Estadisticas y auditoria para el Administrador (RF-15 / CU-10)
public interface IDashboardService
{
    Task<DashboardViewModel> ObtenerAsync();

    // RF-15: lista de usuarios activos con su rol
    Task<UsuariosActivosViewModel> ListarUsuariosActivosAsync();
}
