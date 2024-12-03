using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lemoo_pos.Migrations
{
    /// <inheritdoc />
    public partial class addgenderstaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Staffs",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Staffs");
        }
    }
}
