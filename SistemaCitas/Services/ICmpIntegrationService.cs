namespace SistemaCitas.Services;

// Respuesta en formato JSON de la consulta de habilitacion de un CMP (RNF-03)
public record CmpRespuesta(string Cmp, string Estado, bool Simulado, DateTime FechaConsulta);

// Punto unico para consultar la habilitacion de un medico en el Colegio Medico del Peru (CMP).
// En esta fase es SIMULADO (restriccion RO-03 del informe): cuando exista la API real del CMP,
// solo hay que cambiar el contenido de esta clase; el resto del sistema no se modifica.
public interface ICmpIntegrationService
{
    CmpRespuesta Consultar(string cmp);
}
