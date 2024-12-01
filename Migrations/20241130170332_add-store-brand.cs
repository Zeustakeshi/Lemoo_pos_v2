using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lemoo_pos.Migrations
{
    /// <inheritdoc />
    public partial class addstorebrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StoreId",
                table: "Brands",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_StoreId",
                table: "Brands",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_Stores_StoreId",
                table: "Brands",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_Stores_StoreId",
                table: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_Brands_StoreId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Brands");
        }
    }
}
