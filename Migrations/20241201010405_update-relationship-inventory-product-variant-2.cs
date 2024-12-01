using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lemoo_pos.Migrations
{
    /// <inheritdoc />
    public partial class updaterelationshipinventoryproductvariant2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_ProductVariants_ProductVariantId",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_InventoryId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_ProductVariantId",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "Inventories");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_InventoryId",
                table: "ProductVariants",
                column: "InventoryId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_InventoryId",
                table: "ProductVariants");

            migrationBuilder.AddColumn<long>(
                name: "ProductVariantId",
                table: "Inventories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_InventoryId",
                table: "ProductVariants",
                column: "InventoryId");

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
    }
}
