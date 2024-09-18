using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNovedadInspeccionIdToFileModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NovedadInspeccionId",
                table: "FileModels",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_NovedadInspeccionId",
                table: "FileModels",
                column: "NovedadInspeccionId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileModels_NovedadInspeccion_NovedadInspeccionId",
                table: "FileModels",
                column: "NovedadInspeccionId",
                principalTable: "NovedadInspeccion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileModels_NovedadInspeccion_NovedadInspeccionId",
                table: "FileModels");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_NovedadInspeccionId",
                table: "FileModels");

            migrationBuilder.DropColumn(
                name: "NovedadInspeccionId",
                table: "FileModels");
        }
    }
}
