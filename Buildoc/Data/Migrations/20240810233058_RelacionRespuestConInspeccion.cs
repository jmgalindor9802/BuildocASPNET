using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class RelacionRespuestConInspeccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RespuestaInspeccion_InspeccionId",
                table: "RespuestaInspeccion");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestaInspeccion_InspeccionId",
                table: "RespuestaInspeccion",
                column: "InspeccionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RespuestaInspeccion_InspeccionId",
                table: "RespuestaInspeccion");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestaInspeccion_InspeccionId",
                table: "RespuestaInspeccion",
                column: "InspeccionId");
        }
    }
}
