using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class PPORenameApplicationIntentToApplicationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PPOIntents",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOApplicationIntents",
                schema: "cside");

            migrationBuilder.CreateTable(
                name: "PPOApplicationTypes",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOApplicationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PPOTypes",
                schema: "cside",
                columns: table => new
                {
                    PPOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    TypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOTypes", x => new { x.PPOApplicationId, x.TypeId });
                    table.ForeignKey(
                        name: "FK_PPOTypes_PPOApplicationTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "cside",
                        principalTable: "PPOApplicationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPOTypes_PPOApplication_PPOApplicationId",
                        column: x => x.PPOApplicationId,
                        principalSchema: "cside",
                        principalTable: "PPOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PPOTypes_TypeId",
                schema: "cside",
                table: "PPOTypes",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PPOTypes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOApplicationTypes",
                schema: "cside");

            migrationBuilder.CreateTable(
                name: "PPOApplicationIntents",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
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
                    PPOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    IntentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOIntents", x => new { x.PPOApplicationId, x.IntentId });
                    table.ForeignKey(
                        name: "FK_PPOIntents_PPOApplicationIntents_IntentId",
                        column: x => x.IntentId,
                        principalSchema: "cside",
                        principalTable: "PPOApplicationIntents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPOIntents_PPOApplication_PPOApplicationId",
                        column: x => x.PPOApplicationId,
                        principalSchema: "cside",
                        principalTable: "PPOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PPOIntents_IntentId",
                schema: "cside",
                table: "PPOIntents",
                column: "IntentId");
        }
    }
}
