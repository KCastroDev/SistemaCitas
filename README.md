# SistemaCitas — Gestión de Citas Médicas IPRESS

Sistema web para gestionar citas médicas en las IPRESS de La Libertad. Prioriza la asignación de cupos según el **porcentaje de importancia** del paciente (que baja cuando falta a sus citas) y controla los horarios de los doctores según su tipo de contrato.

Proyecto final del curso **Diseño y Arquitectura de Software** (UPN, Ingeniería de Sistemas).

## Tecnologías

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core 10 (enfoque **Code First** con migraciones)
- SQL Server (LocalDB o SQL Server Express)
- ASP.NET Core Identity (autenticación, roles y contraseñas cifradas)
- Bootstrap 5

## Requisitos previos

1. **Visual Studio 2022 o superior**, con la carga de trabajo *ASP.NET y desarrollo web*.
2. **SDK de .NET 10**.
3. **SQL Server LocalDB** (se instala junto con Visual Studio) o SQL Server Express.
4. **Git**.

## Cómo ejecutar el proyecto en local

### 1. Clonar el repositorio

```
git clone https://github.com/KCastroDev/SistemaCitas.git
```

Abrir el archivo `SistemaCitas.slnx` con Visual Studio.

### 2. Configurar la conexión a la base de datos

El archivo `appsettings.json` trae un texto de relleno como cadena de conexión. **No lo modifique ni lo suba al repositorio.** En su lugar, cada integrante agrega su propia conexión en el archivo `SistemaCitas/appsettings.Development.json`:

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

> Si usa SQL Server Express, cambie `(localdb)\\MSSQLLocalDB` por `.\\SQLEXPRESS`.
> Este cambio es solo para su computadora: no lo incluya en los commits.

### 3. Crear la base de datos

En Visual Studio: **Herramientas → Administrador de paquetes NuGet → Consola del Administrador de paquetes**, con el proyecto predeterminado `SistemaCitas`, y ejecutar:

```
Update-Database
```

Esto crea la base `SistemaCitasDB` con todas sus tablas y los datos iniciales (distritos, planes de seguro, tipos de contrato, especialidades y códigos CIE-10).

### 4. Ejecutar

Presionar el botón verde **https** (o F5). La aplicación abre en `https://localhost:7276`.

Al arrancar, el sistema crea automáticamente los roles y un usuario administrador inicial.

## Usuarios de prueba

| Rol | Correo | Contraseña |
|---|---|---|
| Administrador | `admin@sistemacitas.pe` | `Admin2026` |
| Paciente | se crea desde **Registrarse** en la pantalla de inicio | la que elija (mínimo 8 caracteres, con mayúscula, minúscula y número) |

Roles del sistema: `Administrador`, `Admision`, `Doctor`, `Paciente`.

El correo y la contraseña del administrador inicial se pueden cambiar agregando en `appsettings.Development.json`:

```json
"AdminInicial": { "Correo": "otro@correo.com", "Contrasena": "OtraClave2026" }
```

## Arquitectura

Arquitectura en capas, con separación entre presentación, lógica de negocio y acceso a datos:

```
SistemaCitas/
├── Controllers/   Presentación: reciben la petición y muestran la vista
├── Views/         Presentación: pantallas Razor (Bootstrap)
├── ViewModels/    Datos y validaciones de los formularios
├── Services/      Lógica de negocio (reglas del informe)
├── Repositories/  Acceso a datos (patrón Repository genérico)
├── Models/        Entidades del modelo de datos
├── Data/          DbContext, roles y datos iniciales
└── Migrations/    Migraciones de Entity Framework
```

Flujo: **Controlador → Servicio → Repositorio → Base de datos**.

## Módulos y estado

| Caso de uso | Módulo | Estado |
|---|---|---|
| CU-02 | Inicio de sesión y registro de pacientes | Implementado |
| CU-01 | Registro presencial de pacientes (Admisión) | Implementado |
| — | CRUD de Especialidades | Implementado |
| CU-03 | Gestionar IPRESS | En desarrollo |
| CU-04 | Gestionar doctores y horarios | En desarrollo |
| CU-05 | Validar habilitación CMP (simulada) | En desarrollo |
| CU-06 | Agendar cita web con prioridad | En desarrollo |
| CU-07 | Gestionar estado de cita ("Faltó") | En desarrollo |
| CU-08 | Cita presencial | En desarrollo |
| CU-09 | Registrar atención en consultorio | En desarrollo |
| CU-10 | Dashboards y auditoría | En desarrollo |

## Flujo de trabajo del equipo

- Cada integrante trabaja en su propia rama (`nombre/feat-modulo`) y hace commits pequeños.
- Los cambios llegan a `main` mediante **Pull Request**, con la opción **Merge pull request** (no Squash), para conservar los commits de cada integrante.
- Antes de abrir un Pull Request se trae `main` a la propia rama, se compila y se prueba.
- Solo se modifican el modelo de datos y las migraciones de acuerdo con el encargado de la base de datos.

## Integrantes

- Kevin Castro
- Juan Rivera
- Alexander Contreras
- David Oruna
- Enzo Pastor
