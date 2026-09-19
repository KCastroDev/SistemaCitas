using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaCitas.Models;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Controllers;

public class AuthController : Controller
{
    private readonly SignInManager<Usuario> _signInManager;
    private readonly UserManager<Usuario> _userManager;

    public AuthController(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // Muestra la pantalla de login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // Recibe el formulario de login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var usuario = await _userManager.FindByEmailAsync(modelo.Correo);
        if (usuario == null || !usuario.Activo)
        {
            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos");
            return View(modelo);
        }

        var resultado = await _signInManager.PasswordSignInAsync(
            usuario, modelo.Contrasena, modelo.Recordarme, lockoutOnFailure: true);

        if (resultado.Succeeded)
            return RedirectToAction("Index", "Home");

        if (resultado.IsLockedOut)
            ModelState.AddModelError(string.Empty, "Cuenta bloqueada por 15 minutos por demasiados intentos fallidos");
        else
            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos");

        return View(modelo);
    }

    // Cierra la sesión
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}