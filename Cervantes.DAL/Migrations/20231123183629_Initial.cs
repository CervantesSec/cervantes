using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Avatar = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Position = table.Column<string>(type: "text", nullable: true),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cwe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cwe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedOn = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    StackTrace = table.Column<string>(type: "text", nullable: false),
                    Exception = table.Column<string>(type: "text", nullable: false),
                    Logger = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    ContactName = table.Column<string>(type: "text", nullable: false),
                    ContactEmail = table.Column<string>(type: "text", nullable: false),
                    ContactPhone = table.Column<string>(type: "text", nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VulnCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VulnCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromUserId = table.Column<string>(type: "text", nullable: false),
                    ToUserId = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatMessages_AspNetUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    ContactName = table.Column<string>(type: "text", nullable: false),
                    ContactEmail = table.Column<string>(type: "text", nullable: false),
                    ContactPhone = table.Column<string>(type: "text", nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportTemplates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Template = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProjectType = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    FindingsId = table.Column<string>(type: "text", nullable: false),
                    ExecutiveSummary = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Projects_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAttachments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectAttachments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectNotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectNotes_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Targets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Targets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Targets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Targets_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Template = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedUserId = table.Column<string>(type: "text", nullable: false),
                    AsignedUserId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_AspNetUsers_AsignedUserId",
                        column: x => x.AsignedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_AspNetUsers_CreatedUserId",
                        column: x => x.CreatedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Vaults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vaults_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vaults_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vulns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FindingId = table.Column<string>(type: "text", nullable: false),
                    Template = table.Column<bool>(type: "boolean", nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    VulnCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Risk = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    cve = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ProofOfConcept = table.Column<string>(type: "text", nullable: false),
                    Impact = table.Column<string>(type: "text", nullable: false),
                    CVSS3 = table.Column<double>(type: "double precision", nullable: false),
                    CVSSVector = table.Column<string>(type: "text", nullable: false),
                    Remediation = table.Column<string>(type: "text", nullable: false),
                    RemediationComplexity = table.Column<int>(type: "integer", nullable: false),
                    RemediationPriority = table.Column<int>(type: "integer", nullable: false),
                    JiraCreated = table.Column<bool>(type: "boolean", nullable: false),
                    OWASPRisk = table.Column<string>(type: "text", nullable: false),
                    OWASPImpact = table.Column<string>(type: "text", nullable: false),
                    OWASPLikehood = table.Column<string>(type: "text", nullable: false),
                    OWASPVector = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vulns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vulns_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vulns_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vulns_VulnCategories_VulnCategoryId",
                        column: x => x.VulnCategoryId,
                        principalTable: "VulnCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MASTG",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MobilePlatform = table.Column<int>(type: "integer", nullable: false),
                    Storage1Note = table.Column<string>(type: "text", nullable: false),
                    Storage1Status = table.Column<int>(type: "integer", nullable: false),
                    Storage1Note1 = table.Column<string>(type: "text", nullable: false),
                    Storage1Note2 = table.Column<string>(type: "text", nullable: false),
                    Storage1Note3 = table.Column<string>(type: "text", nullable: false),
                    Storage1Status1 = table.Column<int>(type: "integer", nullable: false),
                    Storage1Status2 = table.Column<int>(type: "integer", nullable: false),
                    Storage1Status3 = table.Column<int>(type: "integer", nullable: false),
                    Storage2Note = table.Column<string>(type: "text", nullable: false),
                    Storage2Status = table.Column<int>(type: "integer", nullable: false),
                    Storage2Note1 = table.Column<string>(type: "text", nullable: false),
                    Storage2Note2 = table.Column<string>(type: "text", nullable: false),
                    Storage2Note3 = table.Column<string>(type: "text", nullable: false),
                    Storage2Note4 = table.Column<string>(type: "text", nullable: false),
                    Storage2Note5 = table.Column<string>(type: "text", nullable: false),
                    Storage2Note6 = table.Column<string>(type: "text", nullable: false),
                    Storage2Note7 = table.Column<string>(type: "text", nullable: false),
                    Storage2Note8 = table.Column<string>(type: "text", nullable: false),
                    Storage2Note9 = table.Column<string>(type: "text", nullable: false),
                    Storage2Note10 = table.Column<string>(type: "text", nullable: false),
                    Storage2Note11 = table.Column<string>(type: "text", nullable: false),
                    Storage2Status1 = table.Column<int>(type: "integer", nullable: false),
                    Storage2Status2 = table.Column<int>(type: "integer", nullable: false),
                    Storage2Status3 = table.Column<int>(type: "integer", nullable: false),
                    Storage2Status4 = table.Column<int>(type: "integer", nullable: false),
                    Storage2Status5 = table.Column<int>(type: "integer", nullable: false),
                    Storage2Status6 = table.Column<int>(type: "integer", nullable: false),
                    Storage2Status7 = table.Column<int>(type: "integer", nullable: false),
                    Storage2Status8 = table.Column<int>(type: "integer", nullable: false),
                    Storage2Status9 = table.Column<int>(type: "integer", nullable: false),
                    Storage2Status10 = table.Column<int>(type: "integer", nullable: false),
                    Storage2Status11 = table.Column<int>(type: "integer", nullable: false),
                    Crypto1Note = table.Column<string>(type: "text", nullable: false),
                    Crypto1Note1 = table.Column<string>(type: "text", nullable: false),
                    Crypto1Note2 = table.Column<string>(type: "text", nullable: false),
                    Crypto1Note3 = table.Column<string>(type: "text", nullable: false),
                    Crypto1Note4 = table.Column<string>(type: "text", nullable: false),
                    Crypto1Note5 = table.Column<string>(type: "text", nullable: false),
                    Crypto1Status = table.Column<int>(type: "integer", nullable: false),
                    Crypto1Status1 = table.Column<int>(type: "integer", nullable: false),
                    Crypto1Status2 = table.Column<int>(type: "integer", nullable: false),
                    Crypto1Status3 = table.Column<int>(type: "integer", nullable: false),
                    Crypto1Status4 = table.Column<int>(type: "integer", nullable: false),
                    Crypto1Status5 = table.Column<int>(type: "integer", nullable: false),
                    Crypto2Note = table.Column<string>(type: "text", nullable: false),
                    Crypto2Note1 = table.Column<string>(type: "text", nullable: false),
                    Crypto2Note2 = table.Column<string>(type: "text", nullable: false),
                    Crypto2Status = table.Column<int>(type: "integer", nullable: false),
                    Crypto2Status1 = table.Column<int>(type: "integer", nullable: false),
                    Crypto2Status2 = table.Column<int>(type: "integer", nullable: false),
                    Auth1Note = table.Column<string>(type: "text", nullable: false),
                    Auth1Status = table.Column<int>(type: "integer", nullable: false),
                    Auth2Note = table.Column<string>(type: "text", nullable: false),
                    Auth2Note1 = table.Column<string>(type: "text", nullable: false),
                    Auth2Note2 = table.Column<string>(type: "text", nullable: false),
                    Auth2Note3 = table.Column<string>(type: "text", nullable: false),
                    Auth2Status = table.Column<int>(type: "integer", nullable: false),
                    Auth2Status1 = table.Column<int>(type: "integer", nullable: false),
                    Auth2Status2 = table.Column<int>(type: "integer", nullable: false),
                    Auth2Status3 = table.Column<int>(type: "integer", nullable: false),
                    Auth3Note = table.Column<string>(type: "text", nullable: false),
                    Auth3Status = table.Column<int>(type: "integer", nullable: false),
                    Network1Note = table.Column<string>(type: "text", nullable: false),
                    Network1Status = table.Column<int>(type: "integer", nullable: false),
                    Network1Note1 = table.Column<string>(type: "text", nullable: false),
                    Network1Note2 = table.Column<string>(type: "text", nullable: false),
                    Network1Note3 = table.Column<string>(type: "text", nullable: false),
                    Network1Note4 = table.Column<string>(type: "text", nullable: false),
                    Network1Note5 = table.Column<string>(type: "text", nullable: false),
                    Network1Note6 = table.Column<string>(type: "text", nullable: false),
                    Network1Note7 = table.Column<string>(type: "text", nullable: false),
                    Network1Status1 = table.Column<int>(type: "integer", nullable: false),
                    Network1Status2 = table.Column<int>(type: "integer", nullable: false),
                    Network1Status3 = table.Column<int>(type: "integer", nullable: false),
                    Network1Status4 = table.Column<int>(type: "integer", nullable: false),
                    Network1Status5 = table.Column<int>(type: "integer", nullable: false),
                    Network1Status6 = table.Column<int>(type: "integer", nullable: false),
                    Network1Status7 = table.Column<int>(type: "integer", nullable: false),
                    Network2Note = table.Column<string>(type: "text", nullable: false),
                    Network2Note1 = table.Column<string>(type: "text", nullable: false),
                    Network2Note2 = table.Column<string>(type: "text", nullable: false),
                    Network2Status = table.Column<int>(type: "integer", nullable: false),
                    Network2Status1 = table.Column<int>(type: "integer", nullable: false),
                    Network2Status2 = table.Column<int>(type: "integer", nullable: false),
                    Platform1Note = table.Column<string>(type: "text", nullable: false),
                    Platform1Status = table.Column<int>(type: "integer", nullable: false),
                    Platform1Note1 = table.Column<string>(type: "text", nullable: false),
                    Platform1Note2 = table.Column<string>(type: "text", nullable: false),
                    Platform1Note3 = table.Column<string>(type: "text", nullable: false),
                    Platform1Note4 = table.Column<string>(type: "text", nullable: false),
                    Platform1Note5 = table.Column<string>(type: "text", nullable: false),
                    Platform1Note6 = table.Column<string>(type: "text", nullable: false),
                    Platform1Note7 = table.Column<string>(type: "text", nullable: false),
                    Platform1Note8 = table.Column<string>(type: "text", nullable: false),
                    Platform1Note9 = table.Column<string>(type: "text", nullable: false),
                    Platform1Note10 = table.Column<string>(type: "text", nullable: false),
                    Platform1Note11 = table.Column<string>(type: "text", nullable: false),
                    Platform1Note12 = table.Column<string>(type: "text", nullable: false),
                    Platform1Note13 = table.Column<string>(type: "text", nullable: false),
                    Platform1Status1 = table.Column<int>(type: "integer", nullable: false),
                    Platform1Status2 = table.Column<int>(type: "integer", nullable: false),
                    Platform1Status3 = table.Column<int>(type: "integer", nullable: false),
                    Platform1Status4 = table.Column<int>(type: "integer", nullable: false),
                    Platform1Status5 = table.Column<int>(type: "integer", nullable: false),
                    Platform1Status6 = table.Column<int>(type: "integer", nullable: false),
                    Platform1Status7 = table.Column<int>(type: "integer", nullable: false),
                    Platform1Status8 = table.Column<int>(type: "integer", nullable: false),
                    Platform1Status9 = table.Column<int>(type: "integer", nullable: false),
                    Platform1Status10 = table.Column<int>(type: "integer", nullable: false),
                    Platform1Status11 = table.Column<int>(type: "integer", nullable: false),
                    Platform1Status12 = table.Column<int>(type: "integer", nullable: false),
                    Platform1Status13 = table.Column<int>(type: "integer", nullable: false),
                    Platform2Note = table.Column<string>(type: "text", nullable: false),
                    Platform2Status = table.Column<int>(type: "integer", nullable: false),
                    Platform2Note1 = table.Column<string>(type: "text", nullable: false),
                    Platform2Note2 = table.Column<string>(type: "text", nullable: false),
                    Platform2Note3 = table.Column<string>(type: "text", nullable: false),
                    Platform2Note4 = table.Column<string>(type: "text", nullable: false),
                    Platform2Note5 = table.Column<string>(type: "text", nullable: false),
                    Platform2Note6 = table.Column<string>(type: "text", nullable: false),
                    Platform2Note7 = table.Column<string>(type: "text", nullable: false),
                    Platform2Status1 = table.Column<int>(type: "integer", nullable: false),
                    Platform2Status2 = table.Column<int>(type: "integer", nullable: false),
                    Platform2Status3 = table.Column<int>(type: "integer", nullable: false),
                    Platform2Status4 = table.Column<int>(type: "integer", nullable: false),
                    Platform2Status5 = table.Column<int>(type: "integer", nullable: false),
                    Platform2Status6 = table.Column<int>(type: "integer", nullable: false),
                    Platform2Status7 = table.Column<int>(type: "integer", nullable: false),
                    Platform3Note = table.Column<string>(type: "text", nullable: false),
                    Platform3Status = table.Column<int>(type: "integer", nullable: false),
                    Platform3Note1 = table.Column<string>(type: "text", nullable: false),
                    Platform3Note2 = table.Column<string>(type: "text", nullable: false),
                    Platform3Note3 = table.Column<string>(type: "text", nullable: false),
                    Platform3Note4 = table.Column<string>(type: "text", nullable: false),
                    Platform3Note5 = table.Column<string>(type: "text", nullable: false),
                    Platform3Status1 = table.Column<int>(type: "integer", nullable: false),
                    Platform3Status2 = table.Column<int>(type: "integer", nullable: false),
                    Platform3Status3 = table.Column<int>(type: "integer", nullable: false),
                    Platform3Status4 = table.Column<int>(type: "integer", nullable: false),
                    Platform3Status5 = table.Column<int>(type: "integer", nullable: false),
                    Code1Note = table.Column<string>(type: "text", nullable: false),
                    Code1Status = table.Column<int>(type: "integer", nullable: false),
                    Code2Note = table.Column<string>(type: "text", nullable: false),
                    Code2Status = table.Column<int>(type: "integer", nullable: false),
                    Code2Note1 = table.Column<string>(type: "text", nullable: false),
                    Code2Note2 = table.Column<string>(type: "text", nullable: false),
                    Code2Status1 = table.Column<int>(type: "integer", nullable: false),
                    Code2Status2 = table.Column<int>(type: "integer", nullable: false),
                    Code3Note1 = table.Column<string>(type: "text", nullable: false),
                    Code3Note2 = table.Column<string>(type: "text", nullable: false),
                    Code3Note = table.Column<string>(type: "text", nullable: false),
                    Code3Status = table.Column<int>(type: "integer", nullable: false),
                    Code3Status1 = table.Column<int>(type: "integer", nullable: false),
                    Code3Status2 = table.Column<int>(type: "integer", nullable: false),
                    Code4Note = table.Column<string>(type: "text", nullable: false),
                    Code4Status = table.Column<int>(type: "integer", nullable: false),
                    Code4Note1 = table.Column<string>(type: "text", nullable: false),
                    Code4Note2 = table.Column<string>(type: "text", nullable: false),
                    Code4Note3 = table.Column<string>(type: "text", nullable: false),
                    Code4Note4 = table.Column<string>(type: "text", nullable: false),
                    Code4Note5 = table.Column<string>(type: "text", nullable: false),
                    Code4Note6 = table.Column<string>(type: "text", nullable: false),
                    Code4Note7 = table.Column<string>(type: "text", nullable: false),
                    Code4Note8 = table.Column<string>(type: "text", nullable: false),
                    Code4Note9 = table.Column<string>(type: "text", nullable: false),
                    Code4Note10 = table.Column<string>(type: "text", nullable: false),
                    Code4Status1 = table.Column<int>(type: "integer", nullable: false),
                    Code4Status2 = table.Column<int>(type: "integer", nullable: false),
                    Code4Status3 = table.Column<int>(type: "integer", nullable: false),
                    Code4Status4 = table.Column<int>(type: "integer", nullable: false),
                    Code4Status5 = table.Column<int>(type: "integer", nullable: false),
                    Code4Status6 = table.Column<int>(type: "integer", nullable: false),
                    Code4Status7 = table.Column<int>(type: "integer", nullable: false),
                    Code4Status8 = table.Column<int>(type: "integer", nullable: false),
                    Code4Status9 = table.Column<int>(type: "integer", nullable: false),
                    Code4Status10 = table.Column<int>(type: "integer", nullable: false),
                    Resilience1Note = table.Column<string>(type: "text", nullable: false),
                    Resilience1Status = table.Column<int>(type: "integer", nullable: false),
                    Resilience1Note1 = table.Column<string>(type: "text", nullable: false),
                    Resilience1Note2 = table.Column<string>(type: "text", nullable: false),
                    Resilience1Note3 = table.Column<string>(type: "text", nullable: false),
                    Resilience1Note4 = table.Column<string>(type: "text", nullable: false),
                    Resilience1Status1 = table.Column<int>(type: "integer", nullable: false),
                    Resilience1Status2 = table.Column<int>(type: "integer", nullable: false),
                    Resilience1Status3 = table.Column<int>(type: "integer", nullable: false),
                    Resilience1Status4 = table.Column<int>(type: "integer", nullable: false),
                    Resilience2Note = table.Column<string>(type: "text", nullable: false),
                    Resilience2Status = table.Column<int>(type: "integer", nullable: false),
                    Resilience2Note1 = table.Column<string>(type: "text", nullable: false),
                    Resilience2Note2 = table.Column<string>(type: "text", nullable: false),
                    Resilience2Note3 = table.Column<string>(type: "text", nullable: false),
                    Resilience2Note4 = table.Column<string>(type: "text", nullable: false),
                    Resilience2Note5 = table.Column<string>(type: "text", nullable: false),
                    Resilience2Status1 = table.Column<int>(type: "integer", nullable: false),
                    Resilience2Status2 = table.Column<int>(type: "integer", nullable: false),
                    Resilience2Status3 = table.Column<int>(type: "integer", nullable: false),
                    Resilience2Status4 = table.Column<int>(type: "integer", nullable: false),
                    Resilience2Status5 = table.Column<int>(type: "integer", nullable: false),
                    Resilience3Note = table.Column<string>(type: "text", nullable: false),
                    Resilience3Status = table.Column<int>(type: "integer", nullable: false),
                    Resilience3Note1 = table.Column<string>(type: "text", nullable: false),
                    Resilience3Note2 = table.Column<string>(type: "text", nullable: false),
                    Resilience3Note3 = table.Column<string>(type: "text", nullable: false),
                    Resilience3Note4 = table.Column<string>(type: "text", nullable: false),
                    Resilience3Note5 = table.Column<string>(type: "text", nullable: false),
                    Resilience3Note6 = table.Column<string>(type: "text", nullable: false),
                    Resilience3Status1 = table.Column<int>(type: "integer", nullable: false),
                    Resilience3Status2 = table.Column<int>(type: "integer", nullable: false),
                    Resilience3Status3 = table.Column<int>(type: "integer", nullable: false),
                    Resilience3Status4 = table.Column<int>(type: "integer", nullable: false),
                    Resilience3Status5 = table.Column<int>(type: "integer", nullable: false),
                    Resilience3Status6 = table.Column<int>(type: "integer", nullable: false),
                    Resilience4Note = table.Column<string>(type: "text", nullable: false),
                    Resilience4Status = table.Column<int>(type: "integer", nullable: false),
                    Resilience4Note1 = table.Column<string>(type: "text", nullable: false),
                    Resilience4Note2 = table.Column<string>(type: "text", nullable: false),
                    Resilience4Note3 = table.Column<string>(type: "text", nullable: false),
                    Resilience4Note4 = table.Column<string>(type: "text", nullable: false),
                    Resilience4Note5 = table.Column<string>(type: "text", nullable: false),
                    Resilience4Note6 = table.Column<string>(type: "text", nullable: false),
                    Resilience4Status1 = table.Column<int>(type: "integer", nullable: false),
                    Resilience4Status2 = table.Column<int>(type: "integer", nullable: false),
                    Resilience4Status3 = table.Column<int>(type: "integer", nullable: false),
                    Resilience4Status4 = table.Column<int>(type: "integer", nullable: false),
                    Resilience4Status5 = table.Column<int>(type: "integer", nullable: false),
                    Resilience4Status6 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MASTG", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MASTG_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MASTG_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MASTG_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TargetServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetServices_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TargetServices_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WSTG",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Info01Note = table.Column<string>(type: "text", nullable: false),
                    Info02Note = table.Column<string>(type: "text", nullable: false),
                    Info03Note = table.Column<string>(type: "text", nullable: false),
                    Info04Note = table.Column<string>(type: "text", nullable: false),
                    Info05Note = table.Column<string>(type: "text", nullable: false),
                    Info06Note = table.Column<string>(type: "text", nullable: false),
                    Info07Note = table.Column<string>(type: "text", nullable: false),
                    Info08Note = table.Column<string>(type: "text", nullable: false),
                    Info09Note = table.Column<string>(type: "text", nullable: false),
                    Info10Note = table.Column<string>(type: "text", nullable: false),
                    Conf01Note = table.Column<string>(type: "text", nullable: false),
                    Conf02Note = table.Column<string>(type: "text", nullable: false),
                    Conf03Note = table.Column<string>(type: "text", nullable: false),
                    Conf04Note = table.Column<string>(type: "text", nullable: false),
                    Conf05Note = table.Column<string>(type: "text", nullable: false),
                    Conf06Note = table.Column<string>(type: "text", nullable: false),
                    Conf07Note = table.Column<string>(type: "text", nullable: false),
                    Conf08Note = table.Column<string>(type: "text", nullable: false),
                    Conf09Note = table.Column<string>(type: "text", nullable: false),
                    Conf10Note = table.Column<string>(type: "text", nullable: false),
                    Conf11Note = table.Column<string>(type: "text", nullable: false),
                    Idnt1Note = table.Column<string>(type: "text", nullable: false),
                    Idnt2Note = table.Column<string>(type: "text", nullable: false),
                    Idnt3Note = table.Column<string>(type: "text", nullable: false),
                    Idnt4Note = table.Column<string>(type: "text", nullable: false),
                    Idnt5Note = table.Column<string>(type: "text", nullable: false),
                    Athn01Note = table.Column<string>(type: "text", nullable: false),
                    Athn02Note = table.Column<string>(type: "text", nullable: false),
                    Athn03Note = table.Column<string>(type: "text", nullable: false),
                    Athn04Note = table.Column<string>(type: "text", nullable: false),
                    Athn05Note = table.Column<string>(type: "text", nullable: false),
                    Athn06Note = table.Column<string>(type: "text", nullable: false),
                    Athn07Note = table.Column<string>(type: "text", nullable: false),
                    Athn08Note = table.Column<string>(type: "text", nullable: false),
                    Athn09Note = table.Column<string>(type: "text", nullable: false),
                    Athn10Note = table.Column<string>(type: "text", nullable: false),
                    Athz01Note = table.Column<string>(type: "text", nullable: false),
                    Athz02Note = table.Column<string>(type: "text", nullable: false),
                    Athz03Note = table.Column<string>(type: "text", nullable: false),
                    Athz04Note = table.Column<string>(type: "text", nullable: false),
                    Sess01Note = table.Column<string>(type: "text", nullable: false),
                    Sess02Note = table.Column<string>(type: "text", nullable: false),
                    Sess03Note = table.Column<string>(type: "text", nullable: false),
                    Sess04Note = table.Column<string>(type: "text", nullable: false),
                    Sess05Note = table.Column<string>(type: "text", nullable: false),
                    Sess06Note = table.Column<string>(type: "text", nullable: false),
                    Sess07Note = table.Column<string>(type: "text", nullable: false),
                    Sess08Note = table.Column<string>(type: "text", nullable: false),
                    Sess09Note = table.Column<string>(type: "text", nullable: false),
                    Inpv01Note = table.Column<string>(type: "text", nullable: false),
                    Inpv02Note = table.Column<string>(type: "text", nullable: false),
                    Inpv03Note = table.Column<string>(type: "text", nullable: false),
                    Inpv04Note = table.Column<string>(type: "text", nullable: false),
                    Inpv05Note = table.Column<string>(type: "text", nullable: false),
                    Inpv06Note = table.Column<string>(type: "text", nullable: false),
                    Inpv07Note = table.Column<string>(type: "text", nullable: false),
                    Inpv08Note = table.Column<string>(type: "text", nullable: false),
                    Inpv09Note = table.Column<string>(type: "text", nullable: false),
                    Inpv10Note = table.Column<string>(type: "text", nullable: false),
                    Inpv11Note = table.Column<string>(type: "text", nullable: false),
                    Inpv12Note = table.Column<string>(type: "text", nullable: false),
                    Inpv13Note = table.Column<string>(type: "text", nullable: false),
                    Inpv14Note = table.Column<string>(type: "text", nullable: false),
                    Inpv15Note = table.Column<string>(type: "text", nullable: false),
                    Inpv16Note = table.Column<string>(type: "text", nullable: false),
                    Inpv17Note = table.Column<string>(type: "text", nullable: false),
                    Inpv18Note = table.Column<string>(type: "text", nullable: false),
                    Inpv19Note = table.Column<string>(type: "text", nullable: false),
                    Errh01Note = table.Column<string>(type: "text", nullable: false),
                    Errh02Note = table.Column<string>(type: "text", nullable: false),
                    Cryp01Note = table.Column<string>(type: "text", nullable: false),
                    Cryp02Note = table.Column<string>(type: "text", nullable: false),
                    Cryp03Note = table.Column<string>(type: "text", nullable: false),
                    Cryp04Note = table.Column<string>(type: "text", nullable: false),
                    Busl01Note = table.Column<string>(type: "text", nullable: false),
                    Busl02Note = table.Column<string>(type: "text", nullable: false),
                    Busl03Note = table.Column<string>(type: "text", nullable: false),
                    Busl04Note = table.Column<string>(type: "text", nullable: false),
                    Busl05Note = table.Column<string>(type: "text", nullable: false),
                    Busl06Note = table.Column<string>(type: "text", nullable: false),
                    Busl07Note = table.Column<string>(type: "text", nullable: false),
                    Busl08Note = table.Column<string>(type: "text", nullable: false),
                    Busl09Note = table.Column<string>(type: "text", nullable: false),
                    Clnt01Note = table.Column<string>(type: "text", nullable: false),
                    Clnt02Note = table.Column<string>(type: "text", nullable: false),
                    Clnt03Note = table.Column<string>(type: "text", nullable: false),
                    Clnt04Note = table.Column<string>(type: "text", nullable: false),
                    Clnt05Note = table.Column<string>(type: "text", nullable: false),
                    Clnt06Note = table.Column<string>(type: "text", nullable: false),
                    Clnt07Note = table.Column<string>(type: "text", nullable: false),
                    Clnt08Note = table.Column<string>(type: "text", nullable: false),
                    Clnt09Note = table.Column<string>(type: "text", nullable: false),
                    Clnt10Note = table.Column<string>(type: "text", nullable: false),
                    Clnt11Note = table.Column<string>(type: "text", nullable: false),
                    Clnt12Note = table.Column<string>(type: "text", nullable: false),
                    Clnt13Note = table.Column<string>(type: "text", nullable: false),
                    Apit01Note = table.Column<string>(type: "text", nullable: false),
                    Info01Status = table.Column<int>(type: "integer", nullable: false),
                    Info02Status = table.Column<int>(type: "integer", nullable: false),
                    Info03Status = table.Column<int>(type: "integer", nullable: false),
                    Info04Status = table.Column<int>(type: "integer", nullable: false),
                    Info05Status = table.Column<int>(type: "integer", nullable: false),
                    Info06Status = table.Column<int>(type: "integer", nullable: false),
                    Info07Status = table.Column<int>(type: "integer", nullable: false),
                    Info08Status = table.Column<int>(type: "integer", nullable: false),
                    Info09Status = table.Column<int>(type: "integer", nullable: false),
                    Info10Status = table.Column<int>(type: "integer", nullable: false),
                    Conf01Status = table.Column<int>(type: "integer", nullable: false),
                    Conf02Status = table.Column<int>(type: "integer", nullable: false),
                    Conf03Status = table.Column<int>(type: "integer", nullable: false),
                    Conf04Status = table.Column<int>(type: "integer", nullable: false),
                    Conf05Status = table.Column<int>(type: "integer", nullable: false),
                    Conf06Status = table.Column<int>(type: "integer", nullable: false),
                    Conf07Status = table.Column<int>(type: "integer", nullable: false),
                    Conf08Status = table.Column<int>(type: "integer", nullable: false),
                    Conf09Status = table.Column<int>(type: "integer", nullable: false),
                    Conf10Status = table.Column<int>(type: "integer", nullable: false),
                    Conf11Status = table.Column<int>(type: "integer", nullable: false),
                    Idnt1Status = table.Column<int>(type: "integer", nullable: false),
                    Idnt2Status = table.Column<int>(type: "integer", nullable: false),
                    Idnt3Status = table.Column<int>(type: "integer", nullable: false),
                    Idnt4Status = table.Column<int>(type: "integer", nullable: false),
                    Idnt5Status = table.Column<int>(type: "integer", nullable: false),
                    Athn01Status = table.Column<int>(type: "integer", nullable: false),
                    Athn02Status = table.Column<int>(type: "integer", nullable: false),
                    Athn03Status = table.Column<int>(type: "integer", nullable: false),
                    Athn04Status = table.Column<int>(type: "integer", nullable: false),
                    Athn05Status = table.Column<int>(type: "integer", nullable: false),
                    Athn06Status = table.Column<int>(type: "integer", nullable: false),
                    Athn07Status = table.Column<int>(type: "integer", nullable: false),
                    Athn08Status = table.Column<int>(type: "integer", nullable: false),
                    Athn09Status = table.Column<int>(type: "integer", nullable: false),
                    Athn10Status = table.Column<int>(type: "integer", nullable: false),
                    Athz01Status = table.Column<int>(type: "integer", nullable: false),
                    Athz02Status = table.Column<int>(type: "integer", nullable: false),
                    Athz03Status = table.Column<int>(type: "integer", nullable: false),
                    Athz04Status = table.Column<int>(type: "integer", nullable: false),
                    Sess01Status = table.Column<int>(type: "integer", nullable: false),
                    Sess02Status = table.Column<int>(type: "integer", nullable: false),
                    Sess03Status = table.Column<int>(type: "integer", nullable: false),
                    Sess04Status = table.Column<int>(type: "integer", nullable: false),
                    Sess05Status = table.Column<int>(type: "integer", nullable: false),
                    Sess06Status = table.Column<int>(type: "integer", nullable: false),
                    Sess07Status = table.Column<int>(type: "integer", nullable: false),
                    Sess08Status = table.Column<int>(type: "integer", nullable: false),
                    Sess09Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv01Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv02Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv03Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv04Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv05Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv06Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv07Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv08Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv09Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv10Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv11Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv12Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv13Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv14Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv15Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv16Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv17Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv18Status = table.Column<int>(type: "integer", nullable: false),
                    Inpv19Status = table.Column<int>(type: "integer", nullable: false),
                    Errh01Status = table.Column<int>(type: "integer", nullable: false),
                    Errh02Status = table.Column<int>(type: "integer", nullable: false),
                    Cryp01Status = table.Column<int>(type: "integer", nullable: false),
                    Cryp02Status = table.Column<int>(type: "integer", nullable: false),
                    Cryp03Status = table.Column<int>(type: "integer", nullable: false),
                    Cryp04Status = table.Column<int>(type: "integer", nullable: false),
                    Busl01Status = table.Column<int>(type: "integer", nullable: false),
                    Busl02Status = table.Column<int>(type: "integer", nullable: false),
                    Busl03Status = table.Column<int>(type: "integer", nullable: false),
                    Busl04Status = table.Column<int>(type: "integer", nullable: false),
                    Busl05Status = table.Column<int>(type: "integer", nullable: false),
                    Busl06Status = table.Column<int>(type: "integer", nullable: false),
                    Busl07Status = table.Column<int>(type: "integer", nullable: false),
                    Busl08Status = table.Column<int>(type: "integer", nullable: false),
                    Busl09Status = table.Column<int>(type: "integer", nullable: false),
                    Clnt01Status = table.Column<int>(type: "integer", nullable: false),
                    Clnt02Status = table.Column<int>(type: "integer", nullable: false),
                    Clnt03Status = table.Column<int>(type: "integer", nullable: false),
                    Clnt04Status = table.Column<int>(type: "integer", nullable: false),
                    Clnt05Status = table.Column<int>(type: "integer", nullable: false),
                    Clnt06Status = table.Column<int>(type: "integer", nullable: false),
                    Clnt07Status = table.Column<int>(type: "integer", nullable: false),
                    Clnt08Status = table.Column<int>(type: "integer", nullable: false),
                    Clnt09Status = table.Column<int>(type: "integer", nullable: false),
                    Clnt10Status = table.Column<int>(type: "integer", nullable: false),
                    Clnt11Status = table.Column<int>(type: "integer", nullable: false),
                    Clnt12Status = table.Column<int>(type: "integer", nullable: false),
                    Clnt13Status = table.Column<int>(type: "integer", nullable: false),
                    Apit01Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WSTG", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WSTG_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WSTG_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WSTG_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskAttachments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskAttachments_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskNotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskNotes_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTargets_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskTargets_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jira",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VulnId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    JiraIdentifier = table.Column<string>(type: "text", nullable: false),
                    JiraKey = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Reporter = table.Column<string>(type: "text", nullable: false),
                    Assignee = table.Column<string>(type: "text", nullable: false),
                    JiraType = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Votes = table.Column<long>(type: "bigint", nullable: true),
                    Interested = table.Column<string>(type: "text", nullable: false),
                    JiraCreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    JiraUpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    JiraStatus = table.Column<string>(type: "text", nullable: false),
                    JiraComponent = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    JiraProject = table.Column<string>(type: "text", nullable: false),
                    Resolution = table.Column<string>(type: "text", nullable: false),
                    ResolutionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SecurityLevel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jira", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jira_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jira_Vulns_VulnId",
                        column: x => x.VulnId,
                        principalTable: "Vulns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VulnAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    VulnId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VulnAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VulnAttachments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VulnAttachments_Vulns_VulnId",
                        column: x => x.VulnId,
                        principalTable: "Vulns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VulnCwe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CweId = table.Column<int>(type: "integer", nullable: false),
                    VulnId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VulnCwe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VulnCwe_Cwe_CweId",
                        column: x => x.CweId,
                        principalTable: "Cwe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VulnCwe_Vulns_VulnId",
                        column: x => x.VulnId,
                        principalTable: "Vulns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VulnNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    VulnId = table.Column<Guid>(type: "uuid", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VulnNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VulnNotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VulnNotes_Vulns_VulnId",
                        column: x => x.VulnId,
                        principalTable: "Vulns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VulnTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VulnId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VulnTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VulnTargets_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VulnTargets_Vulns_VulnId",
                        column: x => x.VulnId,
                        principalTable: "Vulns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JiraComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JiraId = table.Column<Guid>(type: "uuid", nullable: false),
                    JiraIdComment = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    GroupLevel = table.Column<string>(type: "text", nullable: false),
                    RoleLevel = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateAuthor = table.Column<string>(type: "text", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JiraComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JiraComments_Jira_JiraId",
                        column: x => x.JiraId,
                        principalTable: "Jira",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_FromUserId",
                table: "ChatMessages",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ToUserId",
                table: "ChatMessages",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserId",
                table: "Clients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UserId",
                table: "Documents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Jira_UserId",
                table: "Jira",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Jira_VulnId",
                table: "Jira",
                column: "VulnId");

            migrationBuilder.CreateIndex(
                name: "IX_JiraComments_JiraId",
                table: "JiraComments",
                column: "JiraId");

            migrationBuilder.CreateIndex(
                name: "IX_MASTG_ProjectId",
                table: "MASTG",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MASTG_TargetId",
                table: "MASTG",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_MASTG_UserId",
                table: "MASTG",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId",
                table: "Notes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachments_ProjectId",
                table: "ProjectAttachments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachments_UserId",
                table: "ProjectAttachments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectNotes_ProjectId",
                table: "ProjectNotes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectNotes_UserId",
                table: "ProjectNotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ClientId",
                table: "Projects",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                table: "Projects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_ProjectId",
                table: "ProjectUsers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_UserId",
                table: "ProjectUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ProjectId",
                table: "Reports",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_UserId",
                table: "Reports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_UserId",
                table: "ReportTemplates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Targets_ProjectId",
                table: "Targets",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Targets_UserId",
                table: "Targets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetServices_TargetId",
                table: "TargetServices",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetServices_UserId",
                table: "TargetServices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_TaskId",
                table: "TaskAttachments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_UserId",
                table: "TaskAttachments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskNotes_TaskId",
                table: "TaskNotes",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskNotes_UserId",
                table: "TaskNotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AsignedUserId",
                table: "Tasks",
                column: "AsignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedUserId",
                table: "Tasks",
                column: "CreatedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId",
                table: "Tasks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTargets_TargetId",
                table: "TaskTargets",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTargets_TaskId",
                table: "TaskTargets",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Vaults_ProjectId",
                table: "Vaults",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Vaults_UserId",
                table: "Vaults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnAttachments_UserId",
                table: "VulnAttachments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnAttachments_VulnId",
                table: "VulnAttachments",
                column: "VulnId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnCwe_CweId",
                table: "VulnCwe",
                column: "CweId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnCwe_VulnId",
                table: "VulnCwe",
                column: "VulnId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnNotes_UserId",
                table: "VulnNotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnNotes_VulnId",
                table: "VulnNotes",
                column: "VulnId");

            migrationBuilder.CreateIndex(
                name: "IX_Vulns_ProjectId",
                table: "Vulns",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Vulns_UserId",
                table: "Vulns",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vulns_VulnCategoryId",
                table: "Vulns",
                column: "VulnCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnTargets_TargetId",
                table: "VulnTargets",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnTargets_VulnId",
                table: "VulnTargets",
                column: "VulnId");

            migrationBuilder.CreateIndex(
                name: "IX_WSTG_ProjectId",
                table: "WSTG",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WSTG_TargetId",
                table: "WSTG",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_WSTG_UserId",
                table: "WSTG",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "JiraComments");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "MASTG");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "ProjectAttachments");

            migrationBuilder.DropTable(
                name: "ProjectNotes");

            migrationBuilder.DropTable(
                name: "ProjectUsers");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "ReportTemplates");

            migrationBuilder.DropTable(
                name: "TargetServices");

            migrationBuilder.DropTable(
                name: "TaskAttachments");

            migrationBuilder.DropTable(
                name: "TaskNotes");

            migrationBuilder.DropTable(
                name: "TaskTargets");

            migrationBuilder.DropTable(
                name: "Vaults");

            migrationBuilder.DropTable(
                name: "VulnAttachments");

            migrationBuilder.DropTable(
                name: "VulnCwe");

            migrationBuilder.DropTable(
                name: "VulnNotes");

            migrationBuilder.DropTable(
                name: "VulnTargets");

            migrationBuilder.DropTable(
                name: "WSTG");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Jira");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Cwe");

            migrationBuilder.DropTable(
                name: "Targets");

            migrationBuilder.DropTable(
                name: "Vulns");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "VulnCategories");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
