using SistemaCitas.Models;

namespace SistemaCitas.ViewModels;

// Datos de la pantalla de confirmacion al inhabilitar un establecimiento:
// el establecimiento y las citas que quedarian por reprogramar.
public class InhabilitarIpressViewModel
{
    public Ipress Ipress { get; set; } = null!;

    public List<Cita> CitasPendientes { get; set; } = new();

    public int Total => CitasPendientes.Count;

    public DateOnly? PrimeraFecha => CitasPendientes.FirstOrDefault()?.FechaCita;

    public DateOnly? UltimaFecha => CitasPendientes.LastOrDefault()?.FechaCita;
}