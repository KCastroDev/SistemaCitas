using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaCitas.Migrations
{
    /// <inheritdoc />
    public partial class AgregarActivoEspecialidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "ESPECIALIDAD",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ESPECIALIDAD",
                keyColumn: "IdEspecialidad",
                keyValue: 1,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "ESPECIALIDAD",
                keyColumn: "IdEspecialidad",
                keyValue: 2,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "ESPECIALIDAD",
                keyColumn: "IdEspecialidad",
                keyValue: 3,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "ESPECIALIDAD",
                keyColumn: "IdEspecialidad",
                keyValue: 4,
                column: "Activo",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "ESPECIALIDAD");
        }
    }
}
