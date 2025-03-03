using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddParishesToDMMO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMMOParish",
                schema: "cside",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ParishId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOParish", x => new { x.ApplicationId, x.ParishId });
                    table.ForeignKey(
                        name: "FK_DMMOParish_DMMOApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOParish_Parishes_ParishId",
                        column: x => x.ParishId,
                        principalSchema: "cside",
                        principalTable: "Parishes",
                        principalColumn: "admin_unit_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DMMOParish_ParishId",
                schema: "cside",
                table: "DMMOParish",
                column: "ParishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMMOParish",
                schema: "cside");
        }
    }
}
