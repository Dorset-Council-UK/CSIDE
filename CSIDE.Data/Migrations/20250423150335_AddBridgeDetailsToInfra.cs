using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class AddBridgeDetailsToInfra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBridge",
                schema: "cside",
                table: "InfrastructureTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "InfrastructureBridgeDetails",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InfrastructureId = table.Column<int>(type: "integer", nullable: false),
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
                    NumDeckingBoards = table.Column<int>(type: "integer", nullable: true),
                    NumHandrailPostsTimbers = table.Column<int>(type: "integer", nullable: true),
                    BeamTimbersSize = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DeckingBoardsSize = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DeckingBoardsLength = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: true),
                    HandrailTimbersSize = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    HandrailPostsTimbersSize = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    HandrailsInPlace = table.Column<bool>(type: "boolean", nullable: true),
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
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfrastructureBridgeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InfrastructureBridgeDetails_Conditions_BankSeatConditionId",
                        column: x => x.BankSeatConditionId,
                        principalSchema: "cside",
                        principalTable: "Conditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InfrastructureBridgeDetails_Conditions_BeamConditionId",
                        column: x => x.BeamConditionId,
                        principalSchema: "cside",
                        principalTable: "Conditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InfrastructureBridgeDetails_Conditions_DeckingConditionId",
                        column: x => x.DeckingConditionId,
                        principalSchema: "cside",
                        principalTable: "Conditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InfrastructureBridgeDetails_Conditions_HandrailConditionId",
                        column: x => x.HandrailConditionId,
                        principalSchema: "cside",
                        principalTable: "Conditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InfrastructureBridgeDetails_Conditions_HandrailPostsConditi~",
                        column: x => x.HandrailPostsConditionId,
                        principalSchema: "cside",
                        principalTable: "Conditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InfrastructureBridgeDetails_Infrastructure_InfrastructureId",
                        column: x => x.InfrastructureId,
                        principalSchema: "cside",
                        principalTable: "Infrastructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InfrastructureBridgeDetails_Materials_BankSeatMaterialId",
                        column: x => x.BankSeatMaterialId,
                        principalSchema: "cside",
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InfrastructureBridgeDetails_Materials_BeamMaterialId",
                        column: x => x.BeamMaterialId,
                        principalSchema: "cside",
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InfrastructureBridgeDetails_Materials_DeckingMaterialId",
                        column: x => x.DeckingMaterialId,
                        principalSchema: "cside",
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InfrastructureBridgeDetails_Materials_HandrailMaterialId",
                        column: x => x.HandrailMaterialId,
                        principalSchema: "cside",
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InfrastructureBridgeDetails_Materials_HandrailPostsMaterial~",
                        column: x => x.HandrailPostsMaterialId,
                        principalSchema: "cside",
                        principalTable: "Materials",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureBridgeDetails_BankSeatConditionId",
                schema: "cside",
                table: "InfrastructureBridgeDetails",
                column: "BankSeatConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureBridgeDetails_BankSeatMaterialId",
                schema: "cside",
                table: "InfrastructureBridgeDetails",
                column: "BankSeatMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureBridgeDetails_BeamConditionId",
                schema: "cside",
                table: "InfrastructureBridgeDetails",
                column: "BeamConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureBridgeDetails_BeamMaterialId",
                schema: "cside",
                table: "InfrastructureBridgeDetails",
                column: "BeamMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureBridgeDetails_DeckingConditionId",
                schema: "cside",
                table: "InfrastructureBridgeDetails",
                column: "DeckingConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureBridgeDetails_DeckingMaterialId",
                schema: "cside",
                table: "InfrastructureBridgeDetails",
                column: "DeckingMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureBridgeDetails_HandrailConditionId",
                schema: "cside",
                table: "InfrastructureBridgeDetails",
                column: "HandrailConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureBridgeDetails_HandrailMaterialId",
                schema: "cside",
                table: "InfrastructureBridgeDetails",
                column: "HandrailMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureBridgeDetails_HandrailPostsConditionId",
                schema: "cside",
                table: "InfrastructureBridgeDetails",
                column: "HandrailPostsConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureBridgeDetails_HandrailPostsMaterialId",
                schema: "cside",
                table: "InfrastructureBridgeDetails",
                column: "HandrailPostsMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureBridgeDetails_InfrastructureId",
                schema: "cside",
                table: "InfrastructureBridgeDetails",
                column: "InfrastructureId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfrastructureBridgeDetails",
                schema: "cside");

            migrationBuilder.DropColumn(
                name: "IsBridge",
                schema: "cside",
                table: "InfrastructureTypes");
        }
    }
}
