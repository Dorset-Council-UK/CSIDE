using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaTypesToLandownerDeposits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "cside",
                table: "LandownerDeposits",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<int>(
                name: "MediaTypeId",
                schema: "cside",
                table: "LandownerDepositMedia",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LandownerDepositMediaTypes",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NumFilesLimit = table.Column<int>(type: "integer", nullable: false),
                    FileTypesLimit = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositMediaTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositMedia_MediaTypeId",
                schema: "cside",
                table: "LandownerDepositMedia",
                column: "MediaTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositMedia_LandownerDepositMediaTypes_MediaTypeId",
                schema: "cside",
                table: "LandownerDepositMedia",
                column: "MediaTypeId",
                principalSchema: "cside",
                principalTable: "LandownerDepositMediaTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositMedia_LandownerDepositMediaTypes_MediaTypeId",
                schema: "cside",
                table: "LandownerDepositMedia");

            migrationBuilder.DropTable(
                name: "LandownerDepositMediaTypes",
                schema: "cside");

            migrationBuilder.DropIndex(
                name: "IX_LandownerDepositMedia_MediaTypeId",
                schema: "cside",
                table: "LandownerDepositMedia");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "cside",
                table: "LandownerDeposits");

            migrationBuilder.DropColumn(
                name: "MediaTypeId",
                schema: "cside",
                table: "LandownerDepositMedia");
        }
    }
}
