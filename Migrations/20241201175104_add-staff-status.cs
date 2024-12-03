using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lemoo_pos.Migrations
{
    /// <inheritdoc />
    public partial class addstaffstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Staffs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Staffs");
        }
    }
}
