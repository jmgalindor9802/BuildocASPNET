using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProyectoCoordinadorResidentes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoordinadorId",
                table: "Proyectos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProyectoResidentes",
                columns: table => new
                {
                    ProyectosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResidentesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoResidentes", x => new { x.ProyectosId, x.ResidentesId });
                    table.ForeignKey(
                        name: "FK_ProyectoResidentes_Proyectos_ProyectosId",
                        column: x => x.ProyectosId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProyectoResidentes_Usuarios_ResidentesId",
                        column: x => x.ResidentesId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Proyectos_CoordinadorId",
                table: "Proyectos",
                column: "CoordinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoResidentes_ResidentesId",
                table: "ProyectoResidentes",
                column: "ResidentesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proyectos_Usuarios_CoordinadorId",
                table: "Proyectos",
                column: "CoordinadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proyectos_Usuarios_CoordinadorId",
                table: "Proyectos");

            migrationBuilder.DropTable(
                name: "ProyectoResidentes");

            migrationBuilder.DropIndex(
                name: "IX_Proyectos_CoordinadorId",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "CoordinadorId",
                table: "Proyectos");
        }
    }
}
