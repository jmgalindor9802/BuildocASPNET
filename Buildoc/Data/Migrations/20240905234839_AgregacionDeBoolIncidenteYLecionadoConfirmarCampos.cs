using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregacionDeBoolIncidenteYLecionadoConfirmarCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConfimacionDefuncion",
                table: "Lesionados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CierrePorServidor",
                table: "Incidentes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfimacionDefuncion",
                table: "Lesionados");

            migrationBuilder.DropColumn(
                name: "CierrePorServidor",
                table: "Incidentes");
        }
    }
}
