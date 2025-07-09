using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedCustomChecklists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChecklistTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: true),
                    IsSystemTemplate = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistTemplates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChecklistTemplates_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ChecklistTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistCategories_ChecklistTemplates_ChecklistTemplateId",
                        column: x => x.ChecklistTemplateId,
                        principalTable: "ChecklistTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Checklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ChecklistTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CompletionPercentage = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checklists_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checklists_ChecklistTemplates_ChecklistTemplateId",
                        column: x => x.ChecklistTemplateId,
                        principalTable: "ChecklistTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checklists_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Checklists_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Objectives = table.Column<string>(type: "text", nullable: true),
                    TestProcedure = table.Column<string>(type: "text", nullable: true),
                    PassCriteria = table.Column<string>(type: "text", nullable: true),
                    ChecklistCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    References = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItems_ChecklistCategories_ChecklistCategoryId",
                        column: x => x.ChecklistCategoryId,
                        principalTable: "ChecklistCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChecklistId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChecklistItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Evidence = table.Column<string>(type: "text", nullable: true),
                    TestedByUserId = table.Column<string>(type: "text", nullable: true),
                    TestedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstimatedTimeMinutes = table.Column<int>(type: "integer", nullable: true),
                    ActualTimeMinutes = table.Column<int>(type: "integer", nullable: true),
                    DifficultyRating = table.Column<int>(type: "integer", nullable: true),
                    VulnId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistExecutions_AspNetUsers_TestedByUserId",
                        column: x => x.TestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChecklistExecutions_ChecklistItems_ChecklistItemId",
                        column: x => x.ChecklistItemId,
                        principalTable: "ChecklistItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChecklistExecutions_Checklists_ChecklistId",
                        column: x => x.ChecklistId,
                        principalTable: "Checklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistExecutions_Vulns_VulnId",
                        column: x => x.VulnId,
                        principalTable: "Vulns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistCategories_ChecklistTemplateId",
                table: "ChecklistCategories",
                column: "ChecklistTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistExecutions_ChecklistId",
                table: "ChecklistExecutions",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistExecutions_ChecklistItemId",
                table: "ChecklistExecutions",
                column: "ChecklistItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistExecutions_TestedByUserId",
                table: "ChecklistExecutions",
                column: "TestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistExecutions_VulnId",
                table: "ChecklistExecutions",
                column: "VulnId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItems_ChecklistCategoryId",
                table: "ChecklistItems",
                column: "ChecklistCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_ChecklistTemplateId",
                table: "Checklists",
                column: "ChecklistTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_ProjectId",
                table: "Checklists",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_TargetId",
                table: "Checklists",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_UserId",
                table: "Checklists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplates_OrganizationId",
                table: "ChecklistTemplates",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplates_UserId",
                table: "ChecklistTemplates",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistExecutions");

            migrationBuilder.DropTable(
                name: "ChecklistItems");

            migrationBuilder.DropTable(
                name: "Checklists");

            migrationBuilder.DropTable(
                name: "ChecklistCategories");

            migrationBuilder.DropTable(
                name: "ChecklistTemplates");
        }
    }
}
