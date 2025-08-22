using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddLandownerDeposit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LandownerDeposits",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReceivedDate = table.Column<LocalDate>(type: "date", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    WebsiteNoticePublished = table.Column<LocalDate>(type: "date", nullable: true),
                    EmailNoticeSent = table.Column<LocalDate>(type: "date", nullable: true),
                    OnsiteNoticeErected = table.Column<LocalDate>(type: "date", nullable: true),
                    IntendedEffect = table.Column<string>(type: "text", nullable: true),
                    ElapseDate = table.Column<LocalDate>(type: "date", nullable: true),
                    FormCompleted = table.Column<bool>(type: "boolean", nullable: true),
                    MapCorrect = table.Column<bool>(type: "boolean", nullable: true),
                    FeePaid = table.Column<bool>(type: "boolean", nullable: true),
                    AllSigned = table.Column<bool>(type: "boolean", nullable: true),
                    DateAcknowledged = table.Column<LocalDate>(type: "date", nullable: true),
                    ChequeReceiptNumber = table.Column<string>(type: "text", nullable: true),
                    ChequePaidInDate = table.Column<LocalDate>(type: "date", nullable: true),
                    NoticeDrafted = table.Column<LocalDate>(type: "date", nullable: true),
                    SentToArchive = table.Column<LocalDate>(type: "date", nullable: true),
                    ArchiveReference = table.Column<string>(type: "text", nullable: true),
                    WebsiteEntryAdded = table.Column<LocalDate>(type: "date", nullable: true),
                    Geom = table.Column<MultiPolygon>(type: "geometry (multipolygon)", nullable: false),
                    ParishId = table.Column<int>(type: "integer", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDeposits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LandownerDeposits_Parishes_ParishId",
                        column: x => x.ParishId,
                        principalSchema: "cside",
                        principalTable: "Parishes",
                        principalColumn: "admin_unit_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDeposits_ParishId",
                schema: "cside",
                table: "LandownerDeposits",
                column: "ParishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LandownerDeposits",
                schema: "cside");
        }
    }
}
