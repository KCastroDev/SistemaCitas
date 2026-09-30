using System.Text.RegularExpressions;
using SistemaCitas.Models;

namespace SistemaCitas.Helpers;

// Reglas de formato y armado del usuario de acceso segun el tipo de documento.
// El usuario de Identity es "CODIGO-NUMERO" (ej. "DNI-71000001", "CE-001234567"),
// asi dos personas con el mismo numero pero distinto tipo son cuentas distintas.
public static class Documento
{
    public const int DniPorDefecto = 1;   // IdTipoDocumento del DNI

    // Caracteres que Identity debe permitir en el UserName (letras, digitos y el guion)
    public const string CaracteresPermitidos =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-";

    public static string Normalizar(string? numero) =>
        (numero ?? string.Empty).Trim().ToUpperInvariant();

    // Arma el usuario de acceso
    public static string ArmarUsuario(string codigoTipo, string numero) =>
        $"{codigoTipo.Trim().ToUpperInvariant()}-{Normalizar(numero)}";

    // Devuelve null si el numero es valido para ese tipo, o el mensaje de error
    public static string? Validar(TipoDocumento tipo, string? numero)
    {
        var valor = Normalizar(numero);

        if (string.IsNullOrWhiteSpace(valor))
            return $"Ingrese el número de {tipo.Nombre}.";

        if (tipo.SoloNumeros && !Regex.IsMatch(valor, @"^\d+$"))
            return $"El {tipo.Nombre} solo admite números.";

        if (!tipo.SoloNumeros && !Regex.IsMatch(valor, @"^[A-Z0-9]+$"))
            return $"El {tipo.Nombre} solo admite letras y números, sin espacios ni guiones.";

        if (valor.Length < tipo.LongitudMinima || valor.Length > tipo.LongitudMaxima)
        {
            return tipo.LongitudMinima == tipo.LongitudMaxima
                ? $"El {tipo.Nombre} debe tener {tipo.LongitudMinima} caracteres."
                : $"El {tipo.Nombre} debe tener entre {tipo.LongitudMinima} y {tipo.LongitudMaxima} caracteres.";
        }

        return null;
    }
}