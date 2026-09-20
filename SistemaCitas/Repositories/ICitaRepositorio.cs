using SistemaCitas.Models;

namespace SistemaCitas.Repositories;

public interface ICitaRepositorio : IRepositorio<Cita>
{
    // Citas de un paciente con su doctor, especialidad e IPRESS (mas recientes primero)
    Task<IEnumerable<Cita>> ListarPorPacienteAsync(int idPaciente);

    // Todas las citas de un dia (para el panel de Admision)
    Task<IEnumerable<Cita>> ListarPorFechaAsync(DateOnly fecha);

    // Horas del doctor que ya estan tomadas ese dia (las canceladas quedan libres)
    Task<HashSet<TimeOnly>> HorasOcupadasAsync(int idDoctor, DateOnly fecha);

    // La cita de ese cupo exacto, sea cual sea su estado (el indice unico no permite dos)
    Task<Cita?> ObtenerPorCupoAsync(int idDoctor, DateOnly fecha, TimeOnly hora);

    // Una cita con su paciente, doctor e IPRESS (para cambiar su estado)
    Task<Cita?> ObtenerConDetalleAsync(int idCita);
}
