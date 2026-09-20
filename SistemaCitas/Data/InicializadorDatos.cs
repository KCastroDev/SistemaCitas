using Microsoft.AspNetCore.Identity;
using SistemaCitas.Models;

namespace SistemaCitas.Data;

// Se ejecuta UNA VEZ cada vez que arranca la aplicacion (ver Program.cs).
// Es seguro repetirlo: solo crea lo que todavia no existe.
//   1) Crea los 4 roles del sistema.
//   2) Crea un usuario Administrador inicial para poder entrar la primera vez.
public static class InicializadorDatos
{
    // Datos del administrador inicial. Se pueden cambiar en appsettings.json:
    //   "AdminInicial": { "Correo": "...", "Contrasena": "..." }
    // Si no estan ahi, se usan estos valores (documentados en el README).
    private const string CorreoPorDefecto = "admin@sistemacitas.pe";
    private const string ContrasenaPorDefecto = "Admin2026";

    public static async Task InicializarAsync(IServiceProvider servicios, IConfiguration configuracion)
    {
        var roleManager = servicios.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = servicios.GetRequiredService<UserManager<Usuario>>();

        // 1) Roles
        foreach (var rol in Roles.Todos)
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }

        // 2) Administrador inicial
        var correo = configuracion["AdminInicial:Correo"] ?? CorreoPorDefecto;
        var contrasena = configuracion["AdminInicial:Contrasena"] ?? ContrasenaPorDefecto;

        if (await userManager.FindByEmailAsync(correo) == null)
        {
            var admin = new Usuario
            {
                UserName = correo,
                Email = correo,
                EmailConfirmed = true,
                Nombres = "Administrador del sistema",
                Activo = true
            };

            var resultado = await userManager.CreateAsync(admin, contrasena);
            if (resultado.Succeeded)
                await userManager.AddToRoleAsync(admin, Roles.Administrador);
        }
    }
}
