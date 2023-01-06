using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    public partial class AddOWASPScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OWASPLikehood",
                table: "Vulns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OWASPRisk",
                table: "Vulns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OWASPScore",
                table: "Vulns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OWASPVector",
                table: "Vulns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OWASPLikehood",
                table: "Vulns");

            migrationBuilder.DropColumn(
                name: "OWASPRisk",
                table: "Vulns");

            migrationBuilder.DropColumn(
                name: "OWASPScore",
                table: "Vulns");

            migrationBuilder.DropColumn(
                name: "OWASPVector",
                table: "Vulns");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Projects");
        }
    }
}
