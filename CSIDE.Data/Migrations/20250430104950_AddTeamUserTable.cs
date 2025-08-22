using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaintenanceTeamUsers",
                schema: "cside",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    IsLead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTeamUsers", x => new { x.TeamId, x.UserId });
                    table.ForeignKey(
                        name: "FK_MaintenanceTeamUsers_MaintenanceTeams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaintenanceTeamUsers",
                schema: "cside");
        }
    }
}
