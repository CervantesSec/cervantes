using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    public partial class AddOWASPScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OWASPScore",
                table: "Vulns",
                newName: "OWASPImpact");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OWASPImpact",
                table: "Vulns",
                newName: "OWASPScore");
        }
    }
}
