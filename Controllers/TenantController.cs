using System.Security.Claims;
using inventoryms.Data;
using inventoryms.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace inventoryms.Controllers;

/// <summary>
/// Handles switching the active organization for the logged-in user.
/// Updates the tenant_id and tenant_name claims in the authentication cookie.
/// </summary>
public class TenantController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<TenantController> _logger;

    public TenantController(
        ApplicationDbContext db,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<TenantController> logger)
    {
        _db = db;
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// POST /Tenant/Switch — switch the user's active organization.
    /// Re-issues the auth cookie with updated tenant claims and redirects back.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Switch(Guid tenantId, string? returnUrl = null)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login", "Account");

        // Verify the user actually belongs to the requested tenant
        var membership = await _db.UserTenants
            .Include(ut => ut.Tenant)
            .FirstOrDefaultAsync(ut => ut.UserId == user.Id && ut.TenantId == tenantId && ut.Tenant.IsActive);

        if (membership == null)
        {
            _logger.LogWarning("User '{Email}' attempted to switch to unauthorized tenant '{TenantId}'.",
                user.Email, tenantId);
            return Forbid();
        }

        // Re-sign-in the user to regenerate claims with the new tenant
        // First, remove old tenant claims
        var existingClaims = await _userManager.GetClaimsAsync(user);
        var oldTenantIdClaim = existingClaims.FirstOrDefault(c => c.Type == "tenant_id");
        var oldTenantNameClaim = existingClaims.FirstOrDefault(c => c.Type == "tenant_name");

        if (oldTenantIdClaim != null)
            await _userManager.RemoveClaimAsync(user, oldTenantIdClaim);
        if (oldTenantNameClaim != null)
            await _userManager.RemoveClaimAsync(user, oldTenantNameClaim);

        // Add new tenant claims
        await _userManager.AddClaimAsync(user, new Claim("tenant_id", membership.TenantId.ToString()));
        await _userManager.AddClaimAsync(user, new Claim("tenant_name", membership.Tenant.Name));

        // Refresh the sign-in to regenerate the cookie with updated claims
        await _signInManager.RefreshSignInAsync(user);

        _logger.LogInformation("User '{Email}' switched to organization '{TenantName}' ({TenantId}).",
            user.Email, membership.Tenant.Name, membership.TenantId);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }
}
