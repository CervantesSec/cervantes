using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedReportsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HtmlCode",
                table: "ReportTemplates");

            migrationBuilder.AddColumn<string>(
                name: "HtmlCode",
                table: "Reports",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HtmlCode",
                table: "Reports");

            migrationBuilder.AddColumn<string>(
                name: "HtmlCode",
                table: "ReportTemplates",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
