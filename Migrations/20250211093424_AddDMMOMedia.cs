using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddDMMOMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<LocalDate>(
                name: "ReceivedDate",
                schema: "cside",
                table: "DMMOApplication",
                type: "date",
                nullable: true,
                oldClrType: typeof(LocalDate),
                oldType: "date");

            migrationBuilder.AlterColumn<LocalDate>(
                name: "ApplicationDate",
                schema: "cside",
                table: "DMMOApplication",
                type: "date",
                nullable: true,
                oldClrType: typeof(LocalDate),
                oldType: "date");

            migrationBuilder.CreateTable(
                name: "DMMOMediaType",
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
                    table.PrimaryKey("PK_DMMOMediaType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DMMOMedia",
                schema: "cside",
                columns: table => new
                {
                    DMMOId = table.Column<int>(type: "integer", nullable: false),
                    MediaId = table.Column<int>(type: "integer", nullable: false),
                    MediaTypeId = table.Column<int>(type: "integer", nullable: false),
                    DMMOApplicationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOMedia", x => new { x.DMMOId, x.MediaId });
                    table.ForeignKey(
                        name: "FK_DMMOMedia_DMMOApplication_DMMOApplicationId",
                        column: x => x.DMMOApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOMedia_DMMOMediaType_MediaTypeId",
                        column: x => x.MediaTypeId,
                        principalSchema: "cside",
                        principalTable: "DMMOMediaType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOMedia_Media_MediaId",
                        column: x => x.MediaId,
                        principalSchema: "cside",
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DMMOMedia_DMMOApplicationId",
                schema: "cside",
                table: "DMMOMedia",
                column: "DMMOApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOMedia_MediaId",
                schema: "cside",
                table: "DMMOMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOMedia_MediaTypeId",
                schema: "cside",
                table: "DMMOMedia",
                column: "MediaTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMMOMedia",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOMediaType",
                schema: "cside");

            migrationBuilder.AlterColumn<LocalDate>(
                name: "ReceivedDate",
                schema: "cside",
                table: "DMMOApplication",
                type: "date",
                nullable: false,
                defaultValue: new NodaTime.LocalDate(1, 1, 1),
                oldClrType: typeof(LocalDate),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<LocalDate>(
                name: "ApplicationDate",
                schema: "cside",
                table: "DMMOApplication",
                type: "date",
                nullable: false,
                defaultValue: new NodaTime.LocalDate(1, 1, 1),
                oldClrType: typeof(LocalDate),
                oldType: "date",
                oldNullable: true);
        }
    }
}
