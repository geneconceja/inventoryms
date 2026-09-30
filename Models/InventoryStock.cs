namespace inventoryms.Models;

public class InventoryStock : ITenantScoped
{
    public int Id { get; set; }

    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;


    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    public int WarehouseId { get; set; }
    public virtual Warehouse Warehouse { get; set; } = null!;

    public int Quantity { get; set; } = 0;
}
