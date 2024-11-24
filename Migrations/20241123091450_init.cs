using System;
using Lemoo_pos.Common.Enums;
using Lemoo_pos.Models.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Lemoo_pos.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Avatar = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Authorities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StoreId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Authorities_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StoreId = table.Column<long>(type: "bigint", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branches_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountAuthorities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    AuthorityId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountAuthorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountAuthorities_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountAuthorities_Authorities_AuthorityId",
                        column: x => x.AuthorityId,
                        principalTable: "Authorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorityPermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorityId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorityPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorityPermissions_Authorities_AuthorityId",
                        column: x => x.AuthorityId,
                        principalTable: "Authorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorityPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Staffs_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Staffs_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountAuthorities_AccountId",
                table: "AccountAuthorities",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountAuthorities_AuthorityId",
                table: "AccountAuthorities",
                column: "AuthorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Authorities_StoreId",
                table: "Authorities",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorityPermissions_AuthorityId",
                table: "AuthorityPermissions",
                column: "AuthorityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorityPermissions_PermissionId",
                table: "AuthorityPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_StoreId",
                table: "Branches",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_AccountId",
                table: "Staffs",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_BranchId",
                table: "Staffs",
                column: "BranchId");

            migrationBuilder.InsertData(
               table: "Permissions",
               columns: new[] { "Type", "CreatedAt", "UpdatedAt" },
               values: new object[,] {
                    { PermissionType.VIEW_ALL_STAFF.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.IMPORT_STAFF_FILE.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.EXPORT_STAFF_FILE.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.ADD_STAFF.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.EDIT_STAFF.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.DELETE_STAFF.ToString(), DateTime.UtcNow, DateTime.UtcNow },

                    { PermissionType.VIEW_ALL_PRODUCT.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.IMPORT_PRODUCT_FILE.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.EXPORT_PRODUCT_FILE.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.ADD_PRODUCT.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.EDIT_PRODUCT.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.DELETE_PRODUCT.ToString(), DateTime.UtcNow, DateTime.UtcNow },

                    { PermissionType.ASSIGN_ROLE.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.MANAGE_BRANCH.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.VIEW_STORE_INFO.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.VIEW_ACTIVITY_LOG.ToString(), DateTime.UtcNow, DateTime.UtcNow },

                    { PermissionType.VIEW_ASSIGNED_CUSTOMER.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.VIEW_ALL_CUSTOMER.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.ADD_CUSTOMER.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.EDIT_CUSTOMER.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.DELETE_CUSTOMER.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.IMPORT_CUSTOMER_FILE.ToString(), DateTime.UtcNow, DateTime.UtcNow },

                    { PermissionType.VIEW_ASSIGNED_ORDER.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.VIEW_ALL_ORDER.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.ADD_ORDER.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.EDIT_ORDER.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.APPROVE_ORDER.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.CANCEL_ORDER.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.EXPORT_ORDER_FILE.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.IMPORT_ORDER_FILE.ToString(), DateTime.UtcNow, DateTime.UtcNow },

                    { PermissionType.VIEW_AUDIT_REPORT.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.CREATE_AUDIT_REPORT.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.DELETE_AUDIT_REPORT.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.EDIT_AUDIT_REPORT.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.BALANCE_WAREHOUSE.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.EXPORT_AUDIT_FILE.ToString(), DateTime.UtcNow, DateTime.UtcNow },
                    { PermissionType.IMPORT_AUDIT_FILE.ToString(), DateTime.UtcNow, DateTime.UtcNow }
               }
               );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountAuthorities");

            migrationBuilder.DropTable(
                name: "AuthorityPermissions");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "Authorities");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Stores");
        }
    }
}
