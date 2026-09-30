namespace inventoryms.Services;

/// <summary>
/// Lightweight DTO for passing tenant info to the UI (sidebar switcher).
/// </summary>
public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

/// <summary>
/// Provides access to the current tenant context for the request.
/// Reads the active tenant_id from the authenticated user's claims.
/// </summary>
public interface ITenantService
{
    /// <summary>
    /// The active tenant ID for the current request (null if unauthenticated or no tenant set).
    /// </summary>
    Guid? CurrentTenantId { get; }

    /// <summary>
    /// The display name of the active tenant.
    /// </summary>
    string? CurrentTenantName { get; }

    /// <summary>
    /// Returns all organizations the current user has access to.
    /// </summary>
    Task<IReadOnlyList<TenantDto>> GetUserTenantsAsync();
}
