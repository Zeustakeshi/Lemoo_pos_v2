using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lemoo_pos.Migrations
{
    /// <inheritdoc />
    public partial class addproductvariantquantityandimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowSale",
                table: "ProductVariants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "ProductVariants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Quantity",
                table: "ProductVariants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowSale",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductVariants");
        }
    }
}
