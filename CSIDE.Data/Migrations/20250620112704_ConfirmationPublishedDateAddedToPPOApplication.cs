using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class ConfirmationPublishedDateAddedToPPOApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<LocalDate>(
                name: "ConfirmationPublishedDate",
                schema: "cside",
                table: "PPOApplication",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmationPublishedDate",
                schema: "cside",
                table: "PPOApplication");
        }
    }
}
