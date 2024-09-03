using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class TipoInspeccionFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileModels_Inspeccion_InspeccionId",
                table: "FileModels");

            migrationBuilder.AlterColumn<Guid>(
                name: "InspeccionId",
                table: "FileModels",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "TipoInspeccionId",
                table: "FileModels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_TipoInspeccionId",
                table: "FileModels",
                column: "TipoInspeccionId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileModels_Inspeccion_InspeccionId",
                table: "FileModels",
                column: "InspeccionId",
                principalTable: "Inspeccion",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileModels_TipoInspeccion_TipoInspeccionId",
                table: "FileModels",
                column: "TipoInspeccionId",
                principalTable: "TipoInspeccion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileModels_Inspeccion_InspeccionId",
                table: "FileModels");

            migrationBuilder.DropForeignKey(
                name: "FK_FileModels_TipoInspeccion_TipoInspeccionId",
                table: "FileModels");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_TipoInspeccionId",
                table: "FileModels");

            migrationBuilder.DropColumn(
                name: "TipoInspeccionId",
                table: "FileModels");

            migrationBuilder.AlterColumn<Guid>(
                name: "InspeccionId",
                table: "FileModels",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FileModels_Inspeccion_InspeccionId",
                table: "FileModels",
                column: "InspeccionId",
                principalTable: "Inspeccion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
