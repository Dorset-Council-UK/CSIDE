using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaToSurveys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "SurveySequence",
                schema: "cside");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "cside",
                table: "BridgeSurveys",
                type: "integer",
                nullable: false,
                defaultValueSql: "nextval('cside.\"SurveySequence\"')",
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "SurveyMedia",
                schema: "cside",
                columns: table => new
                {
                    SurveyId = table.Column<int>(type: "integer", nullable: false),
                    MediaId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyMedia", x => new { x.SurveyId, x.MediaId });
                    table.ForeignKey(
                        name: "FK_SurveyMedia_Media_MediaId",
                        column: x => x.MediaId,
                        principalSchema: "cside",
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyMedia_MediaId",
                schema: "cside",
                table: "SurveyMedia",
                column: "MediaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyMedia",
                schema: "cside");

            migrationBuilder.DropSequence(
                name: "SurveySequence",
                schema: "cside");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "cside",
                table: "BridgeSurveys",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValueSql: "nextval('cside.\"SurveySequence\"')")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
