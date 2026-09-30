using Microsoft.AspNetCore.Identity;

namespace inventoryms.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

    // Multi-tenancy: organizations this user belongs to
    public virtual ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>();
}
