namespace SistemaCitas.Data;

// Nombres de los roles del sistema. Se escriben SIN tilde a proposito.
// Uso en cualquier controlador:  [Authorize(Roles = Roles.Administrador)]
public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Admision = "Admision";
    public const string Doctor = "Doctor";
    public const string Paciente = "Paciente";

    // Lista completa: la usa el inicializador para crear los roles en la BD
    public static readonly string[] Todos = { Administrador, Admision, Doctor, Paciente };
}
