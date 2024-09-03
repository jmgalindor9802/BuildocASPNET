using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class RelacionIncidenteYFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IncidenteId",
                table: "FileModels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_IncidenteId",
                table: "FileModels",
                column: "IncidenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileModels_Incidentes_IncidenteId",
                table: "FileModels",
                column: "IncidenteId",
                principalTable: "Incidentes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileModels_Incidentes_IncidenteId",
                table: "FileModels");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_IncidenteId",
                table: "FileModels");

            migrationBuilder.DropColumn(
                name: "IncidenteId",
                table: "FileModels");
        }
    }
}
