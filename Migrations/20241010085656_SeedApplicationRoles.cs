using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class SeedApplicationRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "cside",
                table: "ApplicationRoles",
                columns: new[] { "Id", "RoleName" },
                values: new object[,]
                {
                    { 1, "Administrator" },
                    { 2, "Ranger" },
                    { 3, "RoW Officer" },
                    { 4, "Survey Validator" },
                    { 5, "RoW Statement Editor" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "cside",
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "cside",
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "cside",
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "cside",
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "cside",
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
