using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameArchiveReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "box_number",
                schema: "cside",
                table: "ppo_application",
                newName: "internal_archive_reference_no");

            migrationBuilder.RenameColumn(
                name: "archive_reference",
                schema: "cside",
                table: "landowner_deposits",
                newName: "external_archive_reference_no");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "internal_archive_reference_no",
                schema: "cside",
                table: "ppo_application",
                newName: "box_number");

            migrationBuilder.RenameColumn(
                name: "external_archive_reference_no",
                schema: "cside",
                table: "landowner_deposits",
                newName: "archive_reference");
        }
    }
}
