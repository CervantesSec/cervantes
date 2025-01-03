using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedRss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RssSource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RssSource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RssSource_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RssNews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    SourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RssNews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RssNews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RssNews_RssSource_SourceId",
                        column: x => x.SourceId,
                        principalTable: "RssSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RssNews_SourceId",
                table: "RssNews",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_RssNews_UserId",
                table: "RssNews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RssSource_UserId",
                table: "RssSource",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RssNews");

            migrationBuilder.DropTable(
                name: "RssSource");
        }
    }
}
