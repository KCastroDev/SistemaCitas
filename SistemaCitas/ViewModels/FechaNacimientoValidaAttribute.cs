using System.ComponentModel.DataAnnotations;

namespace SistemaCitas.ViewModels;

// Regla para fechas de nacimiento: no puede ser una fecha futura ni de hace mas de 120 anios.
// Se usa asi: [FechaNacimientoValida] encima de la propiedad DateOnly?.
public class FechaNacimientoValidaAttribute : ValidationAttribute
{
    public FechaNacimientoValidaAttribute()
    {
        ErrorMessage = "La fecha de nacimiento no es válida (no puede ser futura ni de hace más de 120 años)";
    }

    public override bool IsValid(object? value)
    {
        if (value is null) return true;   // si es obligatoria, ya la revisa [Required]
        if (value is not DateOnly fecha) return false;

        var hoy = DateOnly.FromDateTime(DateTime.Today);
        return fecha <= hoy && fecha >= hoy.AddYears(-120);
    }
}
