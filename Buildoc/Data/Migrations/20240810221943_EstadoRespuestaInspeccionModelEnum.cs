using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class EstadoRespuestaInspeccionModelEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoRespuesta",
                table: "RespuestaInspeccion");

            migrationBuilder.AddColumn<int>(
                name: "EstadoRespuestaInspeccion",
                table: "RespuestaInspeccion",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoRespuestaInspeccion",
                table: "RespuestaInspeccion");

            migrationBuilder.AddColumn<string>(
                name: "EstadoRespuesta",
                table: "RespuestaInspeccion",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
