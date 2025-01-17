using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceTeamToInfrastructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaintenanceTeamId",
                schema: "cside",
                table: "Infrastructure",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Infrastructure_MaintenanceTeamId",
                schema: "cside",
                table: "Infrastructure",
                column: "MaintenanceTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Infrastructure_MaintenanceTeams_MaintenanceTeamId",
                schema: "cside",
                table: "Infrastructure",
                column: "MaintenanceTeamId",
                principalSchema: "cside",
                principalTable: "MaintenanceTeams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Infrastructure_MaintenanceTeams_MaintenanceTeamId",
                schema: "cside",
                table: "Infrastructure");

            migrationBuilder.DropIndex(
                name: "IX_Infrastructure_MaintenanceTeamId",
                schema: "cside",
                table: "Infrastructure");

            migrationBuilder.DropColumn(
                name: "MaintenanceTeamId",
                schema: "cside",
                table: "Infrastructure");
        }
    }
}
