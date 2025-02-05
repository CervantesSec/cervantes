using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedBusinessImpact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "RssSource",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "BusinessImpact",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RssCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RssCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RssSource_CategoryId",
                table: "RssSource",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RssSource_RssCategory_CategoryId",
                table: "RssSource",
                column: "CategoryId",
                principalTable: "RssCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RssSource_RssCategory_CategoryId",
                table: "RssSource");

            migrationBuilder.DropTable(
                name: "RssCategory");

            migrationBuilder.DropIndex(
                name: "IX_RssSource_CategoryId",
                table: "RssSource");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "RssSource");

            migrationBuilder.DropColumn(
                name: "BusinessImpact",
                table: "Projects");
        }
    }
}
