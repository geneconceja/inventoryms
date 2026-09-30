using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using inventoryms.Data;
using inventoryms.Models;
using inventoryms.Models.ViewModels;
using inventoryms.Services;

namespace inventoryms.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ITenantService _tenantService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        ApplicationDbContext db,
        ITenantService tenantService,
        ILogger<HomeController> logger)
    {
        _db = db;
        _tenantService = tenantService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Dashboard";
        ViewData["PageTitle"] = "Dashboard";

        var vm = new DashboardViewModel
        {
            OrganizationName = _tenantService.CurrentTenantName ?? "Default Organization",
            TenantId = _tenantService.CurrentTenantId,
            Warehouses = await _db.Warehouses.OrderBy(w => w.Name).ToListAsync(),
            Categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync(),
            Suppliers = await _db.Suppliers.OrderBy(s => s.Name).ToListAsync()
        };

        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
