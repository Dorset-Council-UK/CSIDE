using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddLandownerDepositContacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LandownerDepositContacts",
                schema: "cside",
                columns: table => new
                {
                    LandownerDepositId = table.Column<int>(type: "integer", nullable: false),
                    ContactId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositContacts", x => new { x.LandownerDepositId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_LandownerDepositContacts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "cside",
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandownerDepositContacts_LandownerDeposits_LandownerDeposit~",
                        column: x => x.LandownerDepositId,
                        principalSchema: "cside",
                        principalTable: "LandownerDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositContacts_ContactId",
                schema: "cside",
                table: "LandownerDepositContacts",
                column: "ContactId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LandownerDepositContacts",
                schema: "cside");
        }
    }
}
