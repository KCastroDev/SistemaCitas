using SistemaCitas.Models;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Services;

// Reglas de negocio de doctores y horarios (RF-04, RF-05, RF-06 / CU-04, CU-05).
public interface IDoctorService
{
    Task<IEnumerable<Doctor>> ListarAsync(string? buscar);
    Task<Doctor?> ObtenerAsync(int id);

    // Llena las listas desplegables del formulario (IPRESS, contratos, especialidades)
    Task CargarListasAsync(DoctorViewModel vm);

    Task<ResultadoOperacion> RegistrarAsync(DoctorViewModel vm);
    Task<ResultadoOperacion> ValidarCmpAsync(int idDoctor);
    Task<ResultadoOperacion> CambiarEstadoAsync(int idDoctor);

    Task<ResultadoOperacion> AgregarHorarioAsync(int idDoctor, HorarioViewModel vm);
    Task<ResultadoOperacion> QuitarHorarioAsync(int idHorario);
}
