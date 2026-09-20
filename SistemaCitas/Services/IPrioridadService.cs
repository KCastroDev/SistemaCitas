namespace SistemaCitas.Services;

// Reglas del "porcentaje de importancia" (RF-08 y RF-11 del informe).
// Es logica pura (no toca la base de datos), por eso es facil de probar.
public interface IPrioridadService
{
    // Cuantos puntos pierde el paciente cada vez que falta a una cita (RF-11)
    decimal PenalizacionPorFalta { get; }

    // Debajo del umbral, el paciente queda bloqueado para agendar por la web (CAR-01)
    bool EstaBloqueado(decimal importancia);

    // Dias minimos de anticipacion con que puede reservar (los cupos mas cercanos son limitados
    // y se reservan a los pacientes con mayor importancia)
    int DiasMinimosAnticipacion(decimal importancia);

    // Devuelve el nuevo porcentaje tras una inasistencia (nunca baja de 0)
    decimal AplicarPenalizacion(decimal importancia);

    // Texto que se muestra al paciente para explicarle su situacion
    string Describir(decimal importancia);
}
