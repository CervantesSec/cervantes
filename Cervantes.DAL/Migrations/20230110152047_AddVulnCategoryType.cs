using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    public partial class AddVulnCategoryType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "VulnCategories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "VulnCategories");
        }
    }
}
