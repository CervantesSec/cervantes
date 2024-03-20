using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cervantes.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EditedWstg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Idnt5Status",
                table: "WSTG",
                newName: "Idnt05Status");

            migrationBuilder.RenameColumn(
                name: "Idnt5Note",
                table: "WSTG",
                newName: "Idnt05Note");

            migrationBuilder.RenameColumn(
                name: "Idnt4Status",
                table: "WSTG",
                newName: "Idnt04Status");

            migrationBuilder.RenameColumn(
                name: "Idnt4Note",
                table: "WSTG",
                newName: "Idnt04Note");

            migrationBuilder.RenameColumn(
                name: "Idnt3Status",
                table: "WSTG",
                newName: "Idnt03Status");

            migrationBuilder.RenameColumn(
                name: "Idnt3Note",
                table: "WSTG",
                newName: "Idnt03Note");

            migrationBuilder.RenameColumn(
                name: "Idnt2Status",
                table: "WSTG",
                newName: "Idnt02Status");

            migrationBuilder.RenameColumn(
                name: "Idnt2Note",
                table: "WSTG",
                newName: "Idnt02Note");

            migrationBuilder.RenameColumn(
                name: "Idnt1Status",
                table: "WSTG",
                newName: "Idnt01Status");

            migrationBuilder.RenameColumn(
                name: "Idnt1Note",
                table: "WSTG",
                newName: "Idnt01Note");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Idnt05Status",
                table: "WSTG",
                newName: "Idnt5Status");

            migrationBuilder.RenameColumn(
                name: "Idnt05Note",
                table: "WSTG",
                newName: "Idnt5Note");

            migrationBuilder.RenameColumn(
                name: "Idnt04Status",
                table: "WSTG",
                newName: "Idnt4Status");

            migrationBuilder.RenameColumn(
                name: "Idnt04Note",
                table: "WSTG",
                newName: "Idnt4Note");

            migrationBuilder.RenameColumn(
                name: "Idnt03Status",
                table: "WSTG",
                newName: "Idnt3Status");

            migrationBuilder.RenameColumn(
                name: "Idnt03Note",
                table: "WSTG",
                newName: "Idnt3Note");

            migrationBuilder.RenameColumn(
                name: "Idnt02Status",
                table: "WSTG",
                newName: "Idnt2Status");

            migrationBuilder.RenameColumn(
                name: "Idnt02Note",
                table: "WSTG",
                newName: "Idnt2Note");

            migrationBuilder.RenameColumn(
                name: "Idnt01Status",
                table: "WSTG",
                newName: "Idnt1Status");

            migrationBuilder.RenameColumn(
                name: "Idnt01Note",
                table: "WSTG",
                newName: "Idnt1Note");
        }
    }
}
