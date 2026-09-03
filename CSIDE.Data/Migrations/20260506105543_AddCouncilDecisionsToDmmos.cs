using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCouncilDecisionsToDmmos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dmmo_council_decision_types",
                schema: "cside",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_council_decision_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dmmo_council_decisions",
                schema: "cside",
                columns: table => new
                {
                    council_decision_id = table.Column<int>(type: "integer", nullable: false),
                    dmmo_application_id = table.Column<int>(type: "integer", nullable: false),
                    council_decision_type_id = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<LocalDate>(type: "date", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dmmo_council_decisions", x => new { x.council_decision_id, x.dmmo_application_id });
                    table.ForeignKey(
                        name: "fk_dmmo_council_decisions_dmmo_application_dmmo_application_id",
                        column: x => x.dmmo_application_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dmmo_council_decisions_dmmo_council_decision_types_council_",
                        column: x => x.council_decision_type_id,
                        principalSchema: "cside",
                        principalTable: "dmmo_council_decision_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_council_decisions_council_decision_type_id",
                schema: "cside",
                table: "dmmo_council_decisions",
                column: "council_decision_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_dmmo_council_decisions_dmmo_application_id",
                schema: "cside",
                table: "dmmo_council_decisions",
                column: "dmmo_application_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dmmo_council_decisions",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "dmmo_council_decision_types",
                schema: "cside");
        }
    }
}
