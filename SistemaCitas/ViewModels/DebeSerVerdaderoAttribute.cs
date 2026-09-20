using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SistemaCitas.ViewModels;

// Regla para casillas (checkbox) que OBLIGATORIAMENTE deben estar marcadas,
// por ejemplo "Acepto los terminos y condiciones".
// Se valida en el servidor (IsValid) y tambien en el navegador (AddValidation).
// El script del navegador esta en Views/Auth/Register.cshtml.
public class DebeSerVerdaderoAttribute : ValidationAttribute, IClientModelValidator
{
    public override bool IsValid(object? value) => value is bool marcado && marcado;

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val"] = "true";
        context.Attributes["data-val-debeserverdadero"] = ErrorMessage ?? "Debe marcar esta casilla";
    }
}
