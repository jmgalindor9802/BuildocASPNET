using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class DuracionInspeccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuracionHoras",
                table: "Inspeccion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsTodoElDia",
                table: "Inspeccion",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuracionHoras",
                table: "Inspeccion");

            migrationBuilder.DropColumn(
                name: "EsTodoElDia",
                table: "Inspeccion");
        }
    }
}
