using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AnadirCamposBoolAfectado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Hospitalizado",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LesionGravedad",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LesionLeve",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneroFemenino",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "GeneroMasculino",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "Hospitalizado",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "LesionGravedad",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "LesionLeve",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "UsoEPP",
                table: "Afectados");
        }
    }
}
