using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddDMMOContacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMMOContact",
                schema: "cside",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ContactId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOContact", x => new { x.ApplicationId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_DMMOContact_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "cside",
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOContact_DMMOApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DMMOContact_ContactId",
                schema: "cside",
                table: "DMMOContact",
                column: "ContactId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMMOContact",
                schema: "cside");
        }
    }
}
