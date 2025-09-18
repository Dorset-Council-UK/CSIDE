using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class RenameApplicationToDMMOApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DMMOAddresses_DMMOApplication_ApplicationId",
                schema: "cside",
                table: "DMMOAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_DMMOContact_DMMOApplication_ApplicationId",
                schema: "cside",
                table: "DMMOContact");

            migrationBuilder.DropForeignKey(
                name: "FK_DMMOEvents_DMMOApplication_ApplicationId",
                schema: "cside",
                table: "DMMOEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_DMMOLinkedRoutes_DMMOApplication_ApplicationId",
                schema: "cside",
                table: "DMMOLinkedRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_DMMOOrders_DMMOApplication_ApplicationId",
                schema: "cside",
                table: "DMMOOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_DMMOParish_DMMOApplication_ApplicationId",
                schema: "cside",
                table: "DMMOParish");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                schema: "cside",
                table: "DMMOParish",
                newName: "DMMOApplicationId");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                schema: "cside",
                table: "DMMOOrders",
                newName: "DMMOApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_DMMOOrders_ApplicationId",
                schema: "cside",
                table: "DMMOOrders",
                newName: "IX_DMMOOrders_DMMOApplicationId");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                schema: "cside",
                table: "DMMOLinkedRoutes",
                newName: "DMMOApplicationId");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                schema: "cside",
                table: "DMMOEvents",
                newName: "DMMOApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_DMMOEvents_ApplicationId",
                schema: "cside",
                table: "DMMOEvents",
                newName: "IX_DMMOEvents_DMMOApplicationId");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                schema: "cside",
                table: "DMMOContact",
                newName: "DMMOApplicationId");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                schema: "cside",
                table: "DMMOAddresses",
                newName: "DMMOApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOAddresses_DMMOApplication_DMMOApplicationId",
                schema: "cside",
                table: "DMMOAddresses",
                column: "DMMOApplicationId",
                principalSchema: "cside",
                principalTable: "DMMOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOContact_DMMOApplication_DMMOApplicationId",
                schema: "cside",
                table: "DMMOContact",
                column: "DMMOApplicationId",
                principalSchema: "cside",
                principalTable: "DMMOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOEvents_DMMOApplication_DMMOApplicationId",
                schema: "cside",
                table: "DMMOEvents",
                column: "DMMOApplicationId",
                principalSchema: "cside",
                principalTable: "DMMOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOLinkedRoutes_DMMOApplication_DMMOApplicationId",
                schema: "cside",
                table: "DMMOLinkedRoutes",
                column: "DMMOApplicationId",
                principalSchema: "cside",
                principalTable: "DMMOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOOrders_DMMOApplication_DMMOApplicationId",
                schema: "cside",
                table: "DMMOOrders",
                column: "DMMOApplicationId",
                principalSchema: "cside",
                principalTable: "DMMOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOParish_DMMOApplication_DMMOApplicationId",
                schema: "cside",
                table: "DMMOParish",
                column: "DMMOApplicationId",
                principalSchema: "cside",
                principalTable: "DMMOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DMMOAddresses_DMMOApplication_DMMOApplicationId",
                schema: "cside",
                table: "DMMOAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_DMMOContact_DMMOApplication_DMMOApplicationId",
                schema: "cside",
                table: "DMMOContact");

            migrationBuilder.DropForeignKey(
                name: "FK_DMMOEvents_DMMOApplication_DMMOApplicationId",
                schema: "cside",
                table: "DMMOEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_DMMOLinkedRoutes_DMMOApplication_DMMOApplicationId",
                schema: "cside",
                table: "DMMOLinkedRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_DMMOOrders_DMMOApplication_DMMOApplicationId",
                schema: "cside",
                table: "DMMOOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_DMMOParish_DMMOApplication_DMMOApplicationId",
                schema: "cside",
                table: "DMMOParish");

            migrationBuilder.RenameColumn(
                name: "DMMOApplicationId",
                schema: "cside",
                table: "DMMOParish",
                newName: "ApplicationId");

            migrationBuilder.RenameColumn(
                name: "DMMOApplicationId",
                schema: "cside",
                table: "DMMOOrders",
                newName: "ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_DMMOOrders_DMMOApplicationId",
                schema: "cside",
                table: "DMMOOrders",
                newName: "IX_DMMOOrders_ApplicationId");

            migrationBuilder.RenameColumn(
                name: "DMMOApplicationId",
                schema: "cside",
                table: "DMMOLinkedRoutes",
                newName: "ApplicationId");

            migrationBuilder.RenameColumn(
                name: "DMMOApplicationId",
                schema: "cside",
                table: "DMMOEvents",
                newName: "ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_DMMOEvents_DMMOApplicationId",
                schema: "cside",
                table: "DMMOEvents",
                newName: "IX_DMMOEvents_ApplicationId");

            migrationBuilder.RenameColumn(
                name: "DMMOApplicationId",
                schema: "cside",
                table: "DMMOContact",
                newName: "ApplicationId");

            migrationBuilder.RenameColumn(
                name: "DMMOApplicationId",
                schema: "cside",
                table: "DMMOAddresses",
                newName: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOAddresses_DMMOApplication_ApplicationId",
                schema: "cside",
                table: "DMMOAddresses",
                column: "ApplicationId",
                principalSchema: "cside",
                principalTable: "DMMOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOContact_DMMOApplication_ApplicationId",
                schema: "cside",
                table: "DMMOContact",
                column: "ApplicationId",
                principalSchema: "cside",
                principalTable: "DMMOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOEvents_DMMOApplication_ApplicationId",
                schema: "cside",
                table: "DMMOEvents",
                column: "ApplicationId",
                principalSchema: "cside",
                principalTable: "DMMOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOLinkedRoutes_DMMOApplication_ApplicationId",
                schema: "cside",
                table: "DMMOLinkedRoutes",
                column: "ApplicationId",
                principalSchema: "cside",
                principalTable: "DMMOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOOrders_DMMOApplication_ApplicationId",
                schema: "cside",
                table: "DMMOOrders",
                column: "ApplicationId",
                principalSchema: "cside",
                principalTable: "DMMOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOParish_DMMOApplication_ApplicationId",
                schema: "cside",
                table: "DMMOParish",
                column: "ApplicationId",
                principalSchema: "cside",
                principalTable: "DMMOApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
