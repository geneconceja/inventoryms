namespace inventoryms.Models;

public class Category : ITenantScoped
{
    public int Id { get; set; }

    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;


    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
