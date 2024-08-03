using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AnadirTiposLesionesAfectados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LesionLeve",
                table: "Afectados",
                newName: "QuemadurasQuimicas");

            migrationBuilder.RenameColumn(
                name: "LesionGravedad",
                table: "Afectados",
                newName: "QuemaduraCalor");

            migrationBuilder.AddColumn<bool>(
                name: "AbrasionRasgunos",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Amputacion",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConmocionCerebral",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CorteLaceracionPerforacion",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsguinceTension",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Hernia",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HuesosRotos",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Moreton",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PrimerosAuxilios",
                table: "Afectados",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbrasionRasgunos",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "Amputacion",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "ConmocionCerebral",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "CorteLaceracionPerforacion",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "EsguinceTension",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "Hernia",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "HuesosRotos",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "Moreton",
                table: "Afectados");

            migrationBuilder.DropColumn(
                name: "PrimerosAuxilios",
                table: "Afectados");

            migrationBuilder.RenameColumn(
                name: "QuemadurasQuimicas",
                table: "Afectados",
                newName: "LesionLeve");

            migrationBuilder.RenameColumn(
                name: "QuemaduraCalor",
                table: "Afectados",
                newName: "LesionGravedad");
        }
    }
}
