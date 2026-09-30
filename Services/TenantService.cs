using inventoryms.Data;
using Microsoft.EntityFrameworkCore;

namespace inventoryms.Services;

/// <summary>
/// Resolves the active tenant from the current user's authentication claims.
/// 
/// IMPORTANT: This service does NOT inject ApplicationDbContext directly to avoid
/// circular dependency (DbContext → ITenantService → DbContext). Instead, it reads
/// tenant info purely from HttpContext claims for CurrentTenantId/CurrentTenantName.
/// For GetUserTenantsAsync(), it creates a fresh DbContext via IServiceProvider to
/// break the cycle.
/// </summary>
public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;

    public TenantService(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Reads the "tenant_id" claim from the current user's identity.
    /// </summary>
    public Guid? CurrentTenantId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id");
            if (claim != null && Guid.TryParse(claim.Value, out var tenantId))
                return tenantId;
            return null;
        }
    }

    /// <summary>
    /// Reads the "tenant_name" claim from the current user's identity.
    /// </summary>
    public string? CurrentTenantName
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_name")?.Value;
        }
    }

    /// <summary>
    /// Fetches all organizations the current user belongs to, for the sidebar switcher.
    /// Uses a separate scope to avoid circular dependency with ApplicationDbContext.
    /// </summary>
    public async Task<IReadOnlyList<TenantDto>> GetUserTenantsAsync()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Array.Empty<TenantDto>();

        // Create a new scope to get a fresh DbContext without circular dependency
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await db.UserTenants
            .IgnoreQueryFilters()
            .Where(ut => ut.UserId == userId.Value && ut.Tenant.IsActive)
            .OrderBy(ut => ut.Tenant.Name)
            .Select(ut => new TenantDto
            {
                Id = ut.TenantId,
                Name = ut.Tenant.Name,
                Slug = ut.Tenant.Slug
            })
            .ToListAsync();
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            return userId;
        return null;
    }
}
