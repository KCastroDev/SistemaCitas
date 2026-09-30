# SistemaCitas — Gestión de Citas Médicas IPRESS

Sistema web para gestionar citas médicas en las IPRESS de La Libertad. Prioriza la
asignación de cupos según el **porcentaje de importancia** del paciente (que baja cuando
falta a sus citas) y controla los horarios de los doctores según su **tipo de contrato**.

Proyecto final del curso Diseño y Arquitectura de Software (UPN, Ingeniería de Sistemas).

## Tecnologías

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core 10 (Code First con migraciones)
- SQL Server (LocalDB o SQL Server Express)
- ASP.NET Core Identity (autenticación, roles y contraseñas cifradas)
- Bootstrap 5 y Bootstrap Icons

## Requisitos previos

- Visual Studio 2022 **17.16 o superior** (las versiones anteriores no admiten .NET 10)
- SDK de .NET 10
- SQL Server LocalDB (viene con Visual Studio) o SQL Server Express
- Git

## Cómo ejecutar el proyecto en local

### 1. Clonar el repositorio

```bash
git clone https://github.com/KCastroDev/SistemaCitas.git
```

Abrir `SistemaCitas.slnx` con Visual Studio.

### 2. Configurar la conexión a la base de datos

El archivo `appsettings.json` trae un texto de relleno como cadena de conexión. **No lo
modifique ni lo suba al repositorio.** Cada integrante agrega su propia conexión en
`SistemaCitas/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SistemaCitasDB;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

Si usa SQL Server Express, cambie `(localdb)\\MSSQLLocalDB` por `.\\SQLEXPRESS`. Este
cambio es solo para su computadora: no lo incluya en los commits.

### 3. Crear la base de datos

En Visual Studio: *Herramientas → Administrador de paquetes NuGet → Consola del
Administrador de paquetes*, con el proyecto predeterminado `SistemaCitas`:

```powershell
Update-Database
```

Esto crea `SistemaCitasDB` con todas sus tablas y los datos iniciales (tipos de documento,
distritos, planes de seguro, tipos de contrato, especialidades y códigos CIE-10).

> **Si ya tenía la base creada antes del cambio de autenticación**, ejecute primero
> `Drop-Database`. Los usuarios anteriores se identificaban por correo y ya no podrán
> iniciar sesión.

### 4. Ejecutar

Presionar el botón verde **https** (o F5). La aplicación abre en `https://localhost:7276`.
Al arrancar se crean los roles y el usuario administrador inicial.

---

## Cómo se inicia sesión

El acceso es por **documento de identidad**, no por correo. En el login se elige el tipo de
documento en el menú desplegable y se escribe el número.

Internamente, el usuario de Identity tiene la forma `CÓDIGO-NÚMERO` (por ejemplo
`DNI-71000001`), de modo que un DNI y un pasaporte con el mismo número son cuentas
distintas. El prefijo lo agrega el sistema: el usuario solo escribe el número.

| Tipo | Código | Formato |
|---|---|---|
| DNI | DNI | 8 dígitos |
| Carné de extranjería | CE | 9 a 12, letras y números |
| Pasaporte | PAS | 6 a 12, letras y números |
| Certificado de nacido vivo | CNV | 10 dígitos |

El **correo es opcional** y sirve solo para recuperar la contraseña; no es único, así que
una familia puede compartirlo.

## Usuarios y datos de prueba

En todos los casos se elige **DNI** como tipo de documento.

| Rol | Documento | Contraseña |
|---|---|---|
| Administrador | `99999999` | `Admin2026` |
| Doctores (demo) | `41234501` a `41234519` | `Doctor2026` |
| Paciente 100 % (María Quispe) | `71000001` | `Paciente2026` |
| Paciente 70 % (Pedro Alarcón) | `71000002` | `Paciente2026` |
| Paciente 40 %, bloqueado (Carmen Rojas) | `71000003` | `Paciente2026` |
| Paciente nuevo | se crea desde **Registrarse** | la que elija (mínimo 8 caracteres) |

Roles del sistema: `Administrador`, `Admision`, `Doctor`, `Paciente`.

**Datos de demostración:** al arrancar en modo Development (F5 en Visual Studio) se cargan
3 IPRESS, 19 doctores con horarios que cubren todos los días de 6:00 a 22:00 y 15
pacientes con distinto porcentaje de importancia (ver `Data/DatosDemo.cs`). No se duplican
si ya existen. El doctor Roberto Chávez (`41234509`, CMP terminado en 0) queda "No
habilitado" para demostrar la validación del CMP.

> La semilla **solo crea horarios cuando el doctor no tiene ninguno**: los bloques que se
> agreguen a mano desde la pantalla de Horarios no se pierden al reiniciar la aplicación.

Los datos del administrador inicial se pueden cambiar en `appsettings.Development.json`:

```json
"AdminInicial": {
  "Documento": "99999999",
  "Correo": "otro@correo.com",
  "Contrasena": "OtraClave2026"
}
```

Solo se aplican al crear la base por primera vez.

---

## Arquitectura

Arquitectura en capas, con separación entre presentación, lógica de negocio y acceso a
datos:

```
SistemaCitas/
├── Controllers/   Presentación: reciben la petición y muestran la vista
├── Views/         Presentación: pantallas Razor (Bootstrap)
├── ViewModels/    Datos y validaciones de los formularios
├── Services/      Lógica de negocio (reglas del informe)
├── Repositories/  Acceso a datos (patrón Repository genérico)
├── Models/        Entidades del modelo de datos
├── Helpers/       Utilidades transversales (documento, reloj)
├── Data/          DbContext, roles y datos iniciales
└── Migrations/    Migraciones de Entity Framework
```

Flujo: **Controlador → Servicio → Repositorio → Base de datos.**

Dos utilidades transversales que conviene conocer antes de tocar el código:

- **`Helpers/Documento.cs`**: arma el usuario de acceso (`DNI-71000001`) y valida el
  número según el tipo elegido. Nadie debe concatenar el usuario a mano.
- **`Helpers/Reloj.cs`**: entrega la fecha y hora en `America/Lima`. **No usar
  `DateTime.Now`** en código nuevo: si el servidor corre en UTC, la agenda se desplaza un
  día.

## Módulos y estado

| Caso de uso | Módulo | Estado |
|---|---|---|
| CU-02 | Inicio de sesión y registro de pacientes | Implementado |
| CU-01 | Registro presencial de pacientes (Admisión) | Implementado |
| — | CRUD de Especialidades | Implementado |
| CU-03 | Gestionar IPRESS | Implementado |
| CU-04 | Gestionar doctores y horarios por tipo de contrato | Implementado |
| CU-05 | Validar habilitación CMP (simulada) | Implementado |
| CU-06 | Agendar cita web con prioridad | Implementado |
| CU-07 | Gestionar estado de cita ("Asistió" / "Faltó" / "Cancelar") | Implementado |
| CU-08 | Cita presencial (con verificación de DNI físico) | Implementado |
| CU-09 | Consultorio del doctor: agenda, atención con receta e historial | Implementado |
| CU-10 | Dashboard de estadísticas, usuarios activos y auditoría | Implementado |

## Reglas del informe implementadas

- **RC-02**: no se pueden programar horarios que superen las horas semanales del contrato
  (Honorarios 12 h, Part time 24 h, Full day 48 h), ni horarios que se crucen.
- **RC-03**: para asignar una cita presencial, Admisión debe confirmar que verificó el DNI
  físico del paciente.
- **RO-03 / RF-05**: la habilitación del CMP se valida de forma simulada; un doctor no
  habilitado no puede tener horarios ni recibir citas.
- **RF-01**: todo paciente nuevo inicia con 100 % de importancia.
- **RF-08 (prioridad)**: con 80 % o más el paciente reserva cualquier cupo, incluso para
  hoy; con menos de 80 % solo cupos con 3 días o más de anticipación; con menos de 50 %
  queda bloqueado para reservar por la web (puede agendar presencialmente en Admisión).
- **RF-11**: cada inasistencia resta 20 puntos y queda registrada en `HISTORIAL_PUNTAJE`
  (quién, cuándo y motivo).
- Un cupo no se puede reservar dos veces: hay un índice único en base de datos y el
  sistema controla la reserva simultánea.
- **RF-12 / RF-13 / RF-14**: el doctor ve solo su propia agenda, registra la atención
  (triaje, diagnóstico CIE-10 y receta) y consulta el historial clínico solo de pacientes
  que tienen o tuvieron cita con él.
- **RNF-01 (seguridad y auditoría)**: las contraseñas se guardan con hash (PBKDF2 de
  Identity), la cuenta se bloquea 15 minutos tras 5 intentos fallidos, y los cambios
  críticos quedan en la tabla `AUDITORIA` con usuario, fecha y valor anterior y nuevo.
  El registro web **no inicia sesión automáticamente**: el paciente debe autenticarse con
  las credenciales que creó.
- **RNF-02 (validación de datos)**: documento validado según su tipo, nombres y apellidos
  solo con letras, teléfono de 9 dígitos que empieza en 9, fecha de nacimiento no futura y
  mayor de 18 años para crear cuenta web, y código RENIPRESS de 8 dígitos numéricos que se
  completa con ceros a la izquierda (`25` → `00000025`).
- **RNF-03 (JSON)**: endpoints para integración, con sesión de Administrador o Admisión:
  `GET /api/doctores`, `GET /api/doctores/{id}/cupos?fecha=2026-09-21` y
  `GET /api/cmp/{cmp}`. La consulta del CMP está aislada en `CmpIntegrationService`:
  cuando exista la API real del Colegio Médico, solo se cambia esa clase.
- **RF-15**: el Dashboard muestra estadísticas, la auditoría de puntajes y la auditoría
  general; la pantalla Usuarios activos lista las cuentas habilitadas con su rol.
- Cada pantalla está protegida por rol (`[Authorize]`); el paciente solo ve y cancela sus
  propias citas.

## Estándares de interfaz

Las pantallas nuevas siguen estas convenciones:

- **Tablas**: `class="table table-striped table-bordered blue-header-table"`. La clase
  `blue-header-table` (en `wwwroot/css/site.css`) pinta la cabecera de azul corporativo
  `#0D47A1`; los `<th>` no llevan estilos en línea.
- **Caption de la tabla**: buscador a la izquierda y contador "Mostrando X de Y" a la
  derecha, dentro de un `div.table-caption-bar`.
- **Botones**: guardar o agregar en `btn-success` con `bi-save` o `bi-plus-lg`; volver o
  cancelar en `btn-secondary`; eliminar o desactivar en `btn-outline-danger`.
- **Contraseñas**: usan el botón "ver clave" de `wwwroot/js/ver-clave.js`.
- **Documento de identidad**: el campo se adapta al tipo elegido con
  `wwwroot/js/documento.js` (largo máximo, si admite letras y texto de ayuda). Al agregar
  un tipo al catálogo hay que actualizar también ese archivo.

## Flujo de trabajo del equipo

- Cada integrante trabaja en su propia rama (`nombre/feat-modulo`) y hace commits
  pequeños.
- Los cambios llegan a `main` mediante Pull Request, con **Create a merge commit** (no
  Squash ni Rebase), para conservar los commits de cada integrante.
- Antes de abrir un Pull Request se trae `main` a la propia rama, se compila y se prueba.
- El modelo de datos y las migraciones se modifican coordinando con el encargado de la
  base de datos.
- Si un Pull Request incluye una migración, **avisar al equipo**: todos deben ejecutar
  `Update-Database` después de fusionar.

## Pendientes

- Restablecimiento de contraseña desde Admisión (el paciente acude con su documento
  físico) y recuperación por correo para quienes lo hayan registrado.
- Combos en cascada Departamento → Provincia → Distrito en los formularios de registro.

## Integrantes

- Kevin Castro
- Juan Rivera
- Alexander Contreras
- David Oruna
- Enzo Pastor
