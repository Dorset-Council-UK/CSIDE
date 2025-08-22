using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddFurtherMaintJobDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompletionNotes",
                schema: "cside",
                table: "MaintenanceJobs",
                newName: "WorkDone");

            migrationBuilder.AddColumn<string>(
                name: "AssignedToTeamId",
                schema: "cside",
                table: "MaintenanceJobs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoggedById",
                schema: "cside",
                table: "MaintenanceJobs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoggedByName",
                schema: "cside",
                table: "MaintenanceJobs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceTeamId",
                schema: "cside",
                table: "MaintenanceJobs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RouteId",
                schema: "cside",
                table: "MaintenanceJobs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MaintenanceTeams",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTeams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                schema: "cside",
                columns: table => new
                {
                    RouteCode = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.RouteCode);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobs_MaintenanceTeamId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "MaintenanceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobs_RouteId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_MaintenanceTeams_MaintenanceTeamId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "MaintenanceTeamId",
                principalSchema: "cside",
                principalTable: "MaintenanceTeams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_Routes_RouteId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "RouteId",
                principalSchema: "cside",
                principalTable: "Routes",
                principalColumn: "RouteCode",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_MaintenanceTeams_MaintenanceTeamId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_Routes_RouteId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropTable(
                name: "MaintenanceTeams",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "Routes",
                schema: "cside");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceJobs_MaintenanceTeamId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceJobs_RouteId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropColumn(
                name: "AssignedToTeamId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropColumn(
                name: "LoggedById",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropColumn(
                name: "LoggedByName",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropColumn(
                name: "MaintenanceTeamId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropColumn(
                name: "RouteId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.RenameColumn(
                name: "WorkDone",
                schema: "cside",
                table: "MaintenanceJobs",
                newName: "CompletionNotes");
        }
    }
}
