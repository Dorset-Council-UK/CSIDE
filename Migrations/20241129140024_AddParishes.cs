using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddParishes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParishId",
                schema: "cside",
                table: "MaintenanceJobs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobs_ParishId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "ParishId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_Parishes_ParishId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "ParishId",
                principalSchema: "cside",
                principalTable: "Parishes",
                principalColumn: "feature_serial_number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_Parishes_ParishId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceJobs_ParishId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropColumn(
                name: "ParishId",
                schema: "cside",
                table: "MaintenanceJobs");
        }
    }
}
