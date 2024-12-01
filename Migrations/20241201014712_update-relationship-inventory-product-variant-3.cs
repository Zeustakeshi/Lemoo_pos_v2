using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lemoo_pos.Migrations
{
    /// <inheritdoc />
    public partial class updaterelationshipinventoryproductvariant3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Inventories_InventoryId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_InventoryId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "ProductVariants");

            migrationBuilder.AlterColumn<long>(
                name: "Quantity",
                table: "Inventories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Available",
                table: "Inventories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductVariantId",
                table: "Inventories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_ProductVariantId",
                table: "Inventories",
                column: "ProductVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_ProductVariants_ProductVariantId",
                table: "Inventories",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_ProductVariants_ProductVariantId",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_ProductVariantId",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "Inventories");

            migrationBuilder.AddColumn<long>(
                name: "InventoryId",
                table: "ProductVariants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "Quantity",
                table: "Inventories",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Available",
                table: "Inventories",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_InventoryId",
                table: "ProductVariants",
                column: "InventoryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Inventories_InventoryId",
                table: "ProductVariants",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
