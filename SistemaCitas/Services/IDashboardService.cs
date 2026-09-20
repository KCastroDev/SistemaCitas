using SistemaCitas.ViewModels;

namespace SistemaCitas.Services;

// Estadisticas y auditoria para el Administrador (RF-15 / CU-10)
public interface IDashboardService
{
    Task<DashboardViewModel> ObtenerAsync();
}
