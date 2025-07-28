using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedCVEIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cves",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CveId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CvssV3BaseScore = table.Column<double>(type: "double precision", nullable: true),
                    CvssV3Vector = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CvssV3Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CvssV2BaseScore = table.Column<double>(type: "double precision", nullable: true),
                    CvssV2Vector = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CvssV2Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EpssScore = table.Column<double>(type: "double precision", nullable: true),
                    EpssPercentile = table.Column<double>(type: "double precision", nullable: true),
                    IsKnownExploited = table.Column<bool>(type: "boolean", nullable: false),
                    KevDueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PrimaryCweId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PrimaryCweName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    State = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AssignerOrgId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SourceIdentifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cves_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CveSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Vendor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Product = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Keywords = table.Column<string>(type: "text", nullable: false),
                    MinCvssScore = table.Column<double>(type: "double precision", nullable: true),
                    MaxCvssScore = table.Column<double>(type: "double precision", nullable: true),
                    MinEpssScore = table.Column<double>(type: "double precision", nullable: true),
                    OnlyKnownExploited = table.Column<bool>(type: "boolean", nullable: false),
                    CweFilter = table.Column<string>(type: "text", nullable: false),
                    NotificationFrequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NotificationMethod = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    WebhookUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CveSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CveSubscriptions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CveSubscriptions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CveSyncSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    ApiKey = table.Column<string>(type: "text", nullable: true),
                    RateLimitPerMinute = table.Column<int>(type: "integer", nullable: false),
                    LastSyncDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSyncStatus = table.Column<string>(type: "text", nullable: false),
                    LastSyncError = table.Column<string>(type: "text", nullable: true),
                    SyncedCveCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CveSyncSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CveConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CveId = table.Column<Guid>(type: "uuid", nullable: false),
                    CpeUri = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Vendor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Product = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VersionStartIncluding = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VersionStartExcluding = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VersionEndIncluding = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VersionEndExcluding = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsVulnerable = table.Column<bool>(type: "boolean", nullable: false),
                    RunningOn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CveConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CveConfigurations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CveConfigurations_Cves_CveId",
                        column: x => x.CveId,
                        principalTable: "Cves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CveCwe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CveId = table.Column<Guid>(type: "uuid", nullable: false),
                    CweId = table.Column<int>(type: "integer", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    Source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CveCwe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CveCwe_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CveCwe_Cves_CveId",
                        column: x => x.CveId,
                        principalTable: "Cves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CveCwe_Cwe_CweId",
                        column: x => x.CweId,
                        principalTable: "Cwe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CveCweMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CveId = table.Column<Guid>(type: "uuid", nullable: false),
                    CweId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CveCweMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CveCweMappings_Cves_CveId",
                        column: x => x.CveId,
                        principalTable: "Cves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CveCweMappings_Cwe_CweId",
                        column: x => x.CweId,
                        principalTable: "Cwe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CveProjectMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CveId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelevanceScore = table.Column<double>(type: "double precision", nullable: false),
                    IsAutomatic = table.Column<bool>(type: "boolean", nullable: false),
                    IsValidated = table.Column<bool>(type: "boolean", nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CveProjectMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CveProjectMappings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CveProjectMappings_Cves_CveId",
                        column: x => x.CveId,
                        principalTable: "Cves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CveProjectMappings_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CveReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CveId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ReferenceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CveReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CveReferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CveReferences_Cves_CveId",
                        column: x => x.CveId,
                        principalTable: "Cves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CveTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CveId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsSystemTag = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CveTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CveTags_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CveTags_Cves_CveId",
                        column: x => x.CveId,
                        principalTable: "Cves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VulnCve",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VulnId = table.Column<Guid>(type: "uuid", nullable: false),
                    CveId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelevanceScore = table.Column<double>(type: "double precision", nullable: false),
                    IsAutomatic = table.Column<bool>(type: "boolean", nullable: false),
                    IsValidated = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VulnCve", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VulnCve_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VulnCve_Cves_CveId",
                        column: x => x.CveId,
                        principalTable: "Cves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VulnCve_Vulns_VulnId",
                        column: x => x.VulnId,
                        principalTable: "Vulns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CveNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CveId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    NotificationType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsSent = table.Column<bool>(type: "boolean", nullable: false),
                    Method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    NextRetryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReadDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CveNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CveNotifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CveNotifications_CveSubscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "CveSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CveNotifications_Cves_CveId",
                        column: x => x.CveId,
                        principalTable: "Cves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CveConfigurations_CveId",
                table: "CveConfigurations",
                column: "CveId");

            migrationBuilder.CreateIndex(
                name: "IX_CveConfigurations_UserId",
                table: "CveConfigurations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CveCwe_CveId",
                table: "CveCwe",
                column: "CveId");

            migrationBuilder.CreateIndex(
                name: "IX_CveCwe_CweId",
                table: "CveCwe",
                column: "CweId");

            migrationBuilder.CreateIndex(
                name: "IX_CveCwe_UserId",
                table: "CveCwe",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CveCweMappings_CveId",
                table: "CveCweMappings",
                column: "CveId");

            migrationBuilder.CreateIndex(
                name: "IX_CveCweMappings_CweId",
                table: "CveCweMappings",
                column: "CweId");

            migrationBuilder.CreateIndex(
                name: "IX_CveNotifications_CveId",
                table: "CveNotifications",
                column: "CveId");

            migrationBuilder.CreateIndex(
                name: "IX_CveNotifications_SubscriptionId",
                table: "CveNotifications",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_CveNotifications_UserId",
                table: "CveNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CveProjectMappings_CveId",
                table: "CveProjectMappings",
                column: "CveId");

            migrationBuilder.CreateIndex(
                name: "IX_CveProjectMappings_ProjectId",
                table: "CveProjectMappings",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CveProjectMappings_UserId",
                table: "CveProjectMappings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CveReferences_CveId",
                table: "CveReferences",
                column: "CveId");

            migrationBuilder.CreateIndex(
                name: "IX_CveReferences_UserId",
                table: "CveReferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cves_CveId",
                table: "Cves",
                column: "CveId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cves_UserId",
                table: "Cves",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CveSubscriptions_ProjectId",
                table: "CveSubscriptions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CveSubscriptions_UserId",
                table: "CveSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CveSyncSources_Name",
                table: "CveSyncSources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CveTags_CveId",
                table: "CveTags",
                column: "CveId");

            migrationBuilder.CreateIndex(
                name: "IX_CveTags_UserId",
                table: "CveTags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnCve_CveId",
                table: "VulnCve",
                column: "CveId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnCve_UserId",
                table: "VulnCve",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnCve_VulnId",
                table: "VulnCve",
                column: "VulnId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CveConfigurations");

            migrationBuilder.DropTable(
                name: "CveCwe");

            migrationBuilder.DropTable(
                name: "CveCweMappings");

            migrationBuilder.DropTable(
                name: "CveNotifications");

            migrationBuilder.DropTable(
                name: "CveProjectMappings");

            migrationBuilder.DropTable(
                name: "CveReferences");

            migrationBuilder.DropTable(
                name: "CveSyncSources");

            migrationBuilder.DropTable(
                name: "CveTags");

            migrationBuilder.DropTable(
                name: "VulnCve");

            migrationBuilder.DropTable(
                name: "CveSubscriptions");

            migrationBuilder.DropTable(
                name: "Cves");
        }
    }
}
