using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_AspNetUsers_FromUserId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_AspNetUsers_ToUserId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_FromUserId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ToUserId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "FromUserId",
                table: "ChatMessages");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "ChatMessages",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "ChatMessages",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "ChatMessages",
                newName: "Timestamp");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ChatMessages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "ChatMessages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChatId",
                table: "ChatMessages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "ChatMessages",
                type: "vector",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "MessageIndex",
                table: "ChatMessages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastMessageAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chats_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ApplicationUserId",
                table: "ChatMessages",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ApplicationUserId1",
                table: "ChatMessages",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UserId",
                table: "Chats",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_AspNetUsers_ApplicationUserId",
                table: "ChatMessages",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_AspNetUsers_ApplicationUserId1",
                table: "ChatMessages",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_AspNetUsers_ApplicationUserId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_AspNetUsers_ApplicationUserId1",
                table: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ApplicationUserId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ApplicationUserId1",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "MessageIndex",
                table: "ChatMessages");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "ChatMessages",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "ChatMessages",
                newName: "ToUserId");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "ChatMessages",
                newName: "Message");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.AddColumn<string>(
                name: "FromUserId",
                table: "ChatMessages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_FromUserId",
                table: "ChatMessages",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ToUserId",
                table: "ChatMessages",
                column: "ToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_AspNetUsers_FromUserId",
                table: "ChatMessages",
                column: "FromUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_AspNetUsers_ToUserId",
                table: "ChatMessages",
                column: "ToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
