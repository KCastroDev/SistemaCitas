using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaCitas.Migrations
{
    /// <inheritdoc />
    public partial class InicialCitasIpress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CIE10",
                columns: table => new
                {
                    CodigoCie10 = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CIE10", x => x.CodigoCie10);
                });

            migrationBuilder.CreateTable(
                name: "DEPARTAMENTO",
                columns: table => new
                {
                    IdDepartamento = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEPARTAMENTO", x => x.IdDepartamento);
                });

            migrationBuilder.CreateTable(
                name: "ESPECIALIDAD",
                columns: table => new
                {
                    IdEspecialidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESPECIALIDAD", x => x.IdEspecialidad);
                });

            migrationBuilder.CreateTable(
                name: "MEDICAMENTO",
                columns: table => new
                {
                    IdMedicamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Presentacion = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEDICAMENTO", x => x.IdMedicamento);
                });

            migrationBuilder.CreateTable(
                name: "PLAN_SEGURO",
                columns: table => new
                {
                    IdPlanSeguro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PLAN_SEGURO", x => x.IdPlanSeguro);
                });

            migrationBuilder.CreateTable(
                name: "ROL",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROL", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TIPO_CONTRATO",
                columns: table => new
                {
                    IdTipoContrato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    HorasSemanales = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIPO_CONTRATO", x => x.IdTipoContrato);
                });

            migrationBuilder.CreateTable(
                name: "UNIDAD_EJECUTORA",
                columns: table => new
                {
                    IdUnidadEjecutora = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UNIDAD_EJECUTORA", x => x.IdUnidadEjecutora);
                });

            migrationBuilder.CreateTable(
                name: "PROVINCIA",
                columns: table => new
                {
                    IdProvincia = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    IdDepartamento = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROVINCIA", x => x.IdProvincia);
                    table.ForeignKey(
                        name: "FK_PROVINCIA_DEPARTAMENTO_IdDepartamento",
                        column: x => x.IdDepartamento,
                        principalTable: "DEPARTAMENTO",
                        principalColumn: "IdDepartamento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ROL_CLAIM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROL_CLAIM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ROL_CLAIM_ROL_RoleId",
                        column: x => x.RoleId,
                        principalTable: "ROL",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DISTRITO",
                columns: table => new
                {
                    IdDistrito = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    IdProvincia = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRITO", x => x.IdDistrito);
                    table.ForeignKey(
                        name: "FK_DISTRITO_PROVINCIA_IdProvincia",
                        column: x => x.IdProvincia,
                        principalTable: "PROVINCIA",
                        principalColumn: "IdProvincia",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IPRESS",
                columns: table => new
                {
                    IdIpress = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoRenipress = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NivelAtencion = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    IdUnidadEjecutora = table.Column<int>(type: "int", nullable: false),
                    IdDistrito = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPRESS", x => x.IdIpress);
                    table.ForeignKey(
                        name: "FK_IPRESS_DISTRITO_IdDistrito",
                        column: x => x.IdDistrito,
                        principalTable: "DISTRITO",
                        principalColumn: "IdDistrito",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IPRESS_UNIDAD_EJECUTORA_IdUnidadEjecutora",
                        column: x => x.IdUnidadEjecutora,
                        principalTable: "UNIDAD_EJECUTORA",
                        principalColumn: "IdUnidadEjecutora",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IdIpress = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_USUARIO_IPRESS_IdIpress",
                        column: x => x.IdIpress,
                        principalTable: "IPRESS",
                        principalColumn: "IdIpress");
                });

            migrationBuilder.CreateTable(
                name: "AUDITORIA",
                columns: table => new
                {
                    IdAuditoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Entidad = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ClaveRegistro = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    ValorAnterior = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ValorNuevo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDITORIA", x => x.IdAuditoria);
                    table.ForeignKey(
                        name: "FK_AUDITORIA_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "USUARIO",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DOCTOR",
                columns: table => new
                {
                    IdDoctor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cmp = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Dni = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    EstadoHabilitacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaValidacionCmp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    IdIpress = table.Column<int>(type: "int", nullable: false),
                    IdTipoContrato = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCTOR", x => x.IdDoctor);
                    table.ForeignKey(
                        name: "FK_DOCTOR_IPRESS_IdIpress",
                        column: x => x.IdIpress,
                        principalTable: "IPRESS",
                        principalColumn: "IdIpress",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOCTOR_TIPO_CONTRATO_IdTipoContrato",
                        column: x => x.IdTipoContrato,
                        principalTable: "TIPO_CONTRATO",
                        principalColumn: "IdTipoContrato",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOCTOR_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PACIENTE",
                columns: table => new
                {
                    IdPaciente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dni = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    Sexo = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    PorcentajeImportancia = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IdPlanSeguro = table.Column<int>(type: "int", nullable: false),
                    IdDistrito = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RegistradoPorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PACIENTE", x => x.IdPaciente);
                    table.ForeignKey(
                        name: "FK_PACIENTE_DISTRITO_IdDistrito",
                        column: x => x.IdDistrito,
                        principalTable: "DISTRITO",
                        principalColumn: "IdDistrito",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PACIENTE_PLAN_SEGURO_IdPlanSeguro",
                        column: x => x.IdPlanSeguro,
                        principalTable: "PLAN_SEGURO",
                        principalColumn: "IdPlanSeguro",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PACIENTE_USUARIO_RegistradoPorId",
                        column: x => x.RegistradoPorId,
                        principalTable: "USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PACIENTE_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO_CLAIM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO_CLAIM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_USUARIO_CLAIM_USUARIO_UserId",
                        column: x => x.UserId,
                        principalTable: "USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO_LOGIN",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO_LOGIN", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_USUARIO_LOGIN_USUARIO_UserId",
                        column: x => x.UserId,
                        principalTable: "USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO_ROL",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO_ROL", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_USUARIO_ROL_ROL_RoleId",
                        column: x => x.RoleId,
                        principalTable: "ROL",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_USUARIO_ROL_USUARIO_UserId",
                        column: x => x.UserId,
                        principalTable: "USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO_TOKEN",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO_TOKEN", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_USUARIO_TOKEN_USUARIO_UserId",
                        column: x => x.UserId,
                        principalTable: "USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOCTOR_ESPECIALIDAD",
                columns: table => new
                {
                    IdDoctor = table.Column<int>(type: "int", nullable: false),
                    IdEspecialidad = table.Column<int>(type: "int", nullable: false),
                    Rne = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCTOR_ESPECIALIDAD", x => new { x.IdDoctor, x.IdEspecialidad });
                    table.ForeignKey(
                        name: "FK_DOCTOR_ESPECIALIDAD_DOCTOR_IdDoctor",
                        column: x => x.IdDoctor,
                        principalTable: "DOCTOR",
                        principalColumn: "IdDoctor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOCTOR_ESPECIALIDAD_ESPECIALIDAD_IdEspecialidad",
                        column: x => x.IdEspecialidad,
                        principalTable: "ESPECIALIDAD",
                        principalColumn: "IdEspecialidad",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HORARIO_DOCTOR",
                columns: table => new
                {
                    IdHorario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDoctor = table.Column<int>(type: "int", nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeOnly>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeOnly>(type: "time", nullable: false),
                    CuposPorHora = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HORARIO_DOCTOR", x => x.IdHorario);
                    table.ForeignKey(
                        name: "FK_HORARIO_DOCTOR_DOCTOR_IdDoctor",
                        column: x => x.IdDoctor,
                        principalTable: "DOCTOR",
                        principalColumn: "IdDoctor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CITA",
                columns: table => new
                {
                    IdCita = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPaciente = table.Column<int>(type: "int", nullable: false),
                    IdDoctor = table.Column<int>(type: "int", nullable: false),
                    IdIpress = table.Column<int>(type: "int", nullable: false),
                    FechaCita = table.Column<DateOnly>(type: "date", nullable: false),
                    HoraCita = table.Column<TimeOnly>(type: "time", nullable: false),
                    HoraLlegada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PuntajeAlAgendar = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Canal = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CITA", x => x.IdCita);
                    table.ForeignKey(
                        name: "FK_CITA_DOCTOR_IdDoctor",
                        column: x => x.IdDoctor,
                        principalTable: "DOCTOR",
                        principalColumn: "IdDoctor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CITA_IPRESS_IdIpress",
                        column: x => x.IdIpress,
                        principalTable: "IPRESS",
                        principalColumn: "IdIpress",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CITA_PACIENTE_IdPaciente",
                        column: x => x.IdPaciente,
                        principalTable: "PACIENTE",
                        principalColumn: "IdPaciente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ATENCION_MEDICA",
                columns: table => new
                {
                    IdAtencion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCita = table.Column<int>(type: "int", nullable: false),
                    FechaHoraAtencion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MotivoConsulta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Sintomatologia = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATENCION_MEDICA", x => x.IdAtencion);
                    table.ForeignKey(
                        name: "FK_ATENCION_MEDICA_CITA_IdCita",
                        column: x => x.IdCita,
                        principalTable: "CITA",
                        principalColumn: "IdCita",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HISTORIAL_PUNTAJE",
                columns: table => new
                {
                    IdHistorialPuntaje = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPaciente = table.Column<int>(type: "int", nullable: false),
                    IdCita = table.Column<int>(type: "int", nullable: true),
                    PuntajeAnterior = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PuntajeNuevo = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RegistradoPorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HISTORIAL_PUNTAJE", x => x.IdHistorialPuntaje);
                    table.ForeignKey(
                        name: "FK_HISTORIAL_PUNTAJE_CITA_IdCita",
                        column: x => x.IdCita,
                        principalTable: "CITA",
                        principalColumn: "IdCita");
                    table.ForeignKey(
                        name: "FK_HISTORIAL_PUNTAJE_PACIENTE_IdPaciente",
                        column: x => x.IdPaciente,
                        principalTable: "PACIENTE",
                        principalColumn: "IdPaciente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HISTORIAL_PUNTAJE_USUARIO_RegistradoPorId",
                        column: x => x.RegistradoPorId,
                        principalTable: "USUARIO",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TRIAJE",
                columns: table => new
                {
                    IdTriaje = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCita = table.Column<int>(type: "int", nullable: false),
                    PesoKg = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TallaCm = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TemperaturaC = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    PresionArterial = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    FrecuenciaCardiaca = table.Column<int>(type: "int", nullable: true),
                    SaturacionO2 = table.Column<int>(type: "int", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRIAJE", x => x.IdTriaje);
                    table.ForeignKey(
                        name: "FK_TRIAJE_CITA_IdCita",
                        column: x => x.IdCita,
                        principalTable: "CITA",
                        principalColumn: "IdCita",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ATENCION_DIAGNOSTICO",
                columns: table => new
                {
                    IdAtencion = table.Column<int>(type: "int", nullable: false),
                    CodigoCie10 = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TipoDiagnostico = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATENCION_DIAGNOSTICO", x => new { x.IdAtencion, x.CodigoCie10 });
                    table.ForeignKey(
                        name: "FK_ATENCION_DIAGNOSTICO_ATENCION_MEDICA_IdAtencion",
                        column: x => x.IdAtencion,
                        principalTable: "ATENCION_MEDICA",
                        principalColumn: "IdAtencion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ATENCION_DIAGNOSTICO_CIE10_CodigoCie10",
                        column: x => x.CodigoCie10,
                        principalTable: "CIE10",
                        principalColumn: "CodigoCie10",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DETALLE_RECETA",
                columns: table => new
                {
                    IdDetalleReceta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAtencion = table.Column<int>(type: "int", nullable: false),
                    IdMedicamento = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Indicaciones = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DETALLE_RECETA", x => x.IdDetalleReceta);
                    table.ForeignKey(
                        name: "FK_DETALLE_RECETA_ATENCION_MEDICA_IdAtencion",
                        column: x => x.IdAtencion,
                        principalTable: "ATENCION_MEDICA",
                        principalColumn: "IdAtencion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DETALLE_RECETA_MEDICAMENTO_IdMedicamento",
                        column: x => x.IdMedicamento,
                        principalTable: "MEDICAMENTO",
                        principalColumn: "IdMedicamento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CIE10",
                columns: new[] { "CodigoCie10", "Descripcion" },
                values: new object[,]
                {
                    { "E11", "Diabetes mellitus tipo 2" },
                    { "I10", "Hipertension esencial (primaria)" },
                    { "J00", "Rinofaringitis aguda (resfriado comun)" }
                });

            migrationBuilder.InsertData(
                table: "DEPARTAMENTO",
                columns: new[] { "IdDepartamento", "Nombre" },
                values: new object[] { "13", "La Libertad" });

            migrationBuilder.InsertData(
                table: "ESPECIALIDAD",
                columns: new[] { "IdEspecialidad", "Nombre" },
                values: new object[,]
                {
                    { 1, "Medicina General" },
                    { 2, "Pediatria" },
                    { 3, "Ginecologia" },
                    { 4, "Cirugia" }
                });

            migrationBuilder.InsertData(
                table: "PLAN_SEGURO",
                columns: new[] { "IdPlanSeguro", "Nombre" },
                values: new object[,]
                {
                    { 1, "SIS Gratuito" },
                    { 2, "SIS Para Todos" },
                    { 3, "SIS Independiente" }
                });

            migrationBuilder.InsertData(
                table: "TIPO_CONTRATO",
                columns: new[] { "IdTipoContrato", "HorasSemanales", "Nombre" },
                values: new object[,]
                {
                    { 1, 12, "Honorarios" },
                    { 2, 24, "Part time" },
                    { 3, 48, "Full day" }
                });

            migrationBuilder.InsertData(
                table: "PROVINCIA",
                columns: new[] { "IdProvincia", "IdDepartamento", "Nombre" },
                values: new object[,]
                {
                    { "1301", "13", "Trujillo" },
                    { "1302", "13", "Ascope" },
                    { "1311", "13", "Santiago de Chuco" }
                });

            migrationBuilder.InsertData(
                table: "DISTRITO",
                columns: new[] { "IdDistrito", "IdProvincia", "Nombre" },
                values: new object[,]
                {
                    { "130101", "1301", "Trujillo" },
                    { "130104", "1301", "Laredo" },
                    { "130105", "1301", "Moche" },
                    { "130109", "1301", "Huanchaco" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ATENCION_DIAGNOSTICO_CodigoCie10",
                table: "ATENCION_DIAGNOSTICO",
                column: "CodigoCie10");

            migrationBuilder.CreateIndex(
                name: "IX_ATENCION_MEDICA_IdCita",
                table: "ATENCION_MEDICA",
                column: "IdCita",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUDITORIA_UsuarioId",
                table: "AUDITORIA",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CITA_IdDoctor_FechaCita_HoraCita",
                table: "CITA",
                columns: new[] { "IdDoctor", "FechaCita", "HoraCita" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CITA_IdIpress",
                table: "CITA",
                column: "IdIpress");

            migrationBuilder.CreateIndex(
                name: "IX_CITA_IdPaciente",
                table: "CITA",
                column: "IdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_DETALLE_RECETA_IdAtencion",
                table: "DETALLE_RECETA",
                column: "IdAtencion");

            migrationBuilder.CreateIndex(
                name: "IX_DETALLE_RECETA_IdMedicamento",
                table: "DETALLE_RECETA",
                column: "IdMedicamento");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRITO_IdProvincia_Nombre",
                table: "DISTRITO",
                columns: new[] { "IdProvincia", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_Cmp",
                table: "DOCTOR",
                column: "Cmp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_Dni",
                table: "DOCTOR",
                column: "Dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_IdIpress",
                table: "DOCTOR",
                column: "IdIpress");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_IdTipoContrato",
                table: "DOCTOR",
                column: "IdTipoContrato");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_UsuarioId",
                table: "DOCTOR",
                column: "UsuarioId",
                unique: true,
                filter: "[UsuarioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_ESPECIALIDAD_IdEspecialidad",
                table: "DOCTOR_ESPECIALIDAD",
                column: "IdEspecialidad");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORIAL_PUNTAJE_IdCita",
                table: "HISTORIAL_PUNTAJE",
                column: "IdCita");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORIAL_PUNTAJE_IdPaciente",
                table: "HISTORIAL_PUNTAJE",
                column: "IdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORIAL_PUNTAJE_RegistradoPorId",
                table: "HISTORIAL_PUNTAJE",
                column: "RegistradoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_HORARIO_DOCTOR_IdDoctor",
                table: "HORARIO_DOCTOR",
                column: "IdDoctor");

            migrationBuilder.CreateIndex(
                name: "IX_IPRESS_CodigoRenipress",
                table: "IPRESS",
                column: "CodigoRenipress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IPRESS_IdDistrito",
                table: "IPRESS",
                column: "IdDistrito");

            migrationBuilder.CreateIndex(
                name: "IX_IPRESS_IdUnidadEjecutora",
                table: "IPRESS",
                column: "IdUnidadEjecutora");

            migrationBuilder.CreateIndex(
                name: "IX_PACIENTE_Dni",
                table: "PACIENTE",
                column: "Dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PACIENTE_IdDistrito",
                table: "PACIENTE",
                column: "IdDistrito");

            migrationBuilder.CreateIndex(
                name: "IX_PACIENTE_IdPlanSeguro",
                table: "PACIENTE",
                column: "IdPlanSeguro");

            migrationBuilder.CreateIndex(
                name: "IX_PACIENTE_RegistradoPorId",
                table: "PACIENTE",
                column: "RegistradoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_PACIENTE_UsuarioId",
                table: "PACIENTE",
                column: "UsuarioId",
                unique: true,
                filter: "[UsuarioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PROVINCIA_IdDepartamento",
                table: "PROVINCIA",
                column: "IdDepartamento");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "ROL",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ROL_CLAIM_RoleId",
                table: "ROL_CLAIM",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TRIAJE_IdCita",
                table: "TRIAJE",
                column: "IdCita",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "USUARIO",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_IdIpress",
                table: "USUARIO",
                column: "IdIpress");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "USUARIO",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_CLAIM_UserId",
                table: "USUARIO_CLAIM",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_LOGIN_UserId",
                table: "USUARIO_LOGIN",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_ROL_RoleId",
                table: "USUARIO_ROL",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ATENCION_DIAGNOSTICO");

            migrationBuilder.DropTable(
                name: "AUDITORIA");

            migrationBuilder.DropTable(
                name: "DETALLE_RECETA");

            migrationBuilder.DropTable(
                name: "DOCTOR_ESPECIALIDAD");

            migrationBuilder.DropTable(
                name: "HISTORIAL_PUNTAJE");

            migrationBuilder.DropTable(
                name: "HORARIO_DOCTOR");

            migrationBuilder.DropTable(
                name: "ROL_CLAIM");

            migrationBuilder.DropTable(
                name: "TRIAJE");

            migrationBuilder.DropTable(
                name: "USUARIO_CLAIM");

            migrationBuilder.DropTable(
                name: "USUARIO_LOGIN");

            migrationBuilder.DropTable(
                name: "USUARIO_ROL");

            migrationBuilder.DropTable(
                name: "USUARIO_TOKEN");

            migrationBuilder.DropTable(
                name: "CIE10");

            migrationBuilder.DropTable(
                name: "ATENCION_MEDICA");

            migrationBuilder.DropTable(
                name: "MEDICAMENTO");

            migrationBuilder.DropTable(
                name: "ESPECIALIDAD");

            migrationBuilder.DropTable(
                name: "ROL");

            migrationBuilder.DropTable(
                name: "CITA");

            migrationBuilder.DropTable(
                name: "DOCTOR");

            migrationBuilder.DropTable(
                name: "PACIENTE");

            migrationBuilder.DropTable(
                name: "TIPO_CONTRATO");

            migrationBuilder.DropTable(
                name: "PLAN_SEGURO");

            migrationBuilder.DropTable(
                name: "USUARIO");

            migrationBuilder.DropTable(
                name: "IPRESS");

            migrationBuilder.DropTable(
                name: "DISTRITO");

            migrationBuilder.DropTable(
                name: "UNIDAD_EJECUTORA");

            migrationBuilder.DropTable(
                name: "PROVINCIA");

            migrationBuilder.DropTable(
                name: "DEPARTAMENTO");
        }
    }
}
