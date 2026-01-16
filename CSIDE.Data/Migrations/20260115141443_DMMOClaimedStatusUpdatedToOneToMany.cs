using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class DMMOClaimedStatusUpdatedToOneToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DMMOApplication_DMMOApplicationClaimedStatuses_ClaimedStatu~",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.DropIndex(
                name: "IX_DMMOApplication_ClaimedStatusId",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.DropColumn(
                name: "ClaimedStatusId",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.CreateTable(
                name: "DMMOClaimedStatuses",
                schema: "cside",
                columns: table => new
                {
                    DMMOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ClaimedStatusId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOClaimedStatuses", x => new { x.DMMOApplicationId, x.ClaimedStatusId });
                    table.ForeignKey(
                        name: "FK_DMMOClaimedStatuses_DMMOApplicationClaimedStatuses_ClaimedS~",
                        column: x => x.ClaimedStatusId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplicationClaimedStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOClaimedStatuses_DMMOApplication_DMMOApplicationId",
                        column: x => x.DMMOApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DMMOClaimedStatuses_ClaimedStatusId",
                schema: "cside",
                table: "DMMOClaimedStatuses",
                column: "ClaimedStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMMOClaimedStatuses",
                schema: "cside");

            migrationBuilder.AddColumn<int>(
                name: "ClaimedStatusId",
                schema: "cside",
                table: "DMMOApplication",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DMMOApplication_ClaimedStatusId",
                schema: "cside",
                table: "DMMOApplication",
                column: "ClaimedStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOApplication_DMMOApplicationClaimedStatuses_ClaimedStatu~",
                schema: "cside",
                table: "DMMOApplication",
                column: "ClaimedStatusId",
                principalSchema: "cside",
                principalTable: "DMMOApplicationClaimedStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
