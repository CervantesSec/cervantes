using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    public partial class UpdatedMASTG : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlatformNote01",
                table: "MASTG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlatformNote02",
                table: "MASTG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlatformNote03",
                table: "MASTG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlatformNote04",
                table: "MASTG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlatformNote05",
                table: "MASTG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlatformNote06",
                table: "MASTG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlatformNote07",
                table: "MASTG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlatformNote08",
                table: "MASTG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlatformNote09",
                table: "MASTG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlatformNote10",
                table: "MASTG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlatformNote11",
                table: "MASTG",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlatformStatus01",
                table: "MASTG",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlatformStatus02",
                table: "MASTG",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlatformStatus03",
                table: "MASTG",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlatformStatus04",
                table: "MASTG",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlatformStatus05",
                table: "MASTG",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlatformStatus06",
                table: "MASTG",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlatformStatus07",
                table: "MASTG",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlatformStatus08",
                table: "MASTG",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlatformStatus09",
                table: "MASTG",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlatformStatus10",
                table: "MASTG",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlatformStatus11",
                table: "MASTG",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlatformNote01",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformNote02",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformNote03",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformNote04",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformNote05",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformNote06",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformNote07",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformNote08",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformNote09",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformNote10",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformNote11",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformStatus01",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformStatus02",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformStatus03",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformStatus04",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformStatus05",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformStatus06",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformStatus07",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformStatus08",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformStatus09",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformStatus10",
                table: "MASTG");

            migrationBuilder.DropColumn(
                name: "PlatformStatus11",
                table: "MASTG");
        }
    }
}
