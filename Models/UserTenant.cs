namespace inventoryms.Models;

/// <summary>
/// Join entity linking ApplicationUser to Tenant (many-to-many).
/// A user can belong to multiple organizations.
/// </summary>
public class UserTenant
{
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// If true, this is the user's default organization on login.
    /// </summary>
    public bool IsDefault { get; set; } = false;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
