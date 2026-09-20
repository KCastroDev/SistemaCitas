namespace SistemaCitas.Services;

// Reglas de prioridad. Los valores estan en constantes para que sean faciles de cambiar
// y de explicar en la sustentacion:
//   - Todo paciente nuevo inicia en 100% (RF-01).
//   - Cada inasistencia ("Falto") resta 20 puntos (RF-11).
//   - Con menos de 50% queda bloqueado para agendar por la web (CAR-01).
//   - Con 80% o mas tiene prioridad alta: puede reservar cupos desde hoy.
//   - Entre 50% y 79% puede reservar cupos con al menos 3 dias de anticipacion.
public class PrioridadService : IPrioridadService
{
    public const decimal Penalizacion = 20m;
    public const decimal UmbralBloqueo = 50m;
    public const decimal UmbralPrioritario = 80m;
    public const int DiasAnticipacionNormal = 3;

    public decimal PenalizacionPorFalta => Penalizacion;

    public bool EstaBloqueado(decimal importancia) => importancia < UmbralBloqueo;

    public int DiasMinimosAnticipacion(decimal importancia) =>
        importancia >= UmbralPrioritario ? 0 : DiasAnticipacionNormal;

    public decimal AplicarPenalizacion(decimal importancia) =>
        Math.Max(0m, importancia - Penalizacion);

    public string Describir(decimal importancia)
    {
        if (EstaBloqueado(importancia))
            return $"Su importancia es menor a {UmbralBloqueo:0.##}% por inasistencias: no puede agendar citas por la web. " +
                   "Acérquese a Admisión. Para una urgencia, acuda al área de Emergencia del establecimiento.";

        if (importancia >= UmbralPrioritario)
            return "Prioridad alta: puede reservar cualquier cupo disponible, incluso para hoy.";

        return $"Prioridad normal: puede reservar cupos con al menos {DiasAnticipacionNormal} días de anticipación. " +
               $"Los cupos más cercanos se reservan para pacientes con importancia de {UmbralPrioritario:0.##}% o más. " +
               "Asista a sus citas para recuperar prioridad.";
    }
}
