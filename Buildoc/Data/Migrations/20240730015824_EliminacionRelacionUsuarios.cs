using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class EliminacionRelacionUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TipoIncidentes_Usuarios_UsuarioId",
                table: "TipoIncidentes");

            migrationBuilder.DropIndex(
                name: "IX_TipoIncidentes_UsuarioId",
                table: "TipoIncidentes");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "TipoIncidentes");

            migrationBuilder.AlterColumn<int>(
                name: "Categoria",
                table: "TipoIncidentes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "TipoIncidentes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "TipoIncidentes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TipoIncidentes_UsuarioId",
                table: "TipoIncidentes",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_TipoIncidentes_Usuarios_UsuarioId",
                table: "TipoIncidentes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
