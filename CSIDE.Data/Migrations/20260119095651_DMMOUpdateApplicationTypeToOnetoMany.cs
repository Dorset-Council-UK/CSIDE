using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class DMMOUpdateApplicationTypeToOnetoMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DMMOApplication_DMMOApplicationTypes_ApplicationTypeId",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.DropIndex(
                name: "IX_DMMOApplication_ApplicationTypeId",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.DropColumn(
                name: "ApplicationTypeId",
                schema: "cside",
                table: "DMMOApplication");

            migrationBuilder.CreateTable(
                name: "DMMOTypes",
                schema: "cside",
                columns: table => new
                {
                    DMMOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ApplicationTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOTypes", x => new { x.DMMOApplicationId, x.ApplicationTypeId });
                    table.ForeignKey(
                        name: "FK_DMMOTypes_DMMOApplicationTypes_ApplicationTypeId",
                        column: x => x.ApplicationTypeId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplicationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOTypes_DMMOApplication_DMMOApplicationId",
                        column: x => x.DMMOApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DMMOTypes_ApplicationTypeId",
                schema: "cside",
                table: "DMMOTypes",
                column: "ApplicationTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMMOTypes",
                schema: "cside");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationTypeId",
                schema: "cside",
                table: "DMMOApplication",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DMMOApplication_ApplicationTypeId",
                schema: "cside",
                table: "DMMOApplication",
                column: "ApplicationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DMMOApplication_DMMOApplicationTypes_ApplicationTypeId",
                schema: "cside",
                table: "DMMOApplication",
                column: "ApplicationTypeId",
                principalSchema: "cside",
                principalTable: "DMMOApplicationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
