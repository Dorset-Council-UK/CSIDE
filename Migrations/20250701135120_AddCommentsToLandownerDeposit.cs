using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentsToLandownerDeposit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LandownerDepositComments",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LandownerDepositId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CommentDate = table.Column<LocalDate>(type: "date", nullable: false),
                    CommentText = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: true),
                    AuthorName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LandownerDepositComments_LandownerDeposits_LandownerDeposit~",
                        column: x => x.LandownerDepositId,
                        principalSchema: "cside",
                        principalTable: "LandownerDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositComments_LandownerDepositId",
                schema: "cside",
                table: "LandownerDepositComments",
                column: "LandownerDepositId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LandownerDepositComments",
                schema: "cside");
        }
    }
}
