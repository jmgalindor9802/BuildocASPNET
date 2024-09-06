using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImplementacionArchivosNovedadIncidente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NovedadesIncidenteId",
                table: "FileModels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_NovedadesIncidenteId",
                table: "FileModels",
                column: "NovedadesIncidenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileModels_NovedadesIncidentes_NovedadesIncidenteId",
                table: "FileModels",
                column: "NovedadesIncidenteId",
                principalTable: "NovedadesIncidentes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileModels_NovedadesIncidentes_NovedadesIncidenteId",
                table: "FileModels");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_NovedadesIncidenteId",
                table: "FileModels");

            migrationBuilder.DropColumn(
                name: "NovedadesIncidenteId",
                table: "FileModels");
        }
    }
}
