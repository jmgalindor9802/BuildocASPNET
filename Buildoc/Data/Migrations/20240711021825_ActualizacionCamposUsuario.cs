using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionCamposUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Arl",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Eps",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Arl",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Eps",
                table: "Usuarios");
        }
    }
}
