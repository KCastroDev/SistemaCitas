using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaCitas.Helpers;
using SistemaCitas.Data;
using SistemaCitas.Models;
using SistemaCitas.Repositories;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Controllers;

public class AuthController : Controller
{
    private readonly SignInManager<Usuario> _signInManager;
    private readonly UserManager<Usuario> _userManager;

    // Repositorios (patron de Kevin) para leer los catalogos y guardar al paciente
    private readonly IRepositorio<TipoDocumento> _tiposDocumento;
    private readonly IRepositorio<PlanSeguro> _planes;
    private readonly IRepositorio<Distrito> _distritos;
    private readonly IRepositorio<Paciente> _pacientes;

    public AuthController(
        SignInManager<Usuario> signInManager,
        UserManager<Usuario> userManager,
        IRepositorio<TipoDocumento> tiposDocumento,
        IRepositorio<PlanSeguro> planes,
        IRepositorio<Distrito> distritos,
        IRepositorio<Paciente> pacientes)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _tiposDocumento = tiposDocumento;
        _planes = planes;
        _distritos = distritos;
        _pacientes = pacientes;
    }

    // Muestra la pantalla de login
    [HttpGet]
    public async Task<IActionResult> Login()
    {
        // Si viene del registro, precargamos el correo para que solo escriba su contrasena
        var modelo = new LoginViewModel();

        // Si viene del registro, precargamos el documento para que solo escriba su contrasena
        if (TempData["DocumentoRegistrado"] is string documento)
            modelo.Documento = documento;
        if (TempData["TipoDocumentoRegistrado"] is int idTipo)
            modelo.IdTipoDocumento = idTipo;

        await CargarTiposAsync(modelo);
        return View(modelo);
    }

    // Recibe el formulario de login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            await CargarTiposAsync(modelo);
            return View(modelo);
        }

        var tipo = await _tiposDocumento.ObtenerPorIdAsync(modelo.IdTipoDocumento);
        if (tipo == null)
        {
            ModelState.AddModelError(string.Empty, "El tipo de documento no es válido");
            await CargarTiposAsync(modelo);
            return View(modelo);
        }

        // El usuario de Identity es CODIGO-NUMERO (ej. DNI-71000001)
        var usuario = await _userManager.FindByNameAsync(
            Documento.ArmarUsuario(tipo.Codigo, modelo.Documento));
        if (usuario == null || !usuario.Activo)
        {
            ModelState.AddModelError(string.Empty, "Documento o contraseña incorrectos");
            await CargarTiposAsync(modelo);
            return View(modelo);
        }

        var resultado = await _signInManager.PasswordSignInAsync(
            usuario, modelo.Contrasena, modelo.Recordarme, lockoutOnFailure: true);

        if (resultado.Succeeded)
            return RedirectToAction("Index", "Home");

        if (resultado.IsLockedOut)
            ModelState.AddModelError(string.Empty, "Cuenta bloqueada por 15 minutos por demasiados intentos fallidos");
        else
            ModelState.AddModelError(string.Empty, "Documento o contraseña incorrectos");

        await CargarTiposAsync(modelo);
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

    // Pantalla que se muestra cuando el usuario no tiene el rol necesario
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    // ------------------------------------------------------------------
    // REGISTRO DE PACIENTES (CU-02 / RF-01)
    // ------------------------------------------------------------------

    // Muestra el formulario vacio, con los menus llenos desde la BD
    [HttpGet]
    public async Task<IActionResult> Register()
    {
        var modelo = new RegisterViewModel();
        await CargarListasAsync(modelo);
        return View(modelo);
    }

    // Recibe el formulario de registro
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel modelo)
    {
        // Si algo del formulario esta mal, volvemos a mostrarlo (sin perder los menus)
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(modelo);
            return View(modelo);
        }

        // 0) El formato del numero depende del tipo elegido (DNI, CE, pasaporte, CNV)
        var tipo = await _tiposDocumento.ObtenerPorIdAsync(modelo.IdTipoDocumento);
        if (tipo == null || !tipo.Activo)
        {
            ModelState.AddModelError(nameof(modelo.IdTipoDocumento), "El tipo de documento no es válido");
            await CargarListasAsync(modelo);
            return View(modelo);
        }

        var errorDocumento = Documento.Validar(tipo, modelo.Dni);
        if (errorDocumento != null)
        {
            ModelState.AddModelError(nameof(modelo.Dni), errorDocumento);
            await CargarListasAsync(modelo);
            return View(modelo);
        }

        modelo.Dni = Documento.Normalizar(modelo.Dni);

        // 1) Buscamos si ya existe un paciente con ese documento.
        //    Puede existir si el personal de Admision lo registro antes (RF-02) y aun no tiene cuenta.
        var existentes = await _pacientes.BuscarAsync(
            p => p.Dni == modelo.Dni && p.IdTipoDocumento == modelo.IdTipoDocumento);
        var paciente = existentes.FirstOrDefault();

        if (paciente != null && paciente.UsuarioId != null)
        {
            ModelState.AddModelError(nameof(modelo.Dni), $"Ya existe una cuenta registrada con este {tipo.Nombre}");
            await CargarListasAsync(modelo);
            return View(modelo);
        }

        // 2) Creamos la cuenta de usuario (tabla de Identity). La contrasena se guarda cifrada (hash).
        var usuario = new Usuario
        {
            UserName = Documento.ArmarUsuario(tipo.Codigo, modelo.Dni),   // ej. "DNI-71000001"
            Email = modelo.Correo,      // opcional: solo para recuperar la contrasena
            PhoneNumber = modelo.Telefono,
            Nombres = $"{modelo.Nombres} {modelo.ApellidoPaterno} {modelo.ApellidoMaterno}".Trim(),
            Activo = true
        };

        var resultado = await _userManager.CreateAsync(usuario, modelo.Contrasena);
        if (!resultado.Succeeded)
        {
            // Errores de Identity: correo repetido, contrasena debil, etc. (traducidos abajo)
            foreach (var error in resultado.Errors)
                ModelState.AddModelError(string.Empty, TraducirError(error));

            await CargarListasAsync(modelo);
            return View(modelo);
        }

        // 3) Le damos el rol Paciente
        await _userManager.AddToRoleAsync(usuario, Roles.Paciente);

        // 4) Guardamos (o completamos) los datos del paciente y lo enlazamos con su cuenta
        try
        {
            if (paciente == null)
            {
                paciente = new Paciente
                {
                    IdTipoDocumento = modelo.IdTipoDocumento,
                    Dni = modelo.Dni,
                    PorcentajeImportancia = 100m   // todo paciente nuevo empieza con 100% de importancia
                };
                await _pacientes.AgregarAsync(paciente);
            }
            else
            {
                _pacientes.Actualizar(paciente);
            }

            paciente.Nombres = modelo.Nombres;
            paciente.ApellidoPaterno = modelo.ApellidoPaterno;
            paciente.ApellidoMaterno = modelo.ApellidoMaterno;
            paciente.FechaNacimiento = modelo.FechaNacimiento!.Value;
            paciente.Sexo = modelo.Sexo;
            paciente.Telefono = modelo.Telefono;
            paciente.IdPlanSeguro = modelo.IdPlanSeguro!.Value;
            paciente.IdDistrito = modelo.IdDistrito!;
            paciente.UsuarioId = usuario.Id;

            await _pacientes.GuardarCambiosAsync();
        }
        catch (Exception)
        {
            // Si falla al guardar al paciente, borramos la cuenta para no dejar usuarios "huerfanos"
            await _userManager.DeleteAsync(usuario);
            ModelState.AddModelError(string.Empty, "No se pudo completar el registro. Intente nuevamente.");
            await CargarListasAsync(modelo);
            return View(modelo);
        }

        // 5) NO se inicia sesion automaticamente: el paciente debe autenticarse
        //    manualmente con las credenciales que acaba de crear.
        TempData["RegistroExitoso"] =
            "Su cuenta fue creada correctamente. Ingrese con su DNI y contraseña.";
        TempData["DocumentoRegistrado"] = modelo.Dni;
        TempData["TipoDocumentoRegistrado"] = modelo.IdTipoDocumento;

        return RedirectToAction(nameof(Login));
    }

    // Llena los menus desplegables de Plan de seguro y Distrito desde la BD
    // Llena el menu de tipos de documento de la pantalla de login
    private async Task CargarTiposAsync(LoginViewModel modelo)
    {
        var tipos = await _tiposDocumento.BuscarAsync(t => t.Activo);

        modelo.TiposDocumento = tipos
            .OrderBy(t => t.IdTipoDocumento)
            .Select(t => new SelectListItem(t.Nombre, t.IdTipoDocumento.ToString()));
    }

    // Llena los menus de Tipo de documento, Plan de seguro y Distrito desde la BD
    private async Task CargarListasAsync(RegisterViewModel modelo)
    {
        var planes = await _planes.ObtenerTodosAsync();
        var distritos = await _distritos.ObtenerTodosAsync();
        var tipos = await _tiposDocumento.BuscarAsync(t => t.Activo);

        modelo.TiposDocumento = tipos
            .OrderBy(t => t.IdTipoDocumento)
            .Select(t => new SelectListItem(t.Nombre, t.IdTipoDocumento.ToString()));

        modelo.PlanesSeguro = planes
            .OrderBy(p => p.Nombre)
            .Select(p => new SelectListItem(p.Nombre, p.IdPlanSeguro.ToString()));

        modelo.Distritos = distritos
            .OrderBy(d => d.Nombre)
            .Select(d => new SelectListItem(d.Nombre, d.IdDistrito));
    }

    // Identity devuelve sus mensajes en ingles; aqui traducimos los mas comunes
    private static string TraducirError(IdentityError error) => error.Code switch
    {
        "DuplicateUserName" => "Ya existe una cuenta registrada con ese documento",
        "InvalidUserName" => "El número de documento tiene caracteres no permitidos",
        "PasswordRequiresUpper" => "La contraseña debe tener al menos una letra mayúscula",
        "PasswordRequiresLower" => "La contraseña debe tener al menos una letra minúscula",
        "PasswordRequiresDigit" => "La contraseña debe tener al menos un número",
        "PasswordTooShort" => "La contraseña debe tener mínimo 8 caracteres",
        "InvalidEmail" => "El correo no tiene un formato válido",
        _ => error.Description
    };
}