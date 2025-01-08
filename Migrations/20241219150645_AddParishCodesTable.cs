using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddParishCodesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParishCodes",
                schema: "cside",
                columns: table => new
                {
                    ParishId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParishCodes", x => new { x.ParishId, x.Code });
                    table.ForeignKey(
                        name: "FK_ParishCodes_Parishes_ParishId",
                        column: x => x.ParishId,
                        principalSchema: "cside",
                        principalTable: "Parishes",
                        principalColumn: "admin_unit_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParishCodes",
                schema: "cside");
        }
    }
}
