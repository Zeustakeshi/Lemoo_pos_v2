using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lemoo_pos.Migrations
{
    /// <inheritdoc />
    public partial class removeproductvariantquantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductVariants");

            migrationBuilder.AddColumn<bool>(
                name: "AllowNegativeInventory",
                table: "ProductVariants",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowNegativeInventory",
                table: "ProductVariants");

            migrationBuilder.AddColumn<long>(
                name: "Quantity",
                table: "ProductVariants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
