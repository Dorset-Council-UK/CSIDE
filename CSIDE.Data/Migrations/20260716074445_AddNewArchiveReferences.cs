using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewArchiveReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "external_archive_reference_no",
                schema: "cside",
                table: "ppo_application",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "internal_archive_reference_no",
                schema: "cside",
                table: "landowner_deposits",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "external_archive_reference_no",
                schema: "cside",
                table: "dmmo_application",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "internal_archive_reference_no",
                schema: "cside",
                table: "dmmo_application",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "external_archive_reference_no",
                schema: "cside",
                table: "ppo_application");

            migrationBuilder.DropColumn(
                name: "internal_archive_reference_no",
                schema: "cside",
                table: "landowner_deposits");

            migrationBuilder.DropColumn(
                name: "external_archive_reference_no",
                schema: "cside",
                table: "dmmo_application");

            migrationBuilder.DropColumn(
                name: "internal_archive_reference_no",
                schema: "cside",
                table: "dmmo_application");
        }
    }
}
