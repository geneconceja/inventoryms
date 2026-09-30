namespace inventoryms.Models;

/// <summary>
/// Represents an organization (company / tenant) in the multi-tenant system.
/// All business data (warehouses, products, stock, orders) belongs to a tenant.
/// </summary>
public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug identifier (e.g., "acme", "globex").
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation: users linked to this tenant
    public virtual ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>();
}
