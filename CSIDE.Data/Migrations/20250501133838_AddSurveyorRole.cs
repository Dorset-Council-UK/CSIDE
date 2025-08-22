using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyorRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "cside",
                table: "ApplicationRoles",
                columns: ["Id", "RoleName"],
                values: [7, "Surveyor"]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "cside",
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
