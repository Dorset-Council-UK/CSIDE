using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class RenameApplicationToPPOApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPOContact_PPOApplication_ApplicationId",
                schema: "cside",
                table: "PPOContact");

            migrationBuilder.DropForeignKey(
                name: "FK_PPOEvents_PPOApplication_ApplicationId",
                schema: "cside",
                table: "PPOEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_PPOIntents_PPOApplication_ApplicationId",
                schema: "cside",
                table: "PPOIntents");

            migrationBuilder.DropForeignKey(
                name: "FK_PPOOrders_PPOApplication_ApplicationId",
                schema: "cside",
                table: "PPOOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PPOParishes_PPOApplication_ApplicationId",
                schema: "cside",
                table: "PPOParishes");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                schema: "cside",
                table: "PPOParishes",
                newName: "PPOApplicationId");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                schema: "cside",
                table: "PPOOrders",
                newName: "PPOApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_PPOOrders_ApplicationId",
                schema: "cside",
                table: "PPOOrders",
                newName: "IX_PPOOrders_PPOApplicationId");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                schema: "cside",
                table: "PPOIntents",
                newName: "PPOApplicationId");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                schema: "cside",
                table: "PPOEvents",
                newName: "PPOApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_PPOEvents_ApplicationId",
                schema: "cside",
                table: "PPOEvents",
                newName: "IX_PPOEvents_PPOApplicationId");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                schema: "cside",
                table: "PPOContact",
                newName: "PPOApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PPOContact_PPOApplication_PPOApplicationId",
                schema: "cside",
                table: "PPOContact",
                column: "PPOApplicationId",
                principalSchema: "cside",
                principalTable: "PPOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PPOEvents_PPOApplication_PPOApplicationId",
                schema: "cside",
                table: "PPOEvents",
                column: "PPOApplicationId",
                principalSchema: "cside",
                principalTable: "PPOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PPOIntents_PPOApplication_PPOApplicationId",
                schema: "cside",
                table: "PPOIntents",
                column: "PPOApplicationId",
                principalSchema: "cside",
                principalTable: "PPOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PPOOrders_PPOApplication_PPOApplicationId",
                schema: "cside",
                table: "PPOOrders",
                column: "PPOApplicationId",
                principalSchema: "cside",
                principalTable: "PPOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PPOParishes_PPOApplication_PPOApplicationId",
                schema: "cside",
                table: "PPOParishes",
                column: "PPOApplicationId",
                principalSchema: "cside",
                principalTable: "PPOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPOContact_PPOApplication_PPOApplicationId",
                schema: "cside",
                table: "PPOContact");

            migrationBuilder.DropForeignKey(
                name: "FK_PPOEvents_PPOApplication_PPOApplicationId",
                schema: "cside",
                table: "PPOEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_PPOIntents_PPOApplication_PPOApplicationId",
                schema: "cside",
                table: "PPOIntents");

            migrationBuilder.DropForeignKey(
                name: "FK_PPOOrders_PPOApplication_PPOApplicationId",
                schema: "cside",
                table: "PPOOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PPOParishes_PPOApplication_PPOApplicationId",
                schema: "cside",
                table: "PPOParishes");

            migrationBuilder.RenameColumn(
                name: "PPOApplicationId",
                schema: "cside",
                table: "PPOParishes",
                newName: "ApplicationId");

            migrationBuilder.RenameColumn(
                name: "PPOApplicationId",
                schema: "cside",
                table: "PPOOrders",
                newName: "ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_PPOOrders_PPOApplicationId",
                schema: "cside",
                table: "PPOOrders",
                newName: "IX_PPOOrders_ApplicationId");

            migrationBuilder.RenameColumn(
                name: "PPOApplicationId",
                schema: "cside",
                table: "PPOIntents",
                newName: "ApplicationId");

            migrationBuilder.RenameColumn(
                name: "PPOApplicationId",
                schema: "cside",
                table: "PPOEvents",
                newName: "ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_PPOEvents_PPOApplicationId",
                schema: "cside",
                table: "PPOEvents",
                newName: "IX_PPOEvents_ApplicationId");

            migrationBuilder.RenameColumn(
                name: "PPOApplicationId",
                schema: "cside",
                table: "PPOContact",
                newName: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PPOContact_PPOApplication_ApplicationId",
                schema: "cside",
                table: "PPOContact",
                column: "ApplicationId",
                principalSchema: "cside",
                principalTable: "PPOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PPOEvents_PPOApplication_ApplicationId",
                schema: "cside",
                table: "PPOEvents",
                column: "ApplicationId",
                principalSchema: "cside",
                principalTable: "PPOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PPOIntents_PPOApplication_ApplicationId",
                schema: "cside",
                table: "PPOIntents",
                column: "ApplicationId",
                principalSchema: "cside",
                principalTable: "PPOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PPOOrders_PPOApplication_ApplicationId",
                schema: "cside",
                table: "PPOOrders",
                column: "ApplicationId",
                principalSchema: "cside",
                principalTable: "PPOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PPOParishes_PPOApplication_ApplicationId",
                schema: "cside",
                table: "PPOParishes",
                column: "ApplicationId",
                principalSchema: "cside",
                principalTable: "PPOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
