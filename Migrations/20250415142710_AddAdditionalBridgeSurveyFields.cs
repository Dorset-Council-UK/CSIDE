using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalBridgeSurveyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumDeckingTimbers",
                schema: "cside",
                table: "BridgeSurveys");

            migrationBuilder.RenameColumn(
                name: "NumHandrailTimbers",
                schema: "cside",
                table: "BridgeSurveys",
                newName: "NumDeckingBoards");

            migrationBuilder.RenameColumn(
                name: "DeckingTimbersSize",
                schema: "cside",
                table: "BridgeSurveys",
                newName: "DeckingBoardsSize");

            migrationBuilder.AddColumn<decimal>(
                name: "DeckingBoardsLength",
                schema: "cside",
                table: "BridgeSurveys",
                type: "numeric(3,1)",
                precision: 3,
                scale: 1,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HandrailsInPlace",
                schema: "cside",
                table: "BridgeSurveys",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeckingBoardsLength",
                schema: "cside",
                table: "BridgeSurveys");

            migrationBuilder.DropColumn(
                name: "HandrailsInPlace",
                schema: "cside",
                table: "BridgeSurveys");

            migrationBuilder.RenameColumn(
                name: "NumDeckingBoards",
                schema: "cside",
                table: "BridgeSurveys",
                newName: "NumHandrailTimbers");

            migrationBuilder.RenameColumn(
                name: "DeckingBoardsSize",
                schema: "cside",
                table: "BridgeSurveys",
                newName: "DeckingTimbersSize");

            migrationBuilder.AddColumn<int>(
                name: "NumDeckingTimbers",
                schema: "cside",
                table: "BridgeSurveys",
                type: "integer",
                nullable: true);
        }
    }
}
