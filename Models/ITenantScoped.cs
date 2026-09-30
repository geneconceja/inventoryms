namespace inventoryms.Models;

/// <summary>
/// Marker interface for entities that are scoped to a specific tenant (organization).
/// EF Core Global Query Filters will automatically filter by TenantId.
/// </summary>
public interface ITenantScoped
{
    Guid TenantId { get; set; }
}
