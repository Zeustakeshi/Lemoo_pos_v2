using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lemoo_pos.Migrations
{
    /// <inheritdoc />
    public partial class updateshifttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Shifts_ShiftId",
                table: "Orders");

            migrationBuilder.AddColumn<long>(
                name: "StoreId",
                table: "Shifts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "ShiftId",
                table: "Orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_StoreId",
                table: "Shifts",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Shifts_ShiftId",
                table: "Orders",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Stores_StoreId",
                table: "Shifts",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Shifts_ShiftId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Stores_StoreId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_StoreId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Shifts");

            migrationBuilder.AlterColumn<long>(
                name: "ShiftId",
                table: "Orders",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Shifts_ShiftId",
                table: "Orders",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id");
        }
    }
}
