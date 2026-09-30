using inventoryms.Models;

namespace inventoryms.Models.ViewModels;

public class DashboardViewModel
{
    public string OrganizationName { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public List<Warehouse> Warehouses { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Supplier> Suppliers { get; set; } = new();
}
