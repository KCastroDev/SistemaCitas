using System.ComponentModel.DataAnnotations;

namespace SistemaCitas.ViewModels;

// Regla para fechas de nacimiento:
//   - no puede ser futura ni del dia de hoy
//   - no puede ser de hace mas de 120 anios
//   - opcionalmente exige una edad minima (EdadMinima = 18 para crear cuenta web)
// Se usa asi: [FechaNacimientoValida] o [FechaNacimientoValida(EdadMinima = 18)]
public class FechaNacimientoValidaAttribute : ValidationAttribute
{
    public int EdadMinima { get; set; } = 0;
    public int EdadMaxima { get; set; } = 120;

    public FechaNacimientoValidaAttribute()
    {
        ErrorMessage = "La fecha de nacimiento no es válida (no puede ser futura ni de hace más de 120 años)";
    }

    public override bool IsValid(object? value)
    {
        if (value is null) return true;   // si es obligatoria, ya la revisa [Required]
        if (value is not DateOnly fecha) return false;

        var hoy = DateOnly.FromDateTime(DateTime.Today);

        // La fecha debe ser estrictamente anterior a hoy
        if (fecha >= hoy) return false;

        // Edad cumplida a la fecha de hoy
        var edad = hoy.Year - fecha.Year;
        if (fecha > hoy.AddYears(-edad)) edad--;

        return edad >= EdadMinima && edad <= EdadMaxima;
    }
}