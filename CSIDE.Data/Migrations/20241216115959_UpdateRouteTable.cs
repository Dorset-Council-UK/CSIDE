using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRouteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<LocalDate>(
                name: "ClosureEndDate",
                schema: "cside",
                table: "Routes",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<LocalDate>(
                name: "ClosureStartDate",
                schema: "cside",
                table: "Routes",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LegalStatusId",
                schema: "cside",
                table: "Routes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceTeamId",
                schema: "cside",
                table: "Routes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OperationalStatusId",
                schema: "cside",
                table: "Routes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParishId",
                schema: "cside",
                table: "Routes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteTypeId",
                schema: "cside",
                table: "Routes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RouteLegalStatuses",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteLegalStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteMedia",
                schema: "cside",
                columns: table => new
                {
                    RouteId = table.Column<string>(type: "text", nullable: false),
                    MediaId = table.Column<int>(type: "integer", nullable: false),
                    IsClosureNotificationDocument = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteMedia", x => new { x.RouteId, x.MediaId });
                    table.ForeignKey(
                        name: "FK_RouteMedia_Media_MediaId",
                        column: x => x.MediaId,
                        principalSchema: "cside",
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteMedia_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "cside",
                        principalTable: "Routes",
                        principalColumn: "RouteCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteOperationalStatuses",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteOperationalStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteTypes",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Routes_LegalStatusId",
                schema: "cside",
                table: "Routes",
                column: "LegalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_MaintenanceTeamId",
                schema: "cside",
                table: "Routes",
                column: "MaintenanceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OperationalStatusId",
                schema: "cside",
                table: "Routes",
                column: "OperationalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_ParishId",
                schema: "cside",
                table: "Routes",
                column: "ParishId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_RouteTypeId",
                schema: "cside",
                table: "Routes",
                column: "RouteTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteMedia_MediaId",
                schema: "cside",
                table: "RouteMedia",
                column: "MediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_MaintenanceTeams_MaintenanceTeamId",
                schema: "cside",
                table: "Routes",
                column: "MaintenanceTeamId",
                principalSchema: "cside",
                principalTable: "MaintenanceTeams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Parishes_ParishId",
                schema: "cside",
                table: "Routes",
                column: "ParishId",
                principalSchema: "cside",
                principalTable: "Parishes",
                principalColumn: "admin_unit_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_RouteLegalStatuses_LegalStatusId",
                schema: "cside",
                table: "Routes",
                column: "LegalStatusId",
                principalSchema: "cside",
                principalTable: "RouteLegalStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_RouteOperationalStatuses_OperationalStatusId",
                schema: "cside",
                table: "Routes",
                column: "OperationalStatusId",
                principalSchema: "cside",
                principalTable: "RouteOperationalStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_RouteTypes_RouteTypeId",
                schema: "cside",
                table: "Routes",
                column: "RouteTypeId",
                principalSchema: "cside",
                principalTable: "RouteTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_MaintenanceTeams_MaintenanceTeamId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Parishes_ParishId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_RouteLegalStatuses_LegalStatusId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_RouteOperationalStatuses_OperationalStatusId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_RouteTypes_RouteTypeId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropTable(
                name: "RouteLegalStatuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "RouteMedia",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "RouteOperationalStatuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "RouteTypes",
                schema: "cside");

            migrationBuilder.DropIndex(
                name: "IX_Routes_LegalStatusId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_MaintenanceTeamId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_OperationalStatusId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_ParishId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_RouteTypeId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "ClosureEndDate",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "ClosureStartDate",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "LegalStatusId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "MaintenanceTeamId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "OperationalStatusId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "ParishId",
                schema: "cside",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "RouteTypeId",
                schema: "cside",
                table: "Routes");
        }
    }
}
