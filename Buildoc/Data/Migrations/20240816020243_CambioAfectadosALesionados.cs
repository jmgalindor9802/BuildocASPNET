using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildoc.Data.Migrations
{
    /// <inheritdoc />
    public partial class CambioAfectadosALesionados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Afectados");

            migrationBuilder.CreateTable(
                name: "Lesionados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cedula = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lesionados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncidenteLesionados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LesionadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Defuncion = table.Column<bool>(type: "bit", nullable: false),
                    ActividadRealizada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AsociadaProyecto = table.Column<bool>(type: "bit", nullable: false),
                    GeneroAfectado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hospitalizado = table.Column<bool>(type: "bit", nullable: false),
                    PrimerosAuxilios = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidenteLesionados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidenteLesionados_Incidentes_IncidenteId",
                        column: x => x.IncidenteId,
                        principalTable: "Incidentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidenteLesionados_Lesionados_LesionadoId",
                        column: x => x.LesionadoId,
                        principalTable: "Lesionados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidenteLesionados_IncidenteId",
                table: "IncidenteLesionados",
                column: "IncidenteId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenteLesionados_LesionadoId",
                table: "IncidenteLesionados",
                column: "LesionadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidenteLesionados");

            migrationBuilder.DropTable(
                name: "Lesionados");

            migrationBuilder.CreateTable(
                name: "Afectados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AbrasionRasgunos = table.Column<bool>(type: "bit", nullable: false),
                    ActividadRealizada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amputacion = table.Column<bool>(type: "bit", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AsociadaProyecto = table.Column<bool>(type: "bit", nullable: false),
                    Cedula = table.Column<long>(type: "bigint", nullable: true),
                    ConmocionCerebral = table.Column<bool>(type: "bit", nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorteLaceracionPerforacion = table.Column<bool>(type: "bit", nullable: false),
                    Defuncion = table.Column<bool>(type: "bit", nullable: false),
                    EsguinceTension = table.Column<bool>(type: "bit", nullable: false),
                    GeneroAfectado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hernia = table.Column<bool>(type: "bit", nullable: false),
                    Hospitalizado = table.Column<bool>(type: "bit", nullable: false),
                    HuesosRotos = table.Column<bool>(type: "bit", nullable: false),
                    LesionAplastamiento = table.Column<bool>(type: "bit", nullable: false),
                    LesionDeLaMedulaEspinal = table.Column<bool>(type: "bit", nullable: false),
                    LesionOcular = table.Column<bool>(type: "bit", nullable: false),
                    Moreton = table.Column<bool>(type: "bit", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimerosAuxilios = table.Column<bool>(type: "bit", nullable: false),
                    QuemaduraCalor = table.Column<bool>(type: "bit", nullable: false),
                    QuemadurasQuimicas = table.Column<bool>(type: "bit", nullable: false),
                    TraumatismoCraneoencefalico = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Afectados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Afectados_Incidentes_IncidenteId",
                        column: x => x.IncidenteId,
                        principalTable: "Incidentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Afectados_IncidenteId",
                table: "Afectados",
                column: "IncidenteId");
        }
    }
}
