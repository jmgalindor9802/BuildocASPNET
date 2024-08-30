using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class RespuestaInspeccionFileModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RespuestaInspeccionId",
                table: "FileModels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_RespuestaInspeccionId",
                table: "FileModels",
                column: "RespuestaInspeccionId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileModels_RespuestaInspeccion_RespuestaInspeccionId",
                table: "FileModels",
                column: "RespuestaInspeccionId",
                principalTable: "RespuestaInspeccion",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileModels_RespuestaInspeccion_RespuestaInspeccionId",
                table: "FileModels");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_RespuestaInspeccionId",
                table: "FileModels");

            migrationBuilder.DropColumn(
                name: "RespuestaInspeccionId",
                table: "FileModels");
        }
    }
}
