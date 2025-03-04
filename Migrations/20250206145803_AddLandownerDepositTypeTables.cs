using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddLandownerDepositTypeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LandownerDepositTypeNames",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositTypeNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LandownerDepositType",
                schema: "cside",
                columns: table => new
                {
                    LandownerDepositId = table.Column<int>(type: "integer", nullable: false),
                    LandownerDepositTypeNameId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositType", x => new { x.LandownerDepositId, x.LandownerDepositTypeNameId });
                    table.ForeignKey(
                        name: "FK_LandownerDepositType_LandownerDepositTypeNames_LandownerDep~",
                        column: x => x.LandownerDepositTypeNameId,
                        principalSchema: "cside",
                        principalTable: "LandownerDepositTypeNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandownerDepositType_LandownerDeposits_LandownerDepositId",
                        column: x => x.LandownerDepositId,
                        principalSchema: "cside",
                        principalTable: "LandownerDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositType_LandownerDepositTypeNameId",
                schema: "cside",
                table: "LandownerDepositType",
                column: "LandownerDepositTypeNameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LandownerDepositType",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "LandownerDepositTypeNames",
                schema: "cside");
        }
    }
}
