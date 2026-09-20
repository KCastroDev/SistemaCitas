using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCitas.Data;
using SistemaCitas.Services;

namespace SistemaCitas.Controllers;

// Endpoints JSON (informe: RNF-03 Interoperabilidad). Sirven para integrar el sistema con otros
// (por ejemplo la futura API del Colegio Medico del Peru). Requieren haber iniciado sesion como
// Administrador o Admision.
//   GET /api/doctores                          -> doctores activos
//   GET /api/doctores/{id}/cupos?fecha=2026-09-21 -> cupos del doctor ese dia
//   GET /api/cmp/{cmp}                         -> habilitacion del CMP (simulada)
[ApiController]
[Route("api")]
[Authorize(Roles = Roles.Administrador + "," + Roles.Admision)]
public class IntegracionController : ControllerBase
{
    private readonly IDoctorService _doctores;
    private readonly ICitaService _citas;
    private readonly ICmpIntegrationService _cmp;

    public IntegracionController(IDoctorService doctores, ICitaService citas, ICmpIntegrationService cmp)
    {
        _doctores = doctores;
        _citas = citas;
        _cmp = cmp;
    }

    [HttpGet("doctores")]
    public async Task<IActionResult> Doctores()
    {
        var lista = await _doctores.ListarAsync(null);

        var respuesta = lista
            .Where(d => d.Activo)
            .Select(d => new
            {
                d.IdDoctor,
                d.Cmp,
                Nombre = $"{d.Nombres} {d.ApellidoPaterno} {d.ApellidoMaterno}",
                d.EstadoHabilitacion,
                Ipress = d.Ipress.Nombre,
                Contrato = d.TipoContrato.Nombre,
                Especialidades = d.DoctorEspecialidades.Select(de => de.Especialidad.Nombre)
            });

        return Ok(respuesta);
    }

    [HttpGet("doctores/{id:int}/cupos")]
    public async Task<IActionResult> Cupos(int id, [FromQuery] DateOnly fecha)
    {
        var cupos = await _citas.ObtenerCuposAsync(id, fecha);

        return Ok(new
        {
            IdDoctor = id,
            Fecha = fecha.ToString("yyyy-MM-dd"),
            Cupos = cupos.Select(c => new { Hora = c.Hora.ToString("HH:mm"), c.Disponible, c.Motivo })
        });
    }

    [HttpGet("cmp/{cmp}")]
    public IActionResult ConsultarCmp(string cmp)
    {
        if (!Regex.IsMatch(cmp, @"^\d{4,10}$"))
            return BadRequest(new { error = "El CMP debe tener entre 4 y 10 dígitos." });

        return Ok(_cmp.Consultar(cmp));
    }
}
