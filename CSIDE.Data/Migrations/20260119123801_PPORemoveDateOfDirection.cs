using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class PPORemoveDateOfDirection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfDirection",
                schema: "cside",
                table: "PPOApplication");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<LocalDate>(
                name: "DateOfDirection",
                schema: "cside",
                table: "PPOApplication",
                type: "date",
                nullable: true);
        }
    }
}
