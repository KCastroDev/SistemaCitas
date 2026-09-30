namespace SistemaCitas.Helpers;

// Toda la aplicacion debe leer la fecha y hora desde aqui, nunca con DateTime.Now.
// Motivo: si el servidor esta en otra zona horaria (o en UTC, como en un hosting),
// DateTime.Now devuelve una fecha distinta a la del usuario y la agenda se corre un dia,
// por ejemplo un martes 23:00 en Lima se ve como miercoles 04:00 en UTC.
public static class Reloj
{
    // America/Lima (UTC-5, sin horario de verano). Se prueban los dos identificadores
    // porque Windows y Linux nombran distinto la misma zona.
    private static readonly TimeZoneInfo ZonaPeru = Obtener();

    private static TimeZoneInfo Obtener()
    {
        foreach (var id in new[] { "SA Pacific Standard Time", "America/Lima" })
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }
        return TimeZoneInfo.CreateCustomTimeZone("PET", TimeSpan.FromHours(-5), "Hora de Peru", "PET");
    }

    public static DateTime Ahora => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, ZonaPeru).DateTime;
    public static DateOnly Hoy => DateOnly.FromDateTime(Ahora);
    public static TimeOnly HoraActual => TimeOnly.FromDateTime(Ahora);

    // 1 = lunes ... 7 = domingo, igual que HorarioDoctor.DiaSemana.
    // DayOfWeek de .NET numera Domingo = 0, por eso hay que convertirlo.
    public static int DiaSemana(DateOnly fecha) =>
        fecha.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)fecha.DayOfWeek;

    public static string NombreDia(int diaSemana) => diaSemana switch
    {
        1 => "lunes",
        2 => "martes",
        3 => "miércoles",
        4 => "jueves",
        5 => "viernes",
        6 => "sábado",
        7 => "domingo",
        _ => "día " + diaSemana
    };
}