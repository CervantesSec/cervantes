using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedChatMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "ChatMessages",
                type: "vector",
                nullable: true,
                oldClrType: typeof(Vector),
                oldType: "vector");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "ChatMessages",
                type: "vector",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector",
                oldNullable: true);
        }
    }
}
