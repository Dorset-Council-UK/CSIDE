using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddPPOData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PPOApplicationCaseStatuses",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOApplicationCaseStatuses", x => x.Id);
                });

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
                name: "PPOMediaType",
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
                    table.PrimaryKey("PK_PPOMediaType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PPOApplication",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationDate = table.Column<LocalDate>(type: "date", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicationDetails = table.Column<string>(type: "text", nullable: false),
                    LocationDescription = table.Column<string>(type: "text", nullable: true),
                    CaseOfficer = table.Column<string>(type: "text", nullable: true),
                    CaseOfficerUserId = table.Column<string>(type: "text", nullable: true),
                    PrivateComments = table.Column<string>(type: "text", nullable: true),
                    PublicComments = table.Column<string>(type: "text", nullable: true),
                    DeterminationDate = table.Column<LocalDate>(type: "date", nullable: true),
                    DateOfDirectionOfSecState = table.Column<LocalDate>(type: "date", nullable: true),
                    Geom = table.Column<MultiLineString>(type: "geometry (multilinestring)", nullable: false),
                    ApplicationTypeId = table.Column<int>(type: "integer", nullable: false),
                    CaseStatusId = table.Column<int>(type: "integer", nullable: false),
                    ClaimedStatusId = table.Column<int>(type: "integer", nullable: false),
                    DirectionOfSecStateId = table.Column<int>(type: "integer", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PPOApplication_PPOApplicationCaseStatuses_CaseStatusId",
                        column: x => x.CaseStatusId,
                        principalSchema: "cside",
                        principalTable: "PPOApplicationCaseStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPOApplication_PPOApplicationTypes_ApplicationTypeId",
                        column: x => x.ApplicationTypeId,
                        principalSchema: "cside",
                        principalTable: "PPOApplicationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PPOContact",
                schema: "cside",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ContactId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOContact", x => new { x.ApplicationId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_PPOContact_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "cside",
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPOContact_PPOApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "cside",
                        principalTable: "PPOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PPOMedia",
                schema: "cside",
                columns: table => new
                {
                    PPOId = table.Column<int>(type: "integer", nullable: false),
                    MediaId = table.Column<int>(type: "integer", nullable: false),
                    MediaTypeId = table.Column<int>(type: "integer", nullable: false),
                    PPOApplicationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOMedia", x => new { x.PPOId, x.MediaId });
                    table.ForeignKey(
                        name: "FK_PPOMedia_Media_MediaId",
                        column: x => x.MediaId,
                        principalSchema: "cside",
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPOMedia_PPOApplication_PPOApplicationId",
                        column: x => x.PPOApplicationId,
                        principalSchema: "cside",
                        principalTable: "PPOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPOMedia_PPOMediaType_MediaTypeId",
                        column: x => x.MediaTypeId,
                        principalSchema: "cside",
                        principalTable: "PPOMediaType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PPOOrders",
                schema: "cside",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ObjectionsEndDate = table.Column<LocalDate>(type: "date", nullable: true),
                    ObjectionsReceived = table.Column<bool>(type: "boolean", nullable: true),
                    ObjectionsWithdrawn = table.Column<bool>(type: "boolean", nullable: true),
                    DeterminationProcessId = table.Column<int>(type: "integer", nullable: true),
                    DecisionOfSecStateId = table.Column<int>(type: "integer", nullable: true),
                    DateConfirmed = table.Column<LocalDate>(type: "date", nullable: true),
                    DateSealed = table.Column<LocalDate>(type: "date", nullable: true),
                    DatePublished = table.Column<LocalDate>(type: "date", nullable: true),
                    SubmitToPINS = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOOrders", x => new { x.OrderId, x.ApplicationId });
                    table.ForeignKey(
                        name: "FK_PPOOrders_OrderDecisionsOfSecState_DecisionOfSecStateId",
                        column: x => x.DecisionOfSecStateId,
                        principalSchema: "cside",
                        principalTable: "OrderDecisionsOfSecState",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PPOOrders_OrderDeterminationProcesses_DeterminationProcessId",
                        column: x => x.DeterminationProcessId,
                        principalSchema: "cside",
                        principalTable: "OrderDeterminationProcesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PPOOrders_PPOApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "cside",
                        principalTable: "PPOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PPOParishes",
                schema: "cside",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ParishId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOParishes", x => new { x.ApplicationId, x.ParishId });
                    table.ForeignKey(
                        name: "FK_PPOParishes_PPOApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "cside",
                        principalTable: "PPOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPOParishes_Parishes_ParishId",
                        column: x => x.ParishId,
                        principalSchema: "cside",
                        principalTable: "Parishes",
                        principalColumn: "admin_unit_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PPOApplication_ApplicationTypeId",
                schema: "cside",
                table: "PPOApplication",
                column: "ApplicationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOApplication_CaseStatusId",
                schema: "cside",
                table: "PPOApplication",
                column: "CaseStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOContact_ContactId",
                schema: "cside",
                table: "PPOContact",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOMedia_MediaId",
                schema: "cside",
                table: "PPOMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOMedia_MediaTypeId",
                schema: "cside",
                table: "PPOMedia",
                column: "MediaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOMedia_PPOApplicationId",
                schema: "cside",
                table: "PPOMedia",
                column: "PPOApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOOrders_ApplicationId",
                schema: "cside",
                table: "PPOOrders",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOOrders_DecisionOfSecStateId",
                schema: "cside",
                table: "PPOOrders",
                column: "DecisionOfSecStateId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOOrders_DeterminationProcessId",
                schema: "cside",
                table: "PPOOrders",
                column: "DeterminationProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOParishes_ParishId",
                schema: "cside",
                table: "PPOParishes",
                column: "ParishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PPOContact",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOMedia",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOOrders",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOParishes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOMediaType",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOApplication",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOApplicationCaseStatuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOApplicationTypes",
                schema: "cside");
        }
    }
}
