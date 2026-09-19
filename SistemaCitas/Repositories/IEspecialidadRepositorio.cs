using SistemaCitas.Models;

namespace SistemaCitas.Repositories;

// Repositorio especifico: hereda lo generico y agrega consultas propias.
public interface IEspecialidadRepositorio : IRepositorio<Especialidad>
{
    Task<IEnumerable<Especialidad>> ListarAsync(string? buscar);
    Task<IEnumerable<Especialidad>> ListarActivasAsync();
    Task<bool> NombreRepetidoAsync(string nombre, int idExcluir);
    Task<bool> TieneDoctoresAsync(int idEspecialidad);
}