using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryms.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_products_sku",
                table: "products");

            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "warehouses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "suppliers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "stock_transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "purchase_orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "inventory_stock",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "categories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    slug = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_tenants",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    joined_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tenants", x => new { x.user_id, x.tenant_id });
                    table.ForeignKey(
                        name: "FK_user_tenants_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_tenants_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_warehouses_tenant_name",
                table: "warehouses",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_tenant_id",
                table: "suppliers",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_transactions_tenant_id",
                table: "stock_transactions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_orders_tenant_id",
                table: "purchase_orders",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "idx_products_tenant_sku",
                table: "products",
                columns: new[] { "tenant_id", "sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inventory_stock_tenant_id",
                table: "inventory_stock",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "idx_categories_tenant_name",
                table: "categories",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_slug",
                table: "tenants",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_tenants_tenant_id",
                table: "user_tenants",
                column: "tenant_id");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_tenants_tenant_id",
                table: "categories",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_stock_tenants_tenant_id",
                table: "inventory_stock",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_tenants_tenant_id",
                table: "products",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_orders_tenants_tenant_id",
                table: "purchase_orders",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_transactions_tenants_tenant_id",
                table: "stock_transactions",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_suppliers_tenants_tenant_id",
                table: "suppliers",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_warehouses_tenants_tenant_id",
                table: "warehouses",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_tenants_tenant_id",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_stock_tenants_tenant_id",
                table: "inventory_stock");

            migrationBuilder.DropForeignKey(
                name: "FK_products_tenants_tenant_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_orders_tenants_tenant_id",
                table: "purchase_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_transactions_tenants_tenant_id",
                table: "stock_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_suppliers_tenants_tenant_id",
                table: "suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_warehouses_tenants_tenant_id",
                table: "warehouses");

            migrationBuilder.DropTable(
                name: "user_tenants");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropIndex(
                name: "idx_warehouses_tenant_name",
                table: "warehouses");

            migrationBuilder.DropIndex(
                name: "IX_suppliers_tenant_id",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "IX_stock_transactions_tenant_id",
                table: "stock_transactions");

            migrationBuilder.DropIndex(
                name: "IX_purchase_orders_tenant_id",
                table: "purchase_orders");

            migrationBuilder.DropIndex(
                name: "idx_products_tenant_sku",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_inventory_stock_tenant_id",
                table: "inventory_stock");

            migrationBuilder.DropIndex(
                name: "idx_categories_tenant_name",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "warehouses");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "stock_transactions");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "purchase_orders");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "inventory_stock");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "categories");

            migrationBuilder.CreateIndex(
                name: "IX_products_sku",
                table: "products",
                column: "sku",
                unique: true);
        }
    }
}
