using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class RespuestaInspeccionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RespuestaInspeccion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspeccionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Resultado = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EsNecesariaInspeccionAdicional = table.Column<bool>(type: "bit", nullable: false),
                    AccionesCorrectivas = table.Column<bool>(type: "bit", nullable: false),
                    AccionesCorrectivasLista = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DocumentacionCompleta = table.Column<bool>(type: "bit", nullable: false),
                    RecomendacionesFuturas = table.Column<bool>(type: "bit", nullable: false),
                    RecomendacionesFuturasList = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    InspeccionAdicionalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EstadoRespuesta = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespuestaInspeccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespuestaInspeccion_Inspeccion_InspeccionAdicionalId",
                        column: x => x.InspeccionAdicionalId,
                        principalTable: "Inspeccion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RespuestaInspeccion_Inspeccion_InspeccionId",
                        column: x => x.InspeccionId,
                        principalTable: "Inspeccion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            
            migrationBuilder.CreateIndex(
                name: "IX_RespuestaInspeccion_InspeccionAdicionalId",
                table: "RespuestaInspeccion",
                column: "InspeccionAdicionalId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestaInspeccion_InspeccionId",
                table: "RespuestaInspeccion",
                column: "InspeccionId");

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           ;

            migrationBuilder.DropTable(
                name: "RespuestaInspeccion");

         
        }
    }
}
