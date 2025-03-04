using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddLandownerDepositMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LandownerDepositMedia",
                schema: "cside",
                columns: table => new
                {
                    LandownerDepositId = table.Column<int>(type: "integer", nullable: false),
                    MediaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositMedia", x => new { x.LandownerDepositId, x.MediaId });
                    table.ForeignKey(
                        name: "FK_LandownerDepositMedia_LandownerDeposits_LandownerDepositId",
                        column: x => x.LandownerDepositId,
                        principalSchema: "cside",
                        principalTable: "LandownerDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandownerDepositMedia_Media_MediaId",
                        column: x => x.MediaId,
                        principalSchema: "cside",
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositMedia_MediaId",
                schema: "cside",
                table: "LandownerDepositMedia",
                column: "MediaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LandownerDepositMedia",
                schema: "cside");
        }
    }
}
