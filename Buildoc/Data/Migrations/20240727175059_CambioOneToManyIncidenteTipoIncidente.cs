using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class CambioOneToManyIncidenteTipoIncidente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Incidentes_TipoIncidenteId",
                table: "Incidentes");

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_TipoIncidenteId",
                table: "Incidentes",
                column: "TipoIncidenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Incidentes_TipoIncidenteId",
                table: "Incidentes");

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_TipoIncidenteId",
                table: "Incidentes",
                column: "TipoIncidenteId",
                unique: true,
                filter: "[TipoIncidenteId] IS NOT NULL");
        }
    }
}
