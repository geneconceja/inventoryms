using inventoryms.Models;
using inventoryms.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace inventoryms.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryStock> InventoryStocks => Set<InventoryStock>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Native PostgreSQL Enums
        modelBuilder.HasPostgresEnum<TransactionType>("transaction_type");
        modelBuilder.HasPostgresEnum<OrderStatus>("order_status");

        // ---------- Identity: users, roles, user_roles ----------
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("users");
            entity.Property(u => u.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            entity.Property(u => u.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
            entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired();
            entity.Property(u => u.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(u => u.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").HasDefaultValueSql("now()");

            entity.Property(u => u.UserName).HasColumnName("user_name");
            entity.Property(u => u.NormalizedUserName).HasColumnName("normalized_user_name");
            entity.Property(u => u.NormalizedEmail).HasColumnName("normalized_email");
            entity.Property(u => u.EmailConfirmed).HasColumnName("email_confirmed");
            entity.Property(u => u.SecurityStamp).HasColumnName("security_stamp");
            entity.Property(u => u.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            entity.Property(u => u.PhoneNumber).HasColumnName("phone_number");
            entity.Property(u => u.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
            entity.Property(u => u.TwoFactorEnabled).HasColumnName("two_factor_enabled");
            entity.Property(u => u.LockoutEnd).HasColumnName("lockout_end");
            entity.Property(u => u.LockoutEnabled).HasColumnName("lockout_enabled");
            entity.Property(u => u.AccessFailedCount).HasColumnName("access_failed_count");
        });

        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("roles");
            entity.Property(r => r.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            entity.Property(r => r.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
            entity.HasIndex(r => r.Name).IsUnique();
            entity.Property(r => r.NormalizedName).HasColumnName("normalized_name");
            entity.Property(r => r.ConcurrencyStamp).HasColumnName("concurrency_stamp");
        });

        modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable("user_roles");
            entity.Property(ur => ur.UserId).HasColumnName("user_id");
            entity.Property(ur => ur.RoleId).HasColumnName("role_id");
        });

        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");

        // ---------- Categories ----------
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.Property(c => c.Id).HasColumnName("id").UseIdentityByDefaultColumn();
            entity.Property(c => c.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(c => c.Description).HasColumnName("description").HasMaxLength(500);
        });

        // ---------- Suppliers ----------
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("suppliers");
            entity.Property(s => s.Id).HasColumnName("id").UseIdentityByDefaultColumn();
            entity.Property(s => s.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.Property(s => s.ContactName).HasColumnName("contact_name").HasMaxLength(100);
            entity.Property(s => s.Email).HasColumnName("email").HasMaxLength(100);
            entity.Property(s => s.Phone).HasColumnName("phone").HasMaxLength(30);
            entity.Property(s => s.Address).HasColumnName("address").HasMaxLength(250);
        });

        // ---------- Warehouses ----------
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("warehouses");
            entity.Property(w => w.Id).HasColumnName("id").UseIdentityByDefaultColumn();
            entity.Property(w => w.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(w => w.Location).HasColumnName("location").HasMaxLength(250);
        });

        // ---------- Products ----------
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products", t =>
            {
                t.HasCheckConstraint("CK_products_unit_price", "unit_price >= 0");
                t.HasCheckConstraint("CK_products_reorder_level", "reorder_level >= 0");
            });

            entity.Property(p => p.Id).HasColumnName("id").UseIdentityByDefaultColumn();
            entity.Property(p => p.Sku).HasColumnName("sku").HasMaxLength(40).IsRequired();
            entity.HasIndex(p => p.Sku).IsUnique();

            entity.Property(p => p.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.Property(p => p.Description).HasColumnName("description").HasMaxLength(1000);
            entity.Property(p => p.CategoryId).HasColumnName("category_id");
            entity.Property(p => p.SupplierId).HasColumnName("supplier_id");
            entity.Property(p => p.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 2).HasDefaultValue(0.00m);
            entity.Property(p => p.ReorderLevel).HasColumnName("reorder_level").HasDefaultValue(10);
            entity.Property(p => p.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").HasDefaultValueSql("now()");

            entity.HasIndex(p => p.CategoryId).HasDatabaseName("idx_products_category");
            entity.HasIndex(p => p.SupplierId).HasDatabaseName("idx_products_supplier");

            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ---------- Inventory Stock ----------
        modelBuilder.Entity<InventoryStock>(entity =>
        {
            entity.ToTable("inventory_stock", t =>
            {
                t.HasCheckConstraint("CK_inventory_stock_quantity", "quantity >= 0");
            });

            entity.Property(i => i.Id).HasColumnName("id").UseIdentityByDefaultColumn();
            entity.Property(i => i.ProductId).HasColumnName("product_id").IsRequired();
            entity.Property(i => i.WarehouseId).HasColumnName("warehouse_id").IsRequired();
            entity.Property(i => i.Quantity).HasColumnName("quantity").HasDefaultValue(0);

            entity.HasIndex(i => new { i.ProductId, i.WarehouseId }).IsUnique();
            entity.HasIndex(i => i.ProductId).HasDatabaseName("idx_inventory_stock_product");
            entity.HasIndex(i => i.WarehouseId).HasDatabaseName("idx_inventory_stock_warehouse");

            entity.HasOne(i => i.Product)
                .WithMany(p => p.InventoryStocks)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.Warehouse)
                .WithMany(w => w.InventoryStocks)
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Stock Transactions ----------
        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.ToTable("stock_transactions", t =>
            {
                t.HasCheckConstraint("CK_stock_transactions_quantity", "quantity > 0");
            });

            entity.Property(t => t.Id).HasColumnName("id").UseIdentityByDefaultColumn();
            entity.Property(t => t.ProductId).HasColumnName("product_id").IsRequired();
            entity.Property(t => t.WarehouseId).HasColumnName("warehouse_id").IsRequired();
            entity.Property(t => t.ToWarehouseId).HasColumnName("to_warehouse_id");
            entity.Property(t => t.Type).HasColumnName("type").IsRequired();
            entity.Property(t => t.Quantity).HasColumnName("quantity").IsRequired();
            entity.Property(t => t.Date).HasColumnName("date").HasColumnType("timestamptz").HasDefaultValueSql("now()");
            entity.Property(t => t.Reference).HasColumnName("reference").HasMaxLength(100);
            entity.Property(t => t.Notes).HasColumnName("notes").HasMaxLength(500);
            entity.Property(t => t.UserId).HasColumnName("user_id");

            entity.HasIndex(t => t.ProductId).HasDatabaseName("idx_stock_txn_product");
            entity.HasIndex(t => t.WarehouseId).HasDatabaseName("idx_stock_txn_warehouse");
            entity.HasIndex(t => t.Date).HasDatabaseName("idx_stock_txn_date");

            entity.HasOne(t => t.Product)
                .WithMany(p => p.StockTransactions)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Warehouse)
                .WithMany(w => w.StockTransactions)
                .HasForeignKey(t => t.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.ToWarehouse)
                .WithMany(w => w.IncomingTransfers)
                .HasForeignKey(t => t.ToWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.User)
                .WithMany(u => u.StockTransactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ---------- Purchase Orders ----------
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.ToTable("purchase_orders");
            entity.Property(po => po.Id).HasColumnName("id").UseIdentityByDefaultColumn();
            entity.Property(po => po.SupplierId).HasColumnName("supplier_id").IsRequired();
            entity.Property(po => po.WarehouseId).HasColumnName("warehouse_id").IsRequired();
            entity.Property(po => po.OrderDate).HasColumnName("order_date").HasColumnType("timestamptz").HasDefaultValueSql("now()");
            entity.Property(po => po.ReceivedDate).HasColumnName("received_date").HasColumnType("timestamptz");
            entity.Property(po => po.Status).HasColumnName("status").HasDefaultValue(OrderStatus.Pending).IsRequired();

            entity.HasIndex(po => po.SupplierId).HasDatabaseName("idx_po_supplier");
            entity.HasIndex(po => po.Status).HasDatabaseName("idx_po_status");

            entity.HasOne(po => po.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(po => po.Warehouse)
                .WithMany(w => w.PurchaseOrders)
                .HasForeignKey(po => po.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- Purchase Order Items ----------
        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.ToTable("purchase_order_items", t =>
            {
                t.HasCheckConstraint("CK_purchase_order_items_quantity", "quantity > 0");
                t.HasCheckConstraint("CK_purchase_order_items_unit_cost", "unit_cost >= 0");
            });

            entity.Property(poi => poi.Id).HasColumnName("id").UseIdentityByDefaultColumn();
            entity.Property(poi => poi.PurchaseOrderId).HasColumnName("purchase_order_id").IsRequired();
            entity.Property(poi => poi.ProductId).HasColumnName("product_id").IsRequired();
            entity.Property(poi => poi.Quantity).HasColumnName("quantity").IsRequired();
            entity.Property(poi => poi.UnitCost).HasColumnName("unit_cost").HasPrecision(18, 2).HasDefaultValue(0.00m);

            entity.HasIndex(poi => poi.PurchaseOrderId).HasDatabaseName("idx_poi_purchase_order");

            entity.HasOne(poi => poi.PurchaseOrder)
                .WithMany(po => po.Items)
                .HasForeignKey(poi => poi.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(poi => poi.Product)
                .WithMany(p => p.PurchaseOrderItems)
                .HasForeignKey(poi => poi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
