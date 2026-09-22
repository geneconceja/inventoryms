using inventoryms.Models.Enums;

namespace inventoryms.Models;

public class StockTransaction
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    public int WarehouseId { get; set; }
    public virtual Warehouse Warehouse { get; set; } = null!;

    public int? ToWarehouseId { get; set; }
    public virtual Warehouse? ToWarehouse { get; set; }

    public TransactionType Type { get; set; }

    public int Quantity { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public string? Reference { get; set; }

    public string? Notes { get; set; }

    public Guid? UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }
}
