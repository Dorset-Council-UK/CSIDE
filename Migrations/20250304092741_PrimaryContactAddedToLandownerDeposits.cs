using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class PrimaryContactAddedToLandownerDeposits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrimaryContact",
                schema: "cside",
                table: "LandownerDeposits",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryContactUserId",
                schema: "cside",
                table: "LandownerDeposits",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryContact",
                schema: "cside",
                table: "LandownerDeposits");

            migrationBuilder.DropColumn(
                name: "PrimaryContactUserId",
                schema: "cside",
                table: "LandownerDeposits");
        }
    }
}
