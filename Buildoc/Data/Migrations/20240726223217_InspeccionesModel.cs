using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class InspeccionesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inspeccion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaInspeccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Objetivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoInspeccionId = table.Column<int>(type: "int", nullable: false),
                    ProyectoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Resultado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspeccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inspeccion_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inspeccion_TipoInspeccion_TipoInspeccionId",
                        column: x => x.TipoInspeccionId,
                        principalTable: "TipoInspeccion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inspeccion_Usuarios_InspectorId",
                        column: x => x.InspectorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inspeccion_InspectorId",
                table: "Inspeccion",
                column: "InspectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspeccion_ProyectoId",
                table: "Inspeccion",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspeccion_TipoInspeccionId",
                table: "Inspeccion",
                column: "TipoInspeccionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inspeccion");
        }
    }
}
