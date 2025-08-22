using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddContactsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactTypes",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PrimaryContactNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SecondaryContactNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    OrganisationName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ContactTypeId = table.Column<int>(type: "integer", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_ContactTypes_ContactTypeId",
                        column: x => x.ContactTypeId,
                        principalSchema: "cside",
                        principalTable: "ContactTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobContact",
                schema: "cside",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    ContactId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobContact", x => new { x.JobId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_JobContact_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "cside",
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobContact_MaintenanceJobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ContactTypeId",
                schema: "cside",
                table: "Contacts",
                column: "ContactTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobContact_ContactId",
                schema: "cside",
                table: "JobContact",
                column: "ContactId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobContact",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "Contacts",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ContactTypes",
                schema: "cside");
        }
    }
}
