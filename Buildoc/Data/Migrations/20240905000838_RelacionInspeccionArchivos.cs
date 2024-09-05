using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class RelacionInspeccionArchivos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileModels_Inspeccion_InspeccionId",
                table: "FileModels");

           

            migrationBuilder.AddForeignKey(
                name: "FK_FileModels_Inspeccion_InspeccionId",
                table: "FileModels",
                column: "InspeccionId",
                principalTable: "Inspeccion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileModels_Inspeccion_InspeccionId",
                table: "FileModels");

           

            migrationBuilder.AddForeignKey(
                name: "FK_FileModels_Inspeccion_InspeccionId",
                table: "FileModels",
                column: "InspeccionId",
                principalTable: "Inspeccion",
                principalColumn: "Id");
        }
    }
}
