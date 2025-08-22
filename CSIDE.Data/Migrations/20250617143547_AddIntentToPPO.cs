using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddIntentToPPO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PPOApplicationIntents",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOApplicationIntents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PPOIntents",
                schema: "cside",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    IntentId = table.Column<int>(type: "integer", nullable: false),
                    PPOApplicationIntentId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOIntents", x => new { x.ApplicationId, x.IntentId });
                    table.ForeignKey(
                        name: "FK_PPOIntents_PPOApplicationIntents_PPOApplicationIntentId",
                        column: x => x.PPOApplicationIntentId,
                        principalSchema: "cside",
                        principalTable: "PPOApplicationIntents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPOIntents_PPOApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "cside",
                        principalTable: "PPOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PPOIntents_PPOApplicationIntentId",
                schema: "cside",
                table: "PPOIntents",
                column: "PPOApplicationIntentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PPOIntents",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOApplicationIntents",
                schema: "cside");
        }
    }
}
