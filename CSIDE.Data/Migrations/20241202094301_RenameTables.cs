using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSIDE.Migrations
{
    /// <inheritdoc />
    public partial class RenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobContact_Contacts_ContactId",
                schema: "cside",
                table: "JobContact");

            migrationBuilder.DropForeignKey(
                name: "FK_JobContact_MaintenanceJobs_JobId",
                schema: "cside",
                table: "JobContact");

            migrationBuilder.DropForeignKey(
                name: "FK_JobInfrastructure_Infrastructure_InfrastructureId",
                schema: "cside",
                table: "JobInfrastructure");

            migrationBuilder.DropForeignKey(
                name: "FK_JobInfrastructure_MaintenanceJobs_JobId",
                schema: "cside",
                table: "JobInfrastructure");

            migrationBuilder.DropForeignKey(
                name: "FK_JobMedia_MaintenanceJobs_JobId",
                schema: "cside",
                table: "JobMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_JobMedia_Media_MediaId",
                schema: "cside",
                table: "JobMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_JobProblemTypes_MaintenanceJobs_JobId",
                schema: "cside",
                table: "JobProblemTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_JobProblemTypes_ProblemTypes_ProblemTypeId",
                schema: "cside",
                table: "JobProblemTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_JobPriorities_JobPriorityId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_JobStatuses_JobStatusId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobStatuses",
                schema: "cside",
                table: "JobStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobProblemTypes",
                schema: "cside",
                table: "JobProblemTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobPriorities",
                schema: "cside",
                table: "JobPriorities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobMedia",
                schema: "cside",
                table: "JobMedia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobInfrastructure",
                schema: "cside",
                table: "JobInfrastructure");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobContact",
                schema: "cside",
                table: "JobContact");

            migrationBuilder.RenameTable(
                name: "JobStatuses",
                schema: "cside",
                newName: "MaintenanceJobStatuses",
                newSchema: "cside");

            migrationBuilder.RenameTable(
                name: "JobProblemTypes",
                schema: "cside",
                newName: "MaintenanceJobProblemTypes",
                newSchema: "cside");

            migrationBuilder.RenameTable(
                name: "JobPriorities",
                schema: "cside",
                newName: "MaintenanceJobPriorities",
                newSchema: "cside");

            migrationBuilder.RenameTable(
                name: "JobMedia",
                schema: "cside",
                newName: "MaintenanceJobMedia",
                newSchema: "cside");

            migrationBuilder.RenameTable(
                name: "JobInfrastructure",
                schema: "cside",
                newName: "MaintenanceJobInfrastructure",
                newSchema: "cside");

            migrationBuilder.RenameTable(
                name: "JobContact",
                schema: "cside",
                newName: "MaintenanceJobContact",
                newSchema: "cside");

            migrationBuilder.RenameIndex(
                name: "IX_JobProblemTypes_ProblemTypeId",
                schema: "cside",
                table: "MaintenanceJobProblemTypes",
                newName: "IX_MaintenanceJobProblemTypes_ProblemTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_JobMedia_MediaId",
                schema: "cside",
                table: "MaintenanceJobMedia",
                newName: "IX_MaintenanceJobMedia_MediaId");

            migrationBuilder.RenameIndex(
                name: "IX_JobInfrastructure_InfrastructureId",
                schema: "cside",
                table: "MaintenanceJobInfrastructure",
                newName: "IX_MaintenanceJobInfrastructure_InfrastructureId");

            migrationBuilder.RenameIndex(
                name: "IX_JobContact_ContactId",
                schema: "cside",
                table: "MaintenanceJobContact",
                newName: "IX_MaintenanceJobContact_ContactId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceJobStatuses",
                schema: "cside",
                table: "MaintenanceJobStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceJobProblemTypes",
                schema: "cside",
                table: "MaintenanceJobProblemTypes",
                columns: ["JobId", "ProblemTypeId"]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceJobPriorities",
                schema: "cside",
                table: "MaintenanceJobPriorities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceJobMedia",
                schema: "cside",
                table: "MaintenanceJobMedia",
                columns: ["JobId", "MediaId"]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceJobInfrastructure",
                schema: "cside",
                table: "MaintenanceJobInfrastructure",
                columns: ["JobId", "InfrastructureId"]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceJobContact",
                schema: "cside",
                table: "MaintenanceJobContact",
                columns: ["JobId", "ContactId"]);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobContact_Contacts_ContactId",
                schema: "cside",
                table: "MaintenanceJobContact",
                column: "ContactId",
                principalSchema: "cside",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobContact_MaintenanceJobs_JobId",
                schema: "cside",
                table: "MaintenanceJobContact",
                column: "JobId",
                principalSchema: "cside",
                principalTable: "MaintenanceJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobInfrastructure_Infrastructure_InfrastructureId",
                schema: "cside",
                table: "MaintenanceJobInfrastructure",
                column: "InfrastructureId",
                principalSchema: "cside",
                principalTable: "Infrastructure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobInfrastructure_MaintenanceJobs_JobId",
                schema: "cside",
                table: "MaintenanceJobInfrastructure",
                column: "JobId",
                principalSchema: "cside",
                principalTable: "MaintenanceJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobMedia_MaintenanceJobs_JobId",
                schema: "cside",
                table: "MaintenanceJobMedia",
                column: "JobId",
                principalSchema: "cside",
                principalTable: "MaintenanceJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobMedia_Media_MediaId",
                schema: "cside",
                table: "MaintenanceJobMedia",
                column: "MediaId",
                principalSchema: "cside",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobProblemTypes_MaintenanceJobs_JobId",
                schema: "cside",
                table: "MaintenanceJobProblemTypes",
                column: "JobId",
                principalSchema: "cside",
                principalTable: "MaintenanceJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobProblemTypes_ProblemTypes_ProblemTypeId",
                schema: "cside",
                table: "MaintenanceJobProblemTypes",
                column: "ProblemTypeId",
                principalSchema: "cside",
                principalTable: "ProblemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_MaintenanceJobPriorities_JobPriorityId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "JobPriorityId",
                principalSchema: "cside",
                principalTable: "MaintenanceJobPriorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_MaintenanceJobStatuses_JobStatusId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "JobStatusId",
                principalSchema: "cside",
                principalTable: "MaintenanceJobStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobContact_Contacts_ContactId",
                schema: "cside",
                table: "MaintenanceJobContact");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobContact_MaintenanceJobs_JobId",
                schema: "cside",
                table: "MaintenanceJobContact");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobInfrastructure_Infrastructure_InfrastructureId",
                schema: "cside",
                table: "MaintenanceJobInfrastructure");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobInfrastructure_MaintenanceJobs_JobId",
                schema: "cside",
                table: "MaintenanceJobInfrastructure");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobMedia_MaintenanceJobs_JobId",
                schema: "cside",
                table: "MaintenanceJobMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobMedia_Media_MediaId",
                schema: "cside",
                table: "MaintenanceJobMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobProblemTypes_MaintenanceJobs_JobId",
                schema: "cside",
                table: "MaintenanceJobProblemTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobProblemTypes_ProblemTypes_ProblemTypeId",
                schema: "cside",
                table: "MaintenanceJobProblemTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_MaintenanceJobPriorities_JobPriorityId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_MaintenanceJobStatuses_JobStatusId",
                schema: "cside",
                table: "MaintenanceJobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceJobStatuses",
                schema: "cside",
                table: "MaintenanceJobStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceJobProblemTypes",
                schema: "cside",
                table: "MaintenanceJobProblemTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceJobPriorities",
                schema: "cside",
                table: "MaintenanceJobPriorities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceJobMedia",
                schema: "cside",
                table: "MaintenanceJobMedia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceJobInfrastructure",
                schema: "cside",
                table: "MaintenanceJobInfrastructure");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceJobContact",
                schema: "cside",
                table: "MaintenanceJobContact");

            migrationBuilder.RenameTable(
                name: "MaintenanceJobStatuses",
                schema: "cside",
                newName: "JobStatuses",
                newSchema: "cside");

            migrationBuilder.RenameTable(
                name: "MaintenanceJobProblemTypes",
                schema: "cside",
                newName: "JobProblemTypes",
                newSchema: "cside");

            migrationBuilder.RenameTable(
                name: "MaintenanceJobPriorities",
                schema: "cside",
                newName: "JobPriorities",
                newSchema: "cside");

            migrationBuilder.RenameTable(
                name: "MaintenanceJobMedia",
                schema: "cside",
                newName: "JobMedia",
                newSchema: "cside");

            migrationBuilder.RenameTable(
                name: "MaintenanceJobInfrastructure",
                schema: "cside",
                newName: "JobInfrastructure",
                newSchema: "cside");

            migrationBuilder.RenameTable(
                name: "MaintenanceJobContact",
                schema: "cside",
                newName: "JobContact",
                newSchema: "cside");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceJobProblemTypes_ProblemTypeId",
                schema: "cside",
                table: "JobProblemTypes",
                newName: "IX_JobProblemTypes_ProblemTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceJobMedia_MediaId",
                schema: "cside",
                table: "JobMedia",
                newName: "IX_JobMedia_MediaId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceJobInfrastructure_InfrastructureId",
                schema: "cside",
                table: "JobInfrastructure",
                newName: "IX_JobInfrastructure_InfrastructureId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceJobContact_ContactId",
                schema: "cside",
                table: "JobContact",
                newName: "IX_JobContact_ContactId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobStatuses",
                schema: "cside",
                table: "JobStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobProblemTypes",
                schema: "cside",
                table: "JobProblemTypes",
                columns: ["JobId", "ProblemTypeId"]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobPriorities",
                schema: "cside",
                table: "JobPriorities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobMedia",
                schema: "cside",
                table: "JobMedia",
                columns: ["JobId", "MediaId"]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobInfrastructure",
                schema: "cside",
                table: "JobInfrastructure",
                columns: ["JobId", "InfrastructureId"]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobContact",
                schema: "cside",
                table: "JobContact",
                columns: ["JobId", "ContactId"]);

            migrationBuilder.AddForeignKey(
                name: "FK_JobContact_Contacts_ContactId",
                schema: "cside",
                table: "JobContact",
                column: "ContactId",
                principalSchema: "cside",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobContact_MaintenanceJobs_JobId",
                schema: "cside",
                table: "JobContact",
                column: "JobId",
                principalSchema: "cside",
                principalTable: "MaintenanceJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobInfrastructure_Infrastructure_InfrastructureId",
                schema: "cside",
                table: "JobInfrastructure",
                column: "InfrastructureId",
                principalSchema: "cside",
                principalTable: "Infrastructure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobInfrastructure_MaintenanceJobs_JobId",
                schema: "cside",
                table: "JobInfrastructure",
                column: "JobId",
                principalSchema: "cside",
                principalTable: "MaintenanceJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobMedia_MaintenanceJobs_JobId",
                schema: "cside",
                table: "JobMedia",
                column: "JobId",
                principalSchema: "cside",
                principalTable: "MaintenanceJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobMedia_Media_MediaId",
                schema: "cside",
                table: "JobMedia",
                column: "MediaId",
                principalSchema: "cside",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobProblemTypes_MaintenanceJobs_JobId",
                schema: "cside",
                table: "JobProblemTypes",
                column: "JobId",
                principalSchema: "cside",
                principalTable: "MaintenanceJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobProblemTypes_ProblemTypes_ProblemTypeId",
                schema: "cside",
                table: "JobProblemTypes",
                column: "ProblemTypeId",
                principalSchema: "cside",
                principalTable: "ProblemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_JobPriorities_JobPriorityId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "JobPriorityId",
                principalSchema: "cside",
                principalTable: "JobPriorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_JobStatuses_JobStatusId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "JobStatusId",
                principalSchema: "cside",
                principalTable: "JobStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
