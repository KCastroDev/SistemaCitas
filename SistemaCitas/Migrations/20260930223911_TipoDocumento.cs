using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaCitas.Migrations
{
    /// <inheritdoc />
    public partial class TipoDocumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PACIENTE_Dni",
                table: "PACIENTE");

            migrationBuilder.AlterColumn<string>(
                name: "Dni",
                table: "PACIENTE",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AddColumn<int>(
                name: "IdTipoDocumento",
                table: "PACIENTE",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TIPO_DOCUMENTO",
                columns: table => new
                {
                    IdTipoDocumento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    LongitudMinima = table.Column<int>(type: "int", nullable: false),
                    LongitudMaxima = table.Column<int>(type: "int", nullable: false),
                    SoloNumeros = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIPO_DOCUMENTO", x => x.IdTipoDocumento);
                });

            migrationBuilder.InsertData(
                table: "TIPO_DOCUMENTO",
                columns: new[] { "IdTipoDocumento", "Activo", "Codigo", "LongitudMaxima", "LongitudMinima", "Nombre", "SoloNumeros" },
                values: new object[,]
                {
                    { 1, true, "DNI", 8, 8, "DNI", true },
                    { 2, true, "CE", 12, 9, "Carné de extranjería", false },
                    { 3, true, "PAS", 12, 6, "Pasaporte", false },
                    { 4, true, "CNV", 10, 10, "Certificado de nacido vivo", true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PACIENTE_IdTipoDocumento_Dni",
                table: "PACIENTE",
                columns: new[] { "IdTipoDocumento", "Dni" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TIPO_DOCUMENTO_Codigo",
                table: "TIPO_DOCUMENTO",
                column: "Codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PACIENTE_TIPO_DOCUMENTO_IdTipoDocumento",
                table: "PACIENTE",
                column: "IdTipoDocumento",
                principalTable: "TIPO_DOCUMENTO",
                principalColumn: "IdTipoDocumento",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PACIENTE_TIPO_DOCUMENTO_IdTipoDocumento",
                table: "PACIENTE");

            migrationBuilder.DropTable(
                name: "TIPO_DOCUMENTO");

            migrationBuilder.DropIndex(
                name: "IX_PACIENTE_IdTipoDocumento_Dni",
                table: "PACIENTE");

            migrationBuilder.DropColumn(
                name: "IdTipoDocumento",
                table: "PACIENTE");

            migrationBuilder.AlterColumn<string>(
                name: "Dni",
                table: "PACIENTE",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.CreateIndex(
                name: "IX_PACIENTE_Dni",
                table: "PACIENTE",
                column: "Dni",
                unique: true);
        }
    }
}
