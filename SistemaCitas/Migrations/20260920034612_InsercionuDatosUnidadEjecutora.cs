using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaCitas.Migrations
{
    /// <inheritdoc />
    public partial class InsercionuDatosUnidadEjecutora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UNIDAD_EJECUTORA",
                columns: new[] { "IdUnidadEjecutora", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, "001-1310", "Red de Salud Trujillo" },
                    { 2, "001-1311", "Hospital Regional Docente de Trujillo" },
                    { 3, "001-1312", "Red de Salud Sánchez Carrión" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UNIDAD_EJECUTORA",
                keyColumn: "IdUnidadEjecutora",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UNIDAD_EJECUTORA",
                keyColumn: "IdUnidadEjecutora",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UNIDAD_EJECUTORA",
                keyColumn: "IdUnidadEjecutora",
                keyValue: 3);
        }
    }
}
