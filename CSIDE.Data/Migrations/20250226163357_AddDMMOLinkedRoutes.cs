using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddDMMOLinkedRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMMOLinkedRoutes",
                schema: "cside",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    RouteId = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOLinkedRoutes", x => new { x.ApplicationId, x.RouteId });
                    table.ForeignKey(
                        name: "FK_DMMOLinkedRoutes_DMMOApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOLinkedRoutes_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "cside",
                        principalTable: "Routes",
                        principalColumn: "RouteCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DMMOLinkedRoutes_RouteId",
                schema: "cside",
                table: "DMMOLinkedRoutes",
                column: "RouteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMMOLinkedRoutes",
                schema: "cside");
        }
    }
}
