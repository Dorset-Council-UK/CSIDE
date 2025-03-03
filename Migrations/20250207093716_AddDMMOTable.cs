using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddDMMOTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "DMMOApplicationCaseStatuses",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOApplicationCaseStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DMMOApplicationClaimedStatuses",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOApplicationClaimedStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DMMOApplicationTypes",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOApplicationTypes", x => x.Id);
                });


            migrationBuilder.CreateTable(
                name: "DMMOApplication",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationDate = table.Column<LocalDate>(type: "date", nullable: false),
                    ReceivedDate = table.Column<LocalDate>(type: "date", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicationDetails = table.Column<string>(type: "text", nullable: false),
                    LocationDescription = table.Column<string>(type: "text", nullable: true),
                    PrimaryContact = table.Column<string>(type: "text", nullable: true),
                    PrimaryContactUserId = table.Column<string>(type: "text", nullable: true),
                    CaseOfficer = table.Column<string>(type: "text", nullable: true),
                    CaseOfficerUserId = table.Column<string>(type: "text", nullable: true),
                    PrivateComments = table.Column<string>(type: "text", nullable: true),
                    PublicComments = table.Column<string>(type: "text", nullable: true),
                    ApplicationTypeId = table.Column<int>(type: "integer", nullable: false),
                    CaseStatusId = table.Column<int>(type: "integer", nullable: false),
                    ClaimedStatusId = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DMMOApplication_DMMOApplicationCaseStatuses_CaseStatusId",
                        column: x => x.CaseStatusId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplicationCaseStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOApplication_DMMOApplicationClaimedStatuses_ClaimedStatu~",
                        column: x => x.ClaimedStatusId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplicationClaimedStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOApplication_DMMOApplicationTypes_ApplicationTypeId",
                        column: x => x.ApplicationTypeId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplicationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DMMOApplication_ApplicationTypeId",
                schema: "cside",
                table: "DMMOApplication",
                column: "ApplicationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOApplication_CaseStatusId",
                schema: "cside",
                table: "DMMOApplication",
                column: "CaseStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOApplication_ClaimedStatusId",
                schema: "cside",
                table: "DMMOApplication",
                column: "ClaimedStatusId");

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropTable(
                name: "DMMOApplication",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOApplicationCaseStatuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOApplicationClaimedStatuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOApplicationTypes",
                schema: "cside");
        }
    }
}
