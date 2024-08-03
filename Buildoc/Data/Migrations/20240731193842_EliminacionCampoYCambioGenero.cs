using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class EliminacionCampoYCambioGenero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneroFemenino",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "GeneroMasculino",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "UsoEPP",
                table: "Afectados");

            migrationBuilder.AddColumn<string>(
                name: "GeneroAfectado",
                table: "Afectados",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneroAfectado",
                table: "Afectados");

            migrationBuilder.AddColumn<bool>(
                name: "GeneroFemenino",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GeneroMasculino",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UsoEPP",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
