using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lemoo_pos.Migrations
{
    /// <inheritdoc />
    public partial class addbranchinfomationv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistrictCode",
                table: "Branches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceCode",
                table: "Branches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardCode",
                table: "Branches",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "ProvinceCode",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "WardCode",
                table: "Branches");
        }
    }
}
