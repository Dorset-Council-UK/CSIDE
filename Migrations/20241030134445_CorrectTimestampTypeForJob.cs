using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class CorrectTimestampTypeForJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "cside",
                table: "MaintenanceJobs",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                schema: "cside",
                table: "MaintenanceJobs",
                type: "bytea",
                rowVersion: true,
                nullable: true);
        }
    }
}
