using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    public partial class UpdatedVuln : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vulns_Projects_ProjectId",
                table: "Vulns");

            migrationBuilder.DropForeignKey(
                name: "FK_Vulns_VulnCategories_VulnCategoryId",
                table: "Vulns");

            migrationBuilder.AlterColumn<Guid>(
                name: "VulnCategoryId",
                table: "Vulns",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Vulns",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Vulns_Projects_ProjectId",
                table: "Vulns",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vulns_VulnCategories_VulnCategoryId",
                table: "Vulns",
                column: "VulnCategoryId",
                principalTable: "VulnCategories",
                principalColumn: "Id");
            
            migrationBuilder.DropForeignKey(
                name: "FK_Targets_Projects_ProjectId",
                table: "Targets");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Targets",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Targets_Projects_ProjectId",
                table: "Targets",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vulns_Projects_ProjectId",
                table: "Vulns");

            migrationBuilder.DropForeignKey(
                name: "FK_Vulns_VulnCategories_VulnCategoryId",
                table: "Vulns");

            migrationBuilder.AlterColumn<Guid>(
                name: "VulnCategoryId",
                table: "Vulns",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Vulns",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vulns_Projects_ProjectId",
                table: "Vulns",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vulns_VulnCategories_VulnCategoryId",
                table: "Vulns",
                column: "VulnCategoryId",
                principalTable: "VulnCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            
            migrationBuilder.DropForeignKey(
                name: "FK_Targets_Projects_ProjectId",
                table: "Targets");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Targets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Targets_Projects_ProjectId",
                table: "Targets",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
