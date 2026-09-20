using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaCitas.Data;
using SistemaCitas.Models;
using SistemaCitas.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));
builder.Services.AddScoped<IEspecialidadRepositorio, EspecialidadRepositorio>();

builder.Services.AddDefaultIdentity<Usuario>(o =>
{
    o.Password.RequiredLength = 8;
    o.Password.RequireNonAlphanumeric = false;
    o.Lockout.MaxFailedAccessAttempts = 5;
    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    o.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Cookie de sesion: a donde mandar al usuario si no ha iniciado sesion o no tiene permiso
builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Auth/Login";
    o.AccessDeniedPath = "/Auth/AccessDenied";
    o.ExpireTimeSpan = TimeSpan.FromHours(8);
    o.SlidingExpiration = true;
});

var app = builder.Build();

// Crea los roles y el administrador inicial si todavia no existen.
// Si la base de datos aun no esta lista (falta Update-Database), la app igual arranca
// y solo muestra una advertencia en la consola.
using (var scope = app.Services.CreateScope())
{
    try
    {
        await SistemaCitas.Data.InicializadorDatos.InicializarAsync(scope.ServiceProvider, app.Configuration);
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "No se pudieron crear los roles/administrador iniciales. Verifique la cadena de conexion y ejecute Update-Database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();   // se usa  siempre antes de UseAuthorization
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();       //  se usa para las vistas de login de Identity

app.Run();