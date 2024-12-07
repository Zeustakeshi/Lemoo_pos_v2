using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lemoo_pos.Migrations
{
    /// <inheritdoc />
    public partial class addopencloseshiftnote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Shifts",
                newName: "OpenNote");

            migrationBuilder.AddColumn<string>(
                name: "CloseNote",
                table: "Shifts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseNote",
                table: "Shifts");

            migrationBuilder.RenameColumn(
                name: "OpenNote",
                table: "Shifts",
                newName: "Note");
        }
    }
}
