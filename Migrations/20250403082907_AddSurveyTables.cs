using CSIDE.Data.Models.Surveys;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:survey_status", "completed,incomplete,rejected,verified")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Conditions",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BridgeSurveys",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InfrastructureItemId = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    Width = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    Length = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    BeamConditionId = table.Column<int>(type: "integer", nullable: true),
                    DeckingConditionId = table.Column<int>(type: "integer", nullable: true),
                    HandrailConditionId = table.Column<int>(type: "integer", nullable: true),
                    HandrailPostsConditionId = table.Column<int>(type: "integer", nullable: true),
                    BankSeatConditionId = table.Column<int>(type: "integer", nullable: true),
                    BeamMaterialId = table.Column<int>(type: "integer", nullable: true),
                    DeckingMaterialId = table.Column<int>(type: "integer", nullable: true),
                    HandrailMaterialId = table.Column<int>(type: "integer", nullable: true),
                    HandrailPostsMaterialId = table.Column<int>(type: "integer", nullable: true),
                    BankSeatMaterialId = table.Column<int>(type: "integer", nullable: true),
                    NumBeamTimbers = table.Column<int>(type: "integer", nullable: true),
                    NumDeckingTimbers = table.Column<int>(type: "integer", nullable: true),
                    NumHandrailTimbers = table.Column<int>(type: "integer", nullable: true),
                    NumHandrailPostsTimbers = table.Column<int>(type: "integer", nullable: true),
                    BeamTimbersSize = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DeckingTimbersSize = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    HandrailTimbersSize = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    HandrailPostsTimbersSize = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Overgrown = table.Column<bool>(type: "boolean", nullable: true),
                    SignsOfBankErosion = table.Column<bool>(type: "boolean", nullable: true),
                    SeriouslyEroded = table.Column<bool>(type: "boolean", nullable: true),
                    HighUsage = table.Column<bool>(type: "boolean", nullable: true),
                    CoverBoardsInPlace = table.Column<bool>(type: "boolean", nullable: true),
                    RampInstalled = table.Column<bool>(type: "boolean", nullable: true),
                    StepsInstalled = table.Column<bool>(type: "boolean", nullable: true),
                    AntiSlipInstalled = table.Column<bool>(type: "boolean", nullable: true),
                    GateInstalled = table.Column<bool>(type: "boolean", nullable: true),
                    StileInstalled = table.Column<bool>(type: "boolean", nullable: true),
                    UpdatedX = table.Column<double>(type: "double precision", nullable: true),
                    UpdatedY = table.Column<double>(type: "double precision", nullable: true),
                    LocationAccuracy = table.Column<int>(type: "integer", nullable: true),
                    StartDate = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EndDate = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<SurveyStatus>(type: "cside.survey_status", nullable: false, defaultValue: SurveyStatus.Incomplete),
                    SurveyorId = table.Column<string>(type: "text", nullable: true),
                    SurveyorName = table.Column<string>(type: "text", nullable: true),
                    RepairsRequired = table.Column<string>(type: "text", nullable: true),
                    ValidationNotes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BridgeSurveys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BridgeSurveys_Conditions_BankSeatConditionId",
                        column: x => x.BankSeatConditionId,
                        principalSchema: "cside",
                        principalTable: "Conditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BridgeSurveys_Conditions_BeamConditionId",
                        column: x => x.BeamConditionId,
                        principalSchema: "cside",
                        principalTable: "Conditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BridgeSurveys_Conditions_DeckingConditionId",
                        column: x => x.DeckingConditionId,
                        principalSchema: "cside",
                        principalTable: "Conditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BridgeSurveys_Conditions_HandrailConditionId",
                        column: x => x.HandrailConditionId,
                        principalSchema: "cside",
                        principalTable: "Conditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BridgeSurveys_Conditions_HandrailPostsConditionId",
                        column: x => x.HandrailPostsConditionId,
                        principalSchema: "cside",
                        principalTable: "Conditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BridgeSurveys_Infrastructure_InfrastructureItemId",
                        column: x => x.InfrastructureItemId,
                        principalSchema: "cside",
                        principalTable: "Infrastructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BridgeSurveys_Materials_BankSeatMaterialId",
                        column: x => x.BankSeatMaterialId,
                        principalSchema: "cside",
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BridgeSurveys_Materials_BeamMaterialId",
                        column: x => x.BeamMaterialId,
                        principalSchema: "cside",
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BridgeSurveys_Materials_DeckingMaterialId",
                        column: x => x.DeckingMaterialId,
                        principalSchema: "cside",
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BridgeSurveys_Materials_HandrailMaterialId",
                        column: x => x.HandrailMaterialId,
                        principalSchema: "cside",
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BridgeSurveys_Materials_HandrailPostsMaterialId",
                        column: x => x.HandrailPostsMaterialId,
                        principalSchema: "cside",
                        principalTable: "Materials",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BridgeSurveys_BankSeatConditionId",
                schema: "cside",
                table: "BridgeSurveys",
                column: "BankSeatConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_BridgeSurveys_BankSeatMaterialId",
                schema: "cside",
                table: "BridgeSurveys",
                column: "BankSeatMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_BridgeSurveys_BeamConditionId",
                schema: "cside",
                table: "BridgeSurveys",
                column: "BeamConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_BridgeSurveys_BeamMaterialId",
                schema: "cside",
                table: "BridgeSurveys",
                column: "BeamMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_BridgeSurveys_DeckingConditionId",
                schema: "cside",
                table: "BridgeSurveys",
                column: "DeckingConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_BridgeSurveys_DeckingMaterialId",
                schema: "cside",
                table: "BridgeSurveys",
                column: "DeckingMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_BridgeSurveys_HandrailConditionId",
                schema: "cside",
                table: "BridgeSurveys",
                column: "HandrailConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_BridgeSurveys_HandrailMaterialId",
                schema: "cside",
                table: "BridgeSurveys",
                column: "HandrailMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_BridgeSurveys_HandrailPostsConditionId",
                schema: "cside",
                table: "BridgeSurveys",
                column: "HandrailPostsConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_BridgeSurveys_HandrailPostsMaterialId",
                schema: "cside",
                table: "BridgeSurveys",
                column: "HandrailPostsMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_BridgeSurveys_InfrastructureItemId",
                schema: "cside",
                table: "BridgeSurveys",
                column: "InfrastructureItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BridgeSurveys",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "Conditions",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "Materials",
                schema: "cside");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:Enum:survey_status", "completed,incomplete,rejected,verified")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
