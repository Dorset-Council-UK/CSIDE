using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddInfrastructureTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InfrastructureTypes",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfrastructureTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Infrastructure",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    InstallationDate = table.Column<LocalDate>(type: "date", nullable: true),
                    Height = table.Column<double>(type: "double precision", nullable: true),
                    Width = table.Column<double>(type: "double precision", nullable: true),
                    Length = table.Column<double>(type: "double precision", nullable: true),
                    Geom = table.Column<Point>(type: "geometry (point)", nullable: false),
                    RouteId = table.Column<string>(type: "text", nullable: true),
                    InfrastructureTypeId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infrastructure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Infrastructure_InfrastructureTypes_InfrastructureTypeId",
                        column: x => x.InfrastructureTypeId,
                        principalSchema: "cside",
                        principalTable: "InfrastructureTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Infrastructure_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "cside",
                        principalTable: "Routes",
                        principalColumn: "RouteCode");
                });

            migrationBuilder.CreateTable(
                name: "JobInfrastructure",
                schema: "cside",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    InfrastructureId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobInfrastructure", x => new { x.JobId, x.InfrastructureId });
                    table.ForeignKey(
                        name: "FK_JobInfrastructure_Infrastructure_InfrastructureId",
                        column: x => x.InfrastructureId,
                        principalSchema: "cside",
                        principalTable: "Infrastructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobInfrastructure_MaintenanceJobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Infrastructure_InfrastructureTypeId",
                schema: "cside",
                table: "Infrastructure",
                column: "InfrastructureTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Infrastructure_RouteId",
                schema: "cside",
                table: "Infrastructure",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_JobInfrastructure_InfrastructureId",
                schema: "cside",
                table: "JobInfrastructure",
                column: "InfrastructureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobInfrastructure",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "Infrastructure",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "InfrastructureTypes",
                schema: "cside");
        }
    }
}
