namespace SistemaCitas.Services;

// Resultado sencillo que devuelven los servicios: salio bien o mal, y si mal, por que.
public record ResultadoOperacion(bool Exito, string? Error = null)
{
    public static ResultadoOperacion Ok() => new(true);
    public static ResultadoOperacion Falla(string error) => new(false, error);
}
