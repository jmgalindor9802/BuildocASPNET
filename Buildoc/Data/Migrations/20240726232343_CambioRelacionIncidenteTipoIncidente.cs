using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class CambioRelacionIncidenteTipoIncidente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Incidentes_TipoIncidenteId",
                table: "Incidentes");

            migrationBuilder.AlterColumn<Guid>(
                name: "TipoIncidenteId",
                table: "Incidentes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_TipoIncidenteId",
                table: "Incidentes",
                column: "TipoIncidenteId",
                unique: true,
                filter: "[TipoIncidenteId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Incidentes_TipoIncidenteId",
                table: "Incidentes");

            migrationBuilder.AlterColumn<Guid>(
                name: "TipoIncidenteId",
                table: "Incidentes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_TipoIncidenteId",
                table: "Incidentes",
                column: "TipoIncidenteId",
                unique: true);
        }
    }
}
