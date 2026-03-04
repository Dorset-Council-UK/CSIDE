using System;
using System.Text.Json;
using CSIDE.Data.Models.Surveys;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CSIDE.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cside");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:cside.survey_status", "completed,incomplete,rejected,verified")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateSequence(
                name: "SurveySequence",
                schema: "cside");

            migrationBuilder.CreateTable(
                name: "ApplicationRoles",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityName = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: false),
                    SecondaryEntityId = table.Column<string>(type: "text", nullable: true),
                    ChangeType = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    OldValues = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    NewValues = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    LogDate = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

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
                name: "ContactTypes",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactTypes", x => x.Id);
                });

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
                name: "DMMOApplicationDirectionsOfSecState",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOApplicationDirectionsOfSecState", x => x.Id);
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
                name: "InfrastructureTypes",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsBridge = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfrastructureTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LandownerDepositMediaTypes",
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
                    table.PrimaryKey("PK_LandownerDepositMediaTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LandownerDeposits",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SecondaryId = table.Column<int>(type: "integer", nullable: false),
                    ReceivedDate = table.Column<LocalDate>(type: "date", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    WebsiteNoticePublished = table.Column<LocalDate>(type: "date", nullable: true),
                    EmailNoticeSent = table.Column<LocalDate>(type: "date", nullable: true),
                    OnsiteNoticeErected = table.Column<LocalDate>(type: "date", nullable: true),
                    IntendedEffect = table.Column<string>(type: "text", nullable: true),
                    ElapseDate = table.Column<LocalDate>(type: "date", nullable: true),
                    FormCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    MapCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    FeePaid = table.Column<bool>(type: "boolean", nullable: false),
                    AllSigned = table.Column<bool>(type: "boolean", nullable: false),
                    DateAcknowledged = table.Column<LocalDate>(type: "date", nullable: true),
                    ChequeReceiptNumber = table.Column<string>(type: "text", nullable: true),
                    ChequePaidInDate = table.Column<LocalDate>(type: "date", nullable: true),
                    NoticeDrafted = table.Column<LocalDate>(type: "date", nullable: true),
                    SentToArchive = table.Column<LocalDate>(type: "date", nullable: true),
                    ArchiveReference = table.Column<string>(type: "text", nullable: true),
                    WebsiteEntryAdded = table.Column<LocalDate>(type: "date", nullable: true),
                    PrimaryContact = table.Column<string>(type: "text", nullable: true),
                    PrimaryContactUserId = table.Column<string>(type: "text", nullable: true),
                    Geom = table.Column<MultiPolygon>(type: "geometry (multipolygon)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDeposits", x => new { x.Id, x.SecondaryId });
                });

            migrationBuilder.CreateTable(
                name: "LandownerDepositTypeNames",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositTypeNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceJobPriorities",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJobPriorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceJobStatuses",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    FriendlyDescription = table.Column<string>(type: "text", nullable: false),
                    IsComplete = table.Column<bool>(type: "boolean", nullable: false),
                    IsDuplicate = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJobStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceTeams",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Geom = table.Column<Polygon>(type: "geometry (polygon)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTeams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsWood = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UploadDate = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    URL = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderDecisionsOfSecState",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDecisionsOfSecState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderDeterminationProcesses",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDeterminationProcesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParishCodes",
                schema: "cside",
                columns: table => new
                {
                    ParishId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParishCodes", x => new { x.ParishId, x.Code });
                    table.ForeignKey(
                        name: "FK_ParishCodes_Parishes_ParishId",
                        column: x => x.ParishId,
                        principalSchema: "cside",
                        principalTable: "Parishes",
                        principalColumn: "admin_unit_id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "PPOApplicationLegislation",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOApplicationLegislation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PPOApplicationPriorities",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOApplicationPriorities", x => x.Id);
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
                name: "ProblemTypes",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteLegalStatuses",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteLegalStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteOperationalStatuses",
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
                    table.PrimaryKey("PK_RouteOperationalStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteTypes",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserRoles",
                schema: "cside",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ApplicationRoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserRoles", x => new { x.UserId, x.ApplicationRoleId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserRoles_ApplicationRoles_ApplicationRoleId",
                        column: x => x.ApplicationRoleId,
                        principalSchema: "cside",
                        principalTable: "ApplicationRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    ContactTypeId = table.Column<int>(type: "integer", nullable: true)
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
                name: "DMMOApplication",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationDate = table.Column<LocalDate>(type: "date", nullable: true),
                    ReceivedDate = table.Column<LocalDate>(type: "date", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicationDetails = table.Column<string>(type: "text", nullable: false),
                    LocationDescription = table.Column<string>(type: "text", nullable: true),
                    CaseOfficer = table.Column<string>(type: "text", nullable: true),
                    CaseOfficerUserId = table.Column<string>(type: "text", nullable: true),
                    PrivateComments = table.Column<string>(type: "text", nullable: true),
                    PublicComments = table.Column<string>(type: "text", nullable: true),
                    DeterminationDate = table.Column<LocalDate>(type: "date", nullable: true),
                    Appeal = table.Column<bool>(type: "boolean", nullable: true),
                    AppealDate = table.Column<LocalDate>(type: "date", nullable: true),
                    DateOfDirectionOfSecState = table.Column<LocalDate>(type: "date", nullable: true),
                    Geom = table.Column<MultiLineString>(type: "geometry (multilinestring)", nullable: false),
                    CaseStatusId = table.Column<int>(type: "integer", nullable: false),
                    DirectionOfSecStateId = table.Column<int>(type: "integer", nullable: true),
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
                        name: "FK_DMMOApplication_DMMOApplicationDirectionsOfSecState_Directi~",
                        column: x => x.DirectionOfSecStateId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplicationDirectionsOfSecState",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LandownerDepositAddresses",
                schema: "cside",
                columns: table => new
                {
                    LandownerDepositId = table.Column<int>(type: "integer", nullable: false),
                    LandownerDepositSecondaryId = table.Column<int>(type: "integer", nullable: false),
                    UPRN = table.Column<long>(type: "bigint", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositAddresses", x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId, x.UPRN });
                    table.ForeignKey(
                        name: "FK_LandownerDepositAddresses_LandownerDeposits_LandownerDeposi~",
                        columns: x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId },
                        principalSchema: "cside",
                        principalTable: "LandownerDeposits",
                        principalColumns: new[] { "Id", "SecondaryId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LandownerDepositEvents",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LandownerDepositId = table.Column<int>(type: "integer", nullable: false),
                    LandownerDepositSecondaryId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EventDate = table.Column<LocalDate>(type: "date", nullable: false),
                    EventText = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: true),
                    AuthorName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LandownerDepositEvents_LandownerDeposits_LandownerDepositId~",
                        columns: x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId },
                        principalSchema: "cside",
                        principalTable: "LandownerDeposits",
                        principalColumns: new[] { "Id", "SecondaryId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LandownerDepositParishes",
                schema: "cside",
                columns: table => new
                {
                    LandownerDepositId = table.Column<int>(type: "integer", nullable: false),
                    LandownerDepositSecondaryId = table.Column<int>(type: "integer", nullable: false),
                    ParishId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositParishes", x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId, x.ParishId });
                    table.ForeignKey(
                        name: "FK_LandownerDepositParishes_LandownerDeposits_LandownerDeposit~",
                        columns: x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId },
                        principalSchema: "cside",
                        principalTable: "LandownerDeposits",
                        principalColumns: new[] { "Id", "SecondaryId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandownerDepositParishes_Parishes_ParishId",
                        column: x => x.ParishId,
                        principalSchema: "cside",
                        principalTable: "Parishes",
                        principalColumn: "admin_unit_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LandownerDepositTypes",
                schema: "cside",
                columns: table => new
                {
                    LandownerDepositId = table.Column<int>(type: "integer", nullable: false),
                    LandownerDepositSecondaryId = table.Column<int>(type: "integer", nullable: false),
                    LandownerDepositTypeNameId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositTypes", x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId, x.LandownerDepositTypeNameId });
                    table.ForeignKey(
                        name: "FK_LandownerDepositTypes_LandownerDepositTypeNames_LandownerDe~",
                        column: x => x.LandownerDepositTypeNameId,
                        principalSchema: "cside",
                        principalTable: "LandownerDepositTypeNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandownerDepositTypes_LandownerDeposits_LandownerDepositId_~",
                        columns: x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId },
                        principalSchema: "cside",
                        principalTable: "LandownerDeposits",
                        principalColumns: new[] { "Id", "SecondaryId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceTeamUsers",
                schema: "cside",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    IsLead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTeamUsers", x => new { x.TeamId, x.UserId });
                    table.ForeignKey(
                        name: "FK_MaintenanceTeamUsers_MaintenanceTeams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LandownerDepositMedia",
                schema: "cside",
                columns: table => new
                {
                    LandownerDepositId = table.Column<int>(type: "integer", nullable: false),
                    LandownerDepositSecondaryId = table.Column<int>(type: "integer", nullable: false),
                    MediaId = table.Column<int>(type: "integer", nullable: false),
                    MediaTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositMedia", x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId, x.MediaId });
                    table.ForeignKey(
                        name: "FK_LandownerDepositMedia_LandownerDepositMediaTypes_MediaTypeId",
                        column: x => x.MediaTypeId,
                        principalSchema: "cside",
                        principalTable: "LandownerDepositMediaTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandownerDepositMedia_LandownerDeposits_LandownerDepositId_~",
                        columns: x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId },
                        principalSchema: "cside",
                        principalTable: "LandownerDeposits",
                        principalColumns: new[] { "Id", "SecondaryId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandownerDepositMedia_Media_MediaId",
                        column: x => x.MediaId,
                        principalSchema: "cside",
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyMedia",
                schema: "cside",
                columns: table => new
                {
                    SurveyId = table.Column<int>(type: "integer", nullable: false),
                    MediaId = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "PPOApplication",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReceivedDate = table.Column<LocalDate>(type: "date", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicationDetails = table.Column<string>(type: "text", nullable: false),
                    LocationDescription = table.Column<string>(type: "text", nullable: true),
                    CaseOfficer = table.Column<string>(type: "text", nullable: true),
                    CaseOfficerUserId = table.Column<string>(type: "text", nullable: true),
                    DeterminationDate = table.Column<LocalDate>(type: "date", nullable: true),
                    CouncilLandAffected = table.Column<bool>(type: "boolean", nullable: true),
                    Charge = table.Column<decimal>(type: "numeric", nullable: true),
                    BoxNumber = table.Column<string>(type: "text", nullable: true),
                    PrivateComments = table.Column<string>(type: "text", nullable: true),
                    PublicComments = table.Column<string>(type: "text", nullable: true),
                    Geom = table.Column<MultiLineString>(type: "geometry (multilinestring)", nullable: false),
                    LegislationId = table.Column<int>(type: "integer", nullable: false),
                    PriorityId = table.Column<int>(type: "integer", nullable: false),
                    CaseStatusId = table.Column<int>(type: "integer", nullable: false),
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
                        name: "FK_PPOApplication_PPOApplicationLegislation_LegislationId",
                        column: x => x.LegislationId,
                        principalSchema: "cside",
                        principalTable: "PPOApplicationLegislation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPOApplication_PPOApplicationPriorities_PriorityId",
                        column: x => x.PriorityId,
                        principalSchema: "cside",
                        principalTable: "PPOApplicationPriorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                schema: "cside",
                columns: table => new
                {
                    RouteCode = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ClosureStartDate = table.Column<LocalDate>(type: "date", nullable: true),
                    ClosureEndDate = table.Column<LocalDate>(type: "date", nullable: true),
                    ClosureIsIndefinite = table.Column<bool>(type: "boolean", nullable: false),
                    Geom = table.Column<MultiLineString>(type: "geometry (multilinestring)", nullable: false),
                    RouteTypeId = table.Column<int>(type: "integer", nullable: false),
                    OperationalStatusId = table.Column<int>(type: "integer", nullable: false),
                    LegalStatusId = table.Column<int>(type: "integer", nullable: false),
                    MaintenanceTeamId = table.Column<int>(type: "integer", nullable: true),
                    ParishId = table.Column<int>(type: "integer", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.RouteCode);
                    table.ForeignKey(
                        name: "FK_Routes_MaintenanceTeams_MaintenanceTeamId",
                        column: x => x.MaintenanceTeamId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceTeams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Routes_Parishes_ParishId",
                        column: x => x.ParishId,
                        principalSchema: "cside",
                        principalTable: "Parishes",
                        principalColumn: "admin_unit_id");
                    table.ForeignKey(
                        name: "FK_Routes_RouteLegalStatuses_LegalStatusId",
                        column: x => x.LegalStatusId,
                        principalSchema: "cside",
                        principalTable: "RouteLegalStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Routes_RouteOperationalStatuses_OperationalStatusId",
                        column: x => x.OperationalStatusId,
                        principalSchema: "cside",
                        principalTable: "RouteOperationalStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Routes_RouteTypes_RouteTypeId",
                        column: x => x.RouteTypeId,
                        principalSchema: "cside",
                        principalTable: "RouteTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LandownerDepositContacts",
                schema: "cside",
                columns: table => new
                {
                    LandownerDepositId = table.Column<int>(type: "integer", nullable: false),
                    LandownerDepositSecondaryId = table.Column<int>(type: "integer", nullable: false),
                    ContactId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandownerDepositContacts", x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_LandownerDepositContacts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "cside",
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandownerDepositContacts_LandownerDeposits_LandownerDeposit~",
                        columns: x => new { x.LandownerDepositId, x.LandownerDepositSecondaryId },
                        principalSchema: "cside",
                        principalTable: "LandownerDeposits",
                        principalColumns: new[] { "Id", "SecondaryId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DMMOAddresses",
                schema: "cside",
                columns: table => new
                {
                    DMMOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    UPRN = table.Column<long>(type: "bigint", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOAddresses", x => new { x.DMMOApplicationId, x.UPRN });
                    table.ForeignKey(
                        name: "FK_DMMOAddresses_DMMOApplication_DMMOApplicationId",
                        column: x => x.DMMOApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DMMOClaimedStatuses",
                schema: "cside",
                columns: table => new
                {
                    DMMOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ClaimedStatusId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOClaimedStatuses", x => new { x.DMMOApplicationId, x.ClaimedStatusId });
                    table.ForeignKey(
                        name: "FK_DMMOClaimedStatuses_DMMOApplicationClaimedStatuses_ClaimedS~",
                        column: x => x.ClaimedStatusId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplicationClaimedStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOClaimedStatuses_DMMOApplication_DMMOApplicationId",
                        column: x => x.DMMOApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DMMOContact",
                schema: "cside",
                columns: table => new
                {
                    DMMOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ContactId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOContact", x => new { x.DMMOApplicationId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_DMMOContact_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "cside",
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOContact_DMMOApplication_DMMOApplicationId",
                        column: x => x.DMMOApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DMMOEvents",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DMMOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EventDate = table.Column<LocalDate>(type: "date", nullable: false),
                    EventText = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: true),
                    AuthorName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DMMOEvents_DMMOApplication_DMMOApplicationId",
                        column: x => x.DMMOApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "DMMOOrders",
                schema: "cside",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    DMMOApplicationId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_DMMOOrders", x => new { x.OrderId, x.DMMOApplicationId });
                    table.ForeignKey(
                        name: "FK_DMMOOrders_DMMOApplication_DMMOApplicationId",
                        column: x => x.DMMOApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOOrders_OrderDecisionsOfSecState_DecisionOfSecStateId",
                        column: x => x.DecisionOfSecStateId,
                        principalSchema: "cside",
                        principalTable: "OrderDecisionsOfSecState",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DMMOOrders_OrderDeterminationProcesses_DeterminationProcess~",
                        column: x => x.DeterminationProcessId,
                        principalSchema: "cside",
                        principalTable: "OrderDeterminationProcesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DMMOParish",
                schema: "cside",
                columns: table => new
                {
                    DMMOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ParishId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOParish", x => new { x.DMMOApplicationId, x.ParishId });
                    table.ForeignKey(
                        name: "FK_DMMOParish_DMMOApplication_DMMOApplicationId",
                        column: x => x.DMMOApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOParish_Parishes_ParishId",
                        column: x => x.ParishId,
                        principalSchema: "cside",
                        principalTable: "Parishes",
                        principalColumn: "admin_unit_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DMMOTypes",
                schema: "cside",
                columns: table => new
                {
                    DMMOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ApplicationTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOTypes", x => new { x.DMMOApplicationId, x.ApplicationTypeId });
                    table.ForeignKey(
                        name: "FK_DMMOTypes_DMMOApplicationTypes_ApplicationTypeId",
                        column: x => x.ApplicationTypeId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplicationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOTypes_DMMOApplication_DMMOApplicationId",
                        column: x => x.DMMOApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PPOContact",
                schema: "cside",
                columns: table => new
                {
                    PPOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ContactId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOContact", x => new { x.PPOApplicationId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_PPOContact_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "cside",
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPOContact_PPOApplication_PPOApplicationId",
                        column: x => x.PPOApplicationId,
                        principalSchema: "cside",
                        principalTable: "PPOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PPOEvents",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PPOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EventDate = table.Column<LocalDate>(type: "date", nullable: false),
                    EventText = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: true),
                    AuthorName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PPOEvents_PPOApplication_PPOApplicationId",
                        column: x => x.PPOApplicationId,
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
                    PPOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    OrderTitle = table.Column<string>(type: "text", nullable: true),
                    ObjectionsEndDate = table.Column<LocalDate>(type: "date", nullable: true),
                    ObjectionsReceived = table.Column<bool>(type: "boolean", nullable: true),
                    ObjectionsWithdrawn = table.Column<bool>(type: "boolean", nullable: true),
                    DeterminationProcessId = table.Column<int>(type: "integer", nullable: true),
                    DecisionOfSecStateId = table.Column<int>(type: "integer", nullable: true),
                    DateConfirmed = table.Column<LocalDate>(type: "date", nullable: true),
                    DateSealed = table.Column<LocalDate>(type: "date", nullable: true),
                    DatePublished = table.Column<LocalDate>(type: "date", nullable: true),
                    SubmitToPINS = table.Column<bool>(type: "boolean", nullable: true),
                    InspectionCertification = table.Column<bool>(type: "boolean", nullable: true),
                    InspectionCertificationDate = table.Column<LocalDate>(type: "date", nullable: true),
                    ConfirmationPublishedDate = table.Column<LocalDate>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOOrders", x => new { x.OrderId, x.PPOApplicationId });
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
                        name: "FK_PPOOrders_PPOApplication_PPOApplicationId",
                        column: x => x.PPOApplicationId,
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
                    PPOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    ParishId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOParishes", x => new { x.PPOApplicationId, x.ParishId });
                    table.ForeignKey(
                        name: "FK_PPOParishes_PPOApplication_PPOApplicationId",
                        column: x => x.PPOApplicationId,
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

            migrationBuilder.CreateTable(
                name: "PPOTypes",
                schema: "cside",
                columns: table => new
                {
                    PPOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    TypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPOTypes", x => new { x.PPOApplicationId, x.TypeId });
                    table.ForeignKey(
                        name: "FK_PPOTypes_PPOApplicationTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "cside",
                        principalTable: "PPOApplicationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPOTypes_PPOApplication_PPOApplicationId",
                        column: x => x.PPOApplicationId,
                        principalSchema: "cside",
                        principalTable: "PPOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DMMOLinkedRoutes",
                schema: "cside",
                columns: table => new
                {
                    DMMOApplicationId = table.Column<int>(type: "integer", nullable: false),
                    RouteId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMMOLinkedRoutes", x => new { x.DMMOApplicationId, x.RouteId });
                    table.ForeignKey(
                        name: "FK_DMMOLinkedRoutes_DMMOApplication_DMMOApplicationId",
                        column: x => x.DMMOApplicationId,
                        principalSchema: "cside",
                        principalTable: "DMMOApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DMMOLinkedRoutes_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "cside",
                        principalTable: "Routes",
                        principalColumn: "RouteCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Infrastructure",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    InstallationDate = table.Column<LocalDate>(type: "date", nullable: true),
                    Height = table.Column<double>(type: "double precision", nullable: true),
                    Width = table.Column<double>(type: "double precision", nullable: true),
                    Length = table.Column<double>(type: "double precision", nullable: true),
                    Geom = table.Column<Point>(type: "geometry (point)", nullable: false),
                    RouteId = table.Column<string>(type: "text", nullable: true),
                    InfrastructureTypeId = table.Column<int>(type: "integer", nullable: true),
                    ParishId = table.Column<int>(type: "integer", nullable: true),
                    MaintenanceTeamId = table.Column<int>(type: "integer", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infrastructure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Infrastructure_InfrastructureTypes_InfrastructureTypeId",
                        column: x => x.InfrastructureTypeId,
                        principalSchema: "cside",
                        principalTable: "InfrastructureTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Infrastructure_MaintenanceTeams_MaintenanceTeamId",
                        column: x => x.MaintenanceTeamId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceTeams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Infrastructure_Parishes_ParishId",
                        column: x => x.ParishId,
                        principalSchema: "cside",
                        principalTable: "Parishes",
                        principalColumn: "admin_unit_id");
                    table.ForeignKey(
                        name: "FK_Infrastructure_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "cside",
                        principalTable: "Routes",
                        principalColumn: "RouteCode");
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceJobs",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogDate = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ProblemDescription = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    RedactedProblemDescription = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CompletionDate = table.Column<LocalDate>(type: "date", nullable: true),
                    WorkDone = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    LoggedById = table.Column<string>(type: "text", nullable: true),
                    LoggedByName = table.Column<string>(type: "text", nullable: true),
                    DuplicateJobId = table.Column<int>(type: "integer", nullable: true),
                    Geom = table.Column<Point>(type: "geometry (point)", nullable: false),
                    JobStatusId = table.Column<int>(type: "integer", nullable: false),
                    JobPriorityId = table.Column<int>(type: "integer", nullable: false),
                    RouteId = table.Column<string>(type: "text", nullable: true),
                    MaintenanceTeamId = table.Column<int>(type: "integer", nullable: true),
                    ParishId = table.Column<int>(type: "integer", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceJobs_MaintenanceJobPriorities_JobPriorityId",
                        column: x => x.JobPriorityId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceJobPriorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceJobs_MaintenanceJobStatuses_JobStatusId",
                        column: x => x.JobStatusId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceJobStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceJobs_MaintenanceTeams_MaintenanceTeamId",
                        column: x => x.MaintenanceTeamId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceTeams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaintenanceJobs_Parishes_ParishId",
                        column: x => x.ParishId,
                        principalSchema: "cside",
                        principalTable: "Parishes",
                        principalColumn: "admin_unit_id");
                    table.ForeignKey(
                        name: "FK_MaintenanceJobs_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "cside",
                        principalTable: "Routes",
                        principalColumn: "RouteCode");
                });

            migrationBuilder.CreateTable(
                name: "RouteMedia",
                schema: "cside",
                columns: table => new
                {
                    RouteId = table.Column<string>(type: "text", nullable: false),
                    MediaId = table.Column<int>(type: "integer", nullable: false),
                    IsClosureNotificationDocument = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteMedia", x => new { x.RouteId, x.MediaId });
                    table.ForeignKey(
                        name: "FK_RouteMedia_Media_MediaId",
                        column: x => x.MediaId,
                        principalSchema: "cside",
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteMedia_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "cside",
                        principalTable: "Routes",
                        principalColumn: "RouteCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Statements",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RouteId = table.Column<string>(type: "text", nullable: false),
                    StatementText = table.Column<string>(type: "text", nullable: false),
                    StartGridRef = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EndGridRef = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statements_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "cside",
                        principalTable: "Routes",
                        principalColumn: "RouteCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BridgeSurveys",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('cside.\"SurveySequence\"')"),
                    StartDate = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EndDate = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<SurveyStatus>(type: "cside.survey_status", nullable: false, defaultValueSql: "'incomplete'::survey_status"),
                    SurveyorId = table.Column<string>(type: "text", nullable: true),
                    SurveyorName = table.Column<string>(type: "text", nullable: true),
                    RepairsRequired = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ValidationNotes = table.Column<string>(type: "text", nullable: true),
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
                    UpdatedX = table.Column<double>(type: "double precision", nullable: true),
                    UpdatedY = table.Column<double>(type: "double precision", nullable: true),
                    LocationAccuracy = table.Column<int>(type: "integer", nullable: true)
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
                    StileInstalled = table.Column<bool>(type: "boolean", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "InfrastructureMedia",
                schema: "cside",
                columns: table => new
                {
                    InfrastructureItemId = table.Column<int>(type: "integer", nullable: false),
                    MediaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfrastructureMedia", x => new { x.InfrastructureItemId, x.MediaId });
                    table.ForeignKey(
                        name: "FK_InfrastructureMedia_Infrastructure_InfrastructureItemId",
                        column: x => x.InfrastructureItemId,
                        principalSchema: "cside",
                        principalTable: "Infrastructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InfrastructureMedia_Media_MediaId",
                        column: x => x.MediaId,
                        principalSchema: "cside",
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceComments",
                schema: "cside",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CommentText = table.Column<string>(type: "text", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: true),
                    AuthorName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceComments_MaintenanceJobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceJobContact",
                schema: "cside",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    ContactId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJobContact", x => new { x.JobId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_MaintenanceJobContact_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "cside",
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceJobContact_MaintenanceJobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceJobInfrastructure",
                schema: "cside",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    InfrastructureId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJobInfrastructure", x => new { x.JobId, x.InfrastructureId });
                    table.ForeignKey(
                        name: "FK_MaintenanceJobInfrastructure_Infrastructure_InfrastructureId",
                        column: x => x.InfrastructureId,
                        principalSchema: "cside",
                        principalTable: "Infrastructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceJobInfrastructure_MaintenanceJobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceJobMedia",
                schema: "cside",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    MediaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJobMedia", x => new { x.JobId, x.MediaId });
                    table.ForeignKey(
                        name: "FK_MaintenanceJobMedia_MaintenanceJobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceJobMedia_Media_MediaId",
                        column: x => x.MediaId,
                        principalSchema: "cside",
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceJobProblemTypes",
                schema: "cside",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    ProblemTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJobProblemTypes", x => new { x.JobId, x.ProblemTypeId });
                    table.ForeignKey(
                        name: "FK_MaintenanceJobProblemTypes_MaintenanceJobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceJobProblemTypes_ProblemTypes_ProblemTypeId",
                        column: x => x.ProblemTypeId,
                        principalSchema: "cside",
                        principalTable: "ProblemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceJobSubscribers",
                schema: "cside",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    EmailAddress = table.Column<string>(type: "text", nullable: false),
                    UnsubscribeToken = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceJobSubscribers", x => new { x.JobId, x.EmailAddress });
                    table.ForeignKey(
                        name: "FK_MaintenanceJobSubscribers_MaintenanceJobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "cside",
                        principalTable: "MaintenanceJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "cside",
                table: "ApplicationRoles",
                columns: new[] { "Id", "RoleName" },
                values: new object[,]
                {
                    { 1, "Administrator" },
                    { 2, "Ranger" },
                    { 3, "RoW Officer" },
                    { 4, "Survey Validator" },
                    { 5, "RoW Statement Editor" },
                    { 6, "View" },
                    { 7, "Surveyor" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserRoles_ApplicationRoleId",
                schema: "cside",
                table: "ApplicationUserRoles",
                column: "ApplicationRoleId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ContactTypeId",
                schema: "cside",
                table: "Contacts",
                column: "ContactTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOApplication_CaseStatusId",
                schema: "cside",
                table: "DMMOApplication",
                column: "CaseStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOApplication_DirectionOfSecStateId",
                schema: "cside",
                table: "DMMOApplication",
                column: "DirectionOfSecStateId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOClaimedStatuses_ClaimedStatusId",
                schema: "cside",
                table: "DMMOClaimedStatuses",
                column: "ClaimedStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOContact_ContactId",
                schema: "cside",
                table: "DMMOContact",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOEvents_DMMOApplicationId",
                schema: "cside",
                table: "DMMOEvents",
                column: "DMMOApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOLinkedRoutes_RouteId",
                schema: "cside",
                table: "DMMOLinkedRoutes",
                column: "RouteId");

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

            migrationBuilder.CreateIndex(
                name: "IX_DMMOOrders_DecisionOfSecStateId",
                schema: "cside",
                table: "DMMOOrders",
                column: "DecisionOfSecStateId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOOrders_DeterminationProcessId",
                schema: "cside",
                table: "DMMOOrders",
                column: "DeterminationProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOOrders_DMMOApplicationId",
                schema: "cside",
                table: "DMMOOrders",
                column: "DMMOApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOParish_ParishId",
                schema: "cside",
                table: "DMMOParish",
                column: "ParishId");

            migrationBuilder.CreateIndex(
                name: "IX_DMMOTypes_ApplicationTypeId",
                schema: "cside",
                table: "DMMOTypes",
                column: "ApplicationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Infrastructure_InfrastructureTypeId",
                schema: "cside",
                table: "Infrastructure",
                column: "InfrastructureTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Infrastructure_MaintenanceTeamId",
                schema: "cside",
                table: "Infrastructure",
                column: "MaintenanceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Infrastructure_ParishId",
                schema: "cside",
                table: "Infrastructure",
                column: "ParishId");

            migrationBuilder.CreateIndex(
                name: "IX_Infrastructure_RouteId",
                schema: "cside",
                table: "Infrastructure",
                column: "RouteId");

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

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureMedia_MediaId",
                schema: "cside",
                table: "InfrastructureMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositContacts_ContactId",
                schema: "cside",
                table: "LandownerDepositContacts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositEvents_LandownerDepositId_LandownerDepositS~",
                schema: "cside",
                table: "LandownerDepositEvents",
                columns: new[] { "LandownerDepositId", "LandownerDepositSecondaryId" });

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositMedia_MediaId",
                schema: "cside",
                table: "LandownerDepositMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositMedia_MediaTypeId",
                schema: "cside",
                table: "LandownerDepositMedia",
                column: "MediaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositParishes_ParishId",
                schema: "cside",
                table: "LandownerDepositParishes",
                column: "ParishId");

            migrationBuilder.CreateIndex(
                name: "IX_LandownerDepositTypes_LandownerDepositTypeNameId",
                schema: "cside",
                table: "LandownerDepositTypes",
                column: "LandownerDepositTypeNameId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceComments_JobId",
                schema: "cside",
                table: "MaintenanceComments",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobContact_ContactId",
                schema: "cside",
                table: "MaintenanceJobContact",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobInfrastructure_InfrastructureId",
                schema: "cside",
                table: "MaintenanceJobInfrastructure",
                column: "InfrastructureId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobMedia_MediaId",
                schema: "cside",
                table: "MaintenanceJobMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobProblemTypes_ProblemTypeId",
                schema: "cside",
                table: "MaintenanceJobProblemTypes",
                column: "ProblemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobs_JobPriorityId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "JobPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobs_JobStatusId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "JobStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobs_MaintenanceTeamId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "MaintenanceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobs_ParishId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "ParishId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobs_RouteId",
                schema: "cside",
                table: "MaintenanceJobs",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOApplication_CaseStatusId",
                schema: "cside",
                table: "PPOApplication",
                column: "CaseStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOApplication_LegislationId",
                schema: "cside",
                table: "PPOApplication",
                column: "LegislationId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOApplication_PriorityId",
                schema: "cside",
                table: "PPOApplication",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOContact_ContactId",
                schema: "cside",
                table: "PPOContact",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOEvents_PPOApplicationId",
                schema: "cside",
                table: "PPOEvents",
                column: "PPOApplicationId");

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
                name: "IX_PPOOrders_PPOApplicationId",
                schema: "cside",
                table: "PPOOrders",
                column: "PPOApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOParishes_ParishId",
                schema: "cside",
                table: "PPOParishes",
                column: "ParishId");

            migrationBuilder.CreateIndex(
                name: "IX_PPOTypes_TypeId",
                schema: "cside",
                table: "PPOTypes",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteMedia_MediaId",
                schema: "cside",
                table: "RouteMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_LegalStatusId",
                schema: "cside",
                table: "Routes",
                column: "LegalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_MaintenanceTeamId",
                schema: "cside",
                table: "Routes",
                column: "MaintenanceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OperationalStatusId",
                schema: "cside",
                table: "Routes",
                column: "OperationalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_ParishId",
                schema: "cside",
                table: "Routes",
                column: "ParishId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_RouteTypeId",
                schema: "cside",
                table: "Routes",
                column: "RouteTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Statements_RouteId",
                schema: "cside",
                table: "Statements",
                column: "RouteId");

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
                name: "ApplicationUserRoles",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "AuditLogs",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "BridgeSurveys",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOAddresses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOClaimedStatuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOContact",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOEvents",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOLinkedRoutes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOMedia",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOOrders",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOParish",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOTypes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "InfrastructureBridgeDetails",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "InfrastructureMedia",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "LandownerDepositAddresses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "LandownerDepositContacts",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "LandownerDepositEvents",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "LandownerDepositMedia",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "LandownerDepositParishes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "LandownerDepositTypes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "MaintenanceComments",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "MaintenanceJobContact",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "MaintenanceJobInfrastructure",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "MaintenanceJobMedia",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "MaintenanceJobProblemTypes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "MaintenanceJobSubscribers",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "MaintenanceTeamUsers",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ParishCodes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOContact",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOEvents",
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
                name: "PPOTypes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "RouteMedia",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "Statements",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "SurveyMedia",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ApplicationRoles",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOApplicationClaimedStatuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOMediaType",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOApplicationTypes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOApplication",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "Conditions",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "Materials",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "LandownerDepositMediaTypes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "LandownerDepositTypeNames",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "LandownerDeposits",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "Infrastructure",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ProblemTypes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "MaintenanceJobs",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "Contacts",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOMediaType",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "OrderDecisionsOfSecState",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "OrderDeterminationProcesses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOApplicationTypes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOApplication",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "Media",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOApplicationCaseStatuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "DMMOApplicationDirectionsOfSecState",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "InfrastructureTypes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "MaintenanceJobPriorities",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "MaintenanceJobStatuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "Routes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "ContactTypes",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOApplicationCaseStatuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOApplicationLegislation",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "PPOApplicationPriorities",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "MaintenanceTeams",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "RouteLegalStatuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "RouteOperationalStatuses",
                schema: "cside");

            migrationBuilder.DropTable(
                name: "RouteTypes",
                schema: "cside");

            migrationBuilder.DropSequence(
                name: "SurveySequence",
                schema: "cside");
        }
    }
}
