using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentsToDMMO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMMOComments",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CommentDate = table.Column<LocalDate>(type: "date", nullable: false),
                    CommentText = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: true),
                    AuthorName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DMMOComments_DMMOApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DMMOComments_ApplicationId",
                schema: "cside",
                table: "DMMOComments",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMMOComments",
                schema: "cside");
        }
    }
}
