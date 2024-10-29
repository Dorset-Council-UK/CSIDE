using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAssignedToTeamIdTypeOfJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedToTeamId",
                schema: "cside",
                table: "MaintenanceJobs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedToTeamId",
                schema: "cside",
                table: "MaintenanceJobs",
                type: "text",
                nullable: true);
        }
    }
}
