using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLDepositIDScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositAddresses_LandownerDeposits_LandownerDeposi~",
                schema: "cside",
                table: "LandownerDepositAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositContacts_LandownerDeposits_LandownerDeposit~",
                schema: "cside",
                table: "LandownerDepositContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositEvents_LandownerDeposits_LandownerDepositId",
                schema: "cside",
                table: "LandownerDepositEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositMedia_LandownerDeposits_LandownerDepositId",
                schema: "cside",
                table: "LandownerDepositMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositParishes_LandownerDeposits_LandownerDeposit~",
                schema: "cside",
                table: "LandownerDepositParishes");

            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositTypes_LandownerDeposits_LandownerDepositId",
                schema: "cside",
                table: "LandownerDepositTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDepositTypes",
                schema: "cside",
                table: "LandownerDepositTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDeposits",
                schema: "cside",
                table: "LandownerDeposits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDepositParishes",
                schema: "cside",
                table: "LandownerDepositParishes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDepositMedia",
                schema: "cside",
                table: "LandownerDepositMedia");

            migrationBuilder.DropIndex(
                name: "IX_LandownerDepositEvents_LandownerDepositId",
                schema: "cside",
                table: "LandownerDepositEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDepositContacts",
                schema: "cside",
                table: "LandownerDepositContacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDepositAddresses",
                schema: "cside",
                table: "LandownerDepositAddresses");

            migrationBuilder.AddColumn<int>(
                name: "LandownerDepositSecondaryId",
                schema: "cside",
                table: "LandownerDepositTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecondaryId",
                schema: "cside",
                table: "LandownerDeposits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LandownerDepositSecondaryId",
                schema: "cside",
                table: "LandownerDepositParishes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LandownerDepositSecondaryId",
                schema: "cside",
                table: "LandownerDepositMedia",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LandownerDepositSecondaryId",
                schema: "cside",
                table: "LandownerDepositEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LandownerDepositSecondaryId",
                schema: "cside",
                table: "LandownerDepositContacts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LandownerDepositSecondaryId",
                schema: "cside",
                table: "LandownerDepositAddresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDepositTypes",
                schema: "cside",
                table: "LandownerDepositTypes",
                columns: new[] { "LandownerDepositId", "LandownerDepositSecondaryId", "LandownerDepositTypeNameId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDeposits",
                schema: "cside",
                table: "LandownerDeposits",
                columns: new[] { "Id", "SecondaryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDepositParishes",
                schema: "cside",
                table: "LandownerDepositParishes",
                columns: new[] { "LandownerDepositId", "LandownerDepositSecondaryId", "ParishId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDepositMedia",
                schema: "cside",
                table: "LandownerDepositMedia",
                columns: new[] { "LandownerDepositId", "LandownerDepositSecondaryId", "MediaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDepositContacts",
                schema: "cside",
                table: "LandownerDepositContacts",
                columns: new[] { "LandownerDepositId", "LandownerDepositSecondaryId", "ContactId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDepositAddresses",
                schema: "cside",
                table: "LandownerDepositAddresses",
                columns: new[] { "LandownerDepositId", "LandownerDepositSecondaryId", "UPRN" });

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositEvents_LandownerDepositId_LandownerDepositS~",
                schema: "cside",
                table: "LandownerDepositEvents",
                columns: new[] { "LandownerDepositId", "LandownerDepositSecondaryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositAddresses_LandownerDeposits_LandownerDeposi~",
                schema: "cside",
                table: "LandownerDepositAddresses",
                columns: new[] { "LandownerDepositId", "LandownerDepositSecondaryId" },
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumns: new[] { "Id", "SecondaryId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositContacts_LandownerDeposits_LandownerDeposit~",
                schema: "cside",
                table: "LandownerDepositContacts",
                columns: new[] { "LandownerDepositId", "LandownerDepositSecondaryId" },
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumns: new[] { "Id", "SecondaryId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositEvents_LandownerDeposits_LandownerDepositId~",
                schema: "cside",
                table: "LandownerDepositEvents",
                columns: new[] { "LandownerDepositId", "LandownerDepositSecondaryId" },
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumns: new[] { "Id", "SecondaryId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositMedia_LandownerDeposits_LandownerDepositId_~",
                schema: "cside",
                table: "LandownerDepositMedia",
                columns: new[] { "LandownerDepositId", "LandownerDepositSecondaryId" },
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumns: new[] { "Id", "SecondaryId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositParishes_LandownerDeposits_LandownerDeposit~",
                schema: "cside",
                table: "LandownerDepositParishes",
                columns: new[] { "LandownerDepositId", "LandownerDepositSecondaryId" },
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumns: new[] { "Id", "SecondaryId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositTypes_LandownerDeposits_LandownerDepositId_~",
                schema: "cside",
                table: "LandownerDepositTypes",
                columns: new[] { "LandownerDepositId", "LandownerDepositSecondaryId" },
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumns: new[] { "Id", "SecondaryId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositAddresses_LandownerDeposits_LandownerDeposi~",
                schema: "cside",
                table: "LandownerDepositAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositContacts_LandownerDeposits_LandownerDeposit~",
                schema: "cside",
                table: "LandownerDepositContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositEvents_LandownerDeposits_LandownerDepositId~",
                schema: "cside",
                table: "LandownerDepositEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositMedia_LandownerDeposits_LandownerDepositId_~",
                schema: "cside",
                table: "LandownerDepositMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositParishes_LandownerDeposits_LandownerDeposit~",
                schema: "cside",
                table: "LandownerDepositParishes");

            migrationBuilder.DropForeignKey(
                name: "FK_LandownerDepositTypes_LandownerDeposits_LandownerDepositId_~",
                schema: "cside",
                table: "LandownerDepositTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDepositTypes",
                schema: "cside",
                table: "LandownerDepositTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDeposits",
                schema: "cside",
                table: "LandownerDeposits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDepositParishes",
                schema: "cside",
                table: "LandownerDepositParishes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDepositMedia",
                schema: "cside",
                table: "LandownerDepositMedia");

            migrationBuilder.DropIndex(
                name: "IX_LandownerDepositEvents_LandownerDepositId_LandownerDepositS~",
                schema: "cside",
                table: "LandownerDepositEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDepositContacts",
                schema: "cside",
                table: "LandownerDepositContacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LandownerDepositAddresses",
                schema: "cside",
                table: "LandownerDepositAddresses");

            migrationBuilder.DropColumn(
                name: "LandownerDepositSecondaryId",
                schema: "cside",
                table: "LandownerDepositTypes");

            migrationBuilder.DropColumn(
                name: "SecondaryId",
                schema: "cside",
                table: "LandownerDeposits");

            migrationBuilder.DropColumn(
                name: "LandownerDepositSecondaryId",
                schema: "cside",
                table: "LandownerDepositParishes");

            migrationBuilder.DropColumn(
                name: "LandownerDepositSecondaryId",
                schema: "cside",
                table: "LandownerDepositMedia");

            migrationBuilder.DropColumn(
                name: "LandownerDepositSecondaryId",
                schema: "cside",
                table: "LandownerDepositEvents");

            migrationBuilder.DropColumn(
                name: "LandownerDepositSecondaryId",
                schema: "cside",
                table: "LandownerDepositContacts");

            migrationBuilder.DropColumn(
                name: "LandownerDepositSecondaryId",
                schema: "cside",
                table: "LandownerDepositAddresses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDepositTypes",
                schema: "cside",
                table: "LandownerDepositTypes",
                columns: new[] { "LandownerDepositId", "LandownerDepositTypeNameId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDeposits",
                schema: "cside",
                table: "LandownerDeposits",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDepositParishes",
                schema: "cside",
                table: "LandownerDepositParishes",
                columns: new[] { "LandownerDepositId", "ParishId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDepositMedia",
                schema: "cside",
                table: "LandownerDepositMedia",
                columns: new[] { "LandownerDepositId", "MediaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDepositContacts",
                schema: "cside",
                table: "LandownerDepositContacts",
                columns: new[] { "LandownerDepositId", "ContactId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LandownerDepositAddresses",
                schema: "cside",
                table: "LandownerDepositAddresses",
                columns: new[] { "LandownerDepositId", "UPRN" });

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositEvents_LandownerDepositId",
                schema: "cside",
                table: "LandownerDepositEvents",
                column: "LandownerDepositId");

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositAddresses_LandownerDeposits_LandownerDeposi~",
                schema: "cside",
                table: "LandownerDepositAddresses",
                column: "LandownerDepositId",
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositContacts_LandownerDeposits_LandownerDeposit~",
                schema: "cside",
                table: "LandownerDepositContacts",
                column: "LandownerDepositId",
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositEvents_LandownerDeposits_LandownerDepositId",
                schema: "cside",
                table: "LandownerDepositEvents",
                column: "LandownerDepositId",
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositMedia_LandownerDeposits_LandownerDepositId",
                schema: "cside",
                table: "LandownerDepositMedia",
                column: "LandownerDepositId",
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositParishes_LandownerDeposits_LandownerDeposit~",
                schema: "cside",
                table: "LandownerDepositParishes",
                column: "LandownerDepositId",
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandownerDepositTypes_LandownerDeposits_LandownerDepositId",
                schema: "cside",
                table: "LandownerDepositTypes",
                column: "LandownerDepositId",
                principalSchema: "cside",
                principalTable: "LandownerDeposits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
