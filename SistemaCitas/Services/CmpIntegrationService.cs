namespace SistemaCitas.Services;

public class CmpIntegrationService : ICmpIntegrationService
{
    // Regla de la simulacion: un CMP que termina en 0 figura como "No habilitado"; cualquier otro, "Habilitado".
    public static bool EstaHabilitado(string cmp) => !cmp.EndsWith('0');

    public CmpRespuesta Consultar(string cmp) =>
        new CmpRespuesta(cmp, EstaHabilitado(cmp) ? "Habilitado" : "No habilitado", true, DateTime.Now);
}
