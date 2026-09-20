using SistemaCitas.Models;

namespace SistemaCitas.Repositories;

// Repositorio especifico de doctores: hereda lo generico y agrega consultas con sus datos relacionados.
public interface IDoctorRepositorio : IRepositorio<Doctor>
{
    // Lista doctores con su IPRESS, contrato y especialidad. "buscar" filtra por DNI, CMP o nombre.
    Task<IEnumerable<Doctor>> ListarAsync(string? buscar);

    // Un doctor con todo lo necesario (IPRESS, contrato, especialidad y horarios).
    Task<Doctor?> ObtenerConDetalleAsync(int id);
}
