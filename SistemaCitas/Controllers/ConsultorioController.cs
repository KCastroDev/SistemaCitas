using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaCitas.Data;
using SistemaCitas.Models;
using SistemaCitas.Services;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Controllers;

// Consultorio del doctor (informe: CU-09, RF-12, RF-13, RF-14). Solo entra el rol Doctor.
// El doctor se toma SIEMPRE de la sesion: solo ve y atiende las citas de su propia agenda.
[Authorize(Roles = Roles.Doctor)]
public class ConsultorioController : Controller
{
    private readonly IConsultorioService _consultorio;
    private readonly UserManager<Usuario> _userManager;

    public ConsultorioController(IConsultorioService consultorio, UserManager<Usuario> userManager)
    {
        _consultorio = consultorio;
        _userManager = userManager;
    }

    // RF-12: mi agenda del dia
    [HttpGet]
    public async Task<IActionResult> Index(DateOnly? fecha)
    {
        var doctor = await ObtenerDoctorActualAsync();
        if (doctor == null) return SinFicha();

        var dia = fecha ?? DateOnly.FromDateTime(DateTime.Now);
        var modelo = await _consultorio.ArmarAgendaAsync(doctor, dia);
        return View(modelo);
    }

    // RF-13: formulario de atencion
    [HttpGet]
    public async Task<IActionResult> Atender(int id)
    {
        var doctor = await ObtenerDoctorActualAsync();
        if (doctor == null) return SinFicha();

        var (modelo, error) = await _consultorio.PrepararAtencionAsync(doctor.IdDoctor, id);
        if (modelo == null)
        {
            TempData["Error"] = error;
            return RedirectToAction(nameof(Index));
        }

        return View(modelo);
    }

    // RF-13: guarda la atencion
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Atender(AtencionViewModel modelo)
    {
        var doctor = await ObtenerDoctorActualAsync();
        if (doctor == null) return SinFicha();

        if (ModelState.IsValid)
        {
            var resultado = await _consultorio.RegistrarAtencionAsync(doctor.IdDoctor, modelo);
            if (resultado.Exito)
            {
                TempData["Mensaje"] = "Atención registrada correctamente. La cita quedó como Asistido.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, resultado.Error!);
        }

        await _consultorio.CompletarAsync(modelo);
        return View(modelo);
    }

    // RF-14: historial clinico del paciente
    [HttpGet]
    public async Task<IActionResult> Historial(int idPaciente)
    {
        var doctor = await ObtenerDoctorActualAsync();
        if (doctor == null) return SinFicha();

        var modelo = await _consultorio.ObtenerHistorialAsync(doctor.IdDoctor, idPaciente);
        if (modelo == null)
        {
            TempData["Error"] = "Solo puede consultar el historial de pacientes que tienen o tuvieron una cita con usted.";
            return RedirectToAction(nameof(Index));
        }

        return View(modelo);
    }

    private async Task<Doctor?> ObtenerDoctorActualAsync()
    {
        var usuarioId = _userManager.GetUserId(User);
        return usuarioId == null ? null : await _consultorio.ObtenerDoctorPorUsuarioAsync(usuarioId);
    }

    private IActionResult SinFicha()
    {
        TempData["Error"] = "Su cuenta no tiene una ficha de doctor activa. Comuníquese con el Administrador.";
        return RedirectToAction("Index", "Home");
    }
}
