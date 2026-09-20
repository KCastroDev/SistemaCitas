using SistemaCitas.Models;

namespace SistemaCitas.Services;

// Capa de servicio (informe: AdmisionController + PacienteService).
// Aqui viven las REGLAS DE NEGOCIO; el controlador solo recibe y muestra.
public interface IPacienteService
{
    // Busca por DNI, nombres o apellidos. Sin texto devuelve los primeros pacientes.
    Task<IEnumerable<Paciente>> BuscarAsync(string? texto);

    // CU-01 / RF-02: registro presencial hecho por el personal de Admision.
    Task<ResultadoOperacion> RegistrarPresencialAsync(Paciente paciente, string registradoPorId);
}
