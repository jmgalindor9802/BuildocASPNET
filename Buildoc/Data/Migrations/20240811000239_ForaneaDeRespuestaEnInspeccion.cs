using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class ForaneaDeRespuestaEnInspeccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RespuestaId",
                table: "Inspeccion",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RespuestaId",
                table: "Inspeccion");
        }
    }
}
