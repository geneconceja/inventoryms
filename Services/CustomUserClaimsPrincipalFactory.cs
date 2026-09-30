using System.Security.Claims;
using inventoryms.Data;
using inventoryms.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace inventoryms.Services;

/// <summary>
/// Extends the default Identity claims factory to inject the user's
/// default tenant_id and tenant_name claims into the authentication cookie.
/// This runs automatically whenever the user signs in or their cookie is refreshed.
/// </summary>
public class CustomUserClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    private readonly ApplicationDbContext _db;

    public CustomUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor,
        ApplicationDbContext db)
        : base(userManager, roleManager, optionsAccessor)
    {
        _db = db;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Find the user's default tenant (or fall back to the first one)
        var defaultTenant = await _db.UserTenants
            .Include(ut => ut.Tenant)
            .Where(ut => ut.UserId == user.Id && ut.Tenant.IsActive)
            .OrderByDescending(ut => ut.IsDefault)
            .ThenBy(ut => ut.JoinedAt)
            .FirstOrDefaultAsync();

        if (defaultTenant != null)
        {
            identity.AddClaim(new Claim("tenant_id", defaultTenant.TenantId.ToString()));
            identity.AddClaim(new Claim("tenant_name", defaultTenant.Tenant.Name));
        }

        return identity;
    }
}
