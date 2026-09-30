using inventoryms.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace inventoryms.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        // 1. Seed Roles: Admin, Manager, Staff
        string[] roleNames = ["Admin", "Manager", "Staff"];
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new ApplicationRole(roleName));
                if (roleResult.Succeeded)
                {
                    logger.LogInformation("Role '{RoleName}' created successfully.", roleName);
                }
                else
                {
                    logger.LogError("Failed to create role '{RoleName}': {Errors}", 
                        roleName, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
        }

        // 2. Seed Tenants (Organizations)
        var acmeId = Guid.Parse("a1111111-1111-1111-1111-111111111111");
        var globexId = Guid.Parse("b2222222-2222-2222-2222-222222222222");

        if (!await db.Tenants.AnyAsync())
        {
            db.Tenants.AddRange(
                new Tenant
                {
                    Id = acmeId,
                    Name = "Acme Logistics",
                    Slug = "acme",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Tenant
                {
                    Id = globexId,
                    Name = "Globex Distribution",
                    Slug = "globex",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
            await db.SaveChangesAsync();
            logger.LogInformation("Seeded 2 tenant organizations: Acme Logistics, Globex Distribution.");
        }
        else
        {
            // Ensure IDs match if tenants already exist
            var acme = await db.Tenants.FirstOrDefaultAsync(t => t.Slug == "acme");
            var globex = await db.Tenants.FirstOrDefaultAsync(t => t.Slug == "globex");
            if (acme != null) acmeId = acme.Id;
            if (globex != null) globexId = globex.Id;
        }

        // 3. Seed Default Admin User
        const string adminEmail = "admin@inventoryms.local";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createAdminResult = await userManager.CreateAsync(adminUser, "Admin123!");
            if (createAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Default Admin user '{AdminEmail}' created and assigned to 'Admin' role.", adminEmail);
            }
            else
            {
                logger.LogError("Failed to create default Admin user: {Errors}", 
                    string.Join(", ", createAdminResult.Errors.Select(e => e.Description)));
            }
        }

        // 4. Link Admin to Both Tenants (if not already linked)
        if (adminUser != null)
        {
            var existingLinks = await db.UserTenants
                .Where(ut => ut.UserId == adminUser.Id)
                .ToListAsync();

            if (!existingLinks.Any(ut => ut.TenantId == acmeId))
            {
                db.UserTenants.Add(new UserTenant
                {
                    UserId = adminUser.Id,
                    TenantId = acmeId,
                    IsDefault = true,
                    JoinedAt = DateTime.UtcNow
                });
                logger.LogInformation("Linked admin to Acme Logistics (default).");
            }

            if (!existingLinks.Any(ut => ut.TenantId == globexId))
            {
                db.UserTenants.Add(new UserTenant
                {
                    UserId = adminUser.Id,
                    TenantId = globexId,
                    IsDefault = false,
                    JoinedAt = DateTime.UtcNow
                });
                logger.LogInformation("Linked admin to Globex Distribution.");
            }

            await db.SaveChangesAsync();
        }

        // 5. Seed Sample Data for Each Tenant
        await SeedTenantDataAsync(db, acmeId, "Acme", logger);
        await SeedTenantDataAsync(db, globexId, "Globex", logger);
    }

    private static async Task SeedTenantDataAsync(
        ApplicationDbContext db, Guid tenantId, string prefix, ILogger logger)
    {
        // Check if this tenant already has warehouses (skip if already seeded)
        var hasWarehouses = await db.Warehouses
            .IgnoreQueryFilters()
            .AnyAsync(w => w.TenantId == tenantId);

        if (hasWarehouses)
            return;

        // Warehouses
        var warehouses = prefix == "Acme"
            ? new[]
            {
                new Warehouse { TenantId = tenantId, Name = "Main Depot", Location = "Manila, Philippines" },
                new Warehouse { TenantId = tenantId, Name = "South Warehouse", Location = "Cebu City, Philippines" }
            }
            : new[]
            {
                new Warehouse { TenantId = tenantId, Name = "North Hub", Location = "Quezon City, Philippines" },
                new Warehouse { TenantId = tenantId, Name = "East Storage", Location = "Davao City, Philippines" }
            };

        db.Warehouses.AddRange(warehouses);
        await db.SaveChangesAsync();

        // Categories
        var categories = prefix == "Acme"
            ? new[]
            {
                new Category { TenantId = tenantId, Name = "Electronics", Description = "Electronic components and devices" },
                new Category { TenantId = tenantId, Name = "Industrial Tools", Description = "Heavy-duty industrial equipment" }
            }
            : new[]
            {
                new Category { TenantId = tenantId, Name = "Retail Goods", Description = "Consumer retail products" },
                new Category { TenantId = tenantId, Name = "Packaging", Description = "Packaging materials and supplies" }
            };

        db.Categories.AddRange(categories);
        await db.SaveChangesAsync();

        // Suppliers
        var suppliers = prefix == "Acme"
            ? new[]
            {
                new Supplier { TenantId = tenantId, Name = "TechParts Co.", ContactName = "Juan Reyes", Email = "juan@techparts.ph", Phone = "+63-917-111-2222" },
                new Supplier { TenantId = tenantId, Name = "Steel & Iron Ltd.", ContactName = "Maria Santos", Email = "maria@steelandiron.ph", Phone = "+63-918-333-4444" }
            }
            : new[]
            {
                new Supplier { TenantId = tenantId, Name = "PackagePro Inc.", ContactName = "Carlos Tan", Email = "carlos@packagepro.ph", Phone = "+63-919-555-6666" },
                new Supplier { TenantId = tenantId, Name = "RetailSource PH", ContactName = "Ana Cruz", Email = "ana@retailsource.ph", Phone = "+63-920-777-8888" }
            };

        db.Suppliers.AddRange(suppliers);
        await db.SaveChangesAsync();

        logger.LogInformation("Seeded sample data for tenant '{Prefix}' ({TenantId}).", prefix, tenantId);
    }
}
