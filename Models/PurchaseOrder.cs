using inventoryms.Models.Enums;

namespace inventoryms.Models;

public class PurchaseOrder
{
    public int Id { get; set; }

    public int SupplierId { get; set; }
    public virtual Supplier Supplier { get; set; } = null!;

    public int WarehouseId { get; set; }
    public virtual Warehouse Warehouse { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public DateTime? ReceivedDate { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public virtual ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}
