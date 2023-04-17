using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    public partial class AddedCwe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cwe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cwe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VulnCwe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CweId = table.Column<int>(type: "integer", nullable: false),
                    VulnId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VulnCwe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VulnCwe_Cwe_CweId",
                        column: x => x.CweId,
                        principalTable: "Cwe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VulnCwe_Vulns_VulnId",
                        column: x => x.VulnId,
                        principalTable: "Vulns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VulnCwe_CweId",
                table: "VulnCwe",
                column: "CweId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnCwe_VulnId",
                table: "VulnCwe",
                column: "VulnId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VulnCwe");

            migrationBuilder.DropTable(
                name: "Cwe");
        }
    }
}
