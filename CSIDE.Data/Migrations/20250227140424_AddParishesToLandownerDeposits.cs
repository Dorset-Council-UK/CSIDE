using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddParishesToLandownerDeposits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDeposits_Parishes_ParishId",
                schema: "cside",
                table: "LandownerDeposits");

            migrationBuilder.DropIndex(
                name: "IX_LandownerDeposits_ParishId",
                schema: "cside",
                table: "LandownerDeposits");

            migrationBuilder.DropColumn(
                name: "ParishId",
                schema: "cside",
                table: "LandownerDeposits");

            migrationBuilder.CreateTable(
                name: "LandownerDepositParishes",
                schema: "cside",
                columns: table => new
                {
                    LandownerDepositId = table.Column<int>(type: "integer", nullable: false),
                    ParishId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositParishes", x => new { x.LandownerDepositId, x.ParishId });
                    table.ForeignKey(
                        name: "FK_LandownerDepositParishes_LandownerDeposits_LandownerDeposit~",
                        column: x => x.LandownerDepositId,
                        principalSchema: "cside",
                        principalTable: "LandownerDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandownerDepositParishes_Parishes_ParishId",
                        column: x => x.ParishId,
                        principalSchema: "cside",
                        principalTable: "Parishes",
                        principalColumn: "admin_unit_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositParishes_ParishId",
                schema: "cside",
                table: "LandownerDepositParishes",
                column: "ParishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LandownerDepositParishes",
                schema: "cside");

            migrationBuilder.AddColumn<int>(
                name: "ParishId",
                schema: "cside",
                table: "LandownerDeposits",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDeposits_ParishId",
                schema: "cside",
                table: "LandownerDeposits",
                column: "ParishId");

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDeposits_Parishes_ParishId",
                schema: "cside",
                table: "LandownerDeposits",
                column: "ParishId",
                principalSchema: "cside",
                principalTable: "Parishes",
                principalColumn: "admin_unit_id");
        }
    }
}
