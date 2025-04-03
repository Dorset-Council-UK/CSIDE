using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddViewToApplicationRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "cside",
                table: "ApplicationRoles",
                columns: new[] { "Id", "RoleName" },
                values: new object[] { 6, "View" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "cside",
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
