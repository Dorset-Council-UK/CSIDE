using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteStatementTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Statements",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RouteId = table.Column<string>(type: "text", nullable: false),
                    StatementText = table.Column<string>(type: "text", nullable: false),
                    StartGridRef = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EndGridRef = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statements_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "cside",
                        principalTable: "Routes",
                        principalColumn: "RouteCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Statements_RouteId",
                schema: "cside",
                table: "Statements",
                column: "RouteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Statements",
                schema: "cside");
        }
    }
}
