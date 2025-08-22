using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddDuplicateOfToMaintJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuplicateJobId",
                schema: "cside",
                table: "MaintenanceJobs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDuplicate",
                schema: "cside",
                table: "JobStatuses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuplicateJobId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropColumn(
                name: "IsDuplicate",
                schema: "cside",
                table: "JobStatuses");
        }
    }
}
