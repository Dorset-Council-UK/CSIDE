using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressesToLandownerDeposits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LandownerDepositAddresses",
                schema: "cside",
                columns: table => new
                {
                    LandownerDepositId = table.Column<int>(type: "integer", nullable: false),
                    UPRN = table.Column<long>(type: "bigint", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositAddresses", x => new { x.LandownerDepositId, x.UPRN });
                    table.ForeignKey(
                        name: "FK_LandownerDepositAddresses_LandownerDeposits_LandownerDeposi~",
                        column: x => x.LandownerDepositId,
                        principalSchema: "cside",
                        principalTable: "LandownerDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LandownerDepositAddresses",
                schema: "cside");
        }
    }
}
