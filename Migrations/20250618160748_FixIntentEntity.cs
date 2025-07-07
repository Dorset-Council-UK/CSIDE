using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class FixIntentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPOIntents_PPOApplicationIntents_PPOApplicationIntentId",
                schema: "cside",
                table: "PPOIntents");

            migrationBuilder.DropIndex(
                name: "IX_PPOIntents_PPOApplicationIntentId",
                schema: "cside",
                table: "PPOIntents");

            migrationBuilder.DropColumn(
                name: "PPOApplicationIntentId",
                schema: "cside",
                table: "PPOIntents");

            migrationBuilder.CreateIndex(
                name: "IX_PPOIntents_IntentId",
                schema: "cside",
                table: "PPOIntents",
                column: "IntentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PPOIntents_PPOApplicationIntents_IntentId",
                schema: "cside",
                table: "PPOIntents",
                column: "IntentId",
                principalSchema: "cside",
                principalTable: "PPOApplicationIntents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPOIntents_PPOApplicationIntents_IntentId",
                schema: "cside",
                table: "PPOIntents");

            migrationBuilder.DropIndex(
                name: "IX_PPOIntents_IntentId",
                schema: "cside",
                table: "PPOIntents");

            migrationBuilder.AddColumn<int>(
                name: "PPOApplicationIntentId",
                schema: "cside",
                table: "PPOIntents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PPOIntents_PPOApplicationIntentId",
                schema: "cside",
                table: "PPOIntents",
                column: "PPOApplicationIntentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PPOIntents_PPOApplicationIntents_PPOApplicationIntentId",
                schema: "cside",
                table: "PPOIntents",
                column: "PPOApplicationIntentId",
                principalSchema: "cside",
                principalTable: "PPOApplicationIntents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
