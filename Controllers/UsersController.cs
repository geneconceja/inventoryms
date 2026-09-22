using inventoryms.Models;
using inventoryms.Models.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace inventoryms.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    // GET: /Users
    public async Task<IActionResult> Index(string? search)
    {
        ViewData["Title"] = "User Management";
        ViewData["PageTitle"] = "User Management";

        var query = _userManager.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.Email!.Contains(search) || u.FullName.Contains(search));

        var users = await query.OrderBy(u => u.FullName).ToListAsync();

        var list = new List<UserListItemViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            list.Add(new UserListItemViewModel
            {
                Id        = user.Id,
                FullName  = user.FullName,
                Email     = user.Email ?? "",
                Role      = roles.FirstOrDefault() ?? "—",
                IsActive  = user.IsActive,
                CreatedAt = user.CreatedAt
            });
        }

        ViewBag.Search = search;
        return View(list);
    }

    // GET: /Users/Create
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Create User";
        ViewData["PageTitle"] = "Create User";
        return View(new CreateUserViewModel
        {
            AvailableRoles = await GetRoleNamesAsync()
        });
    }

    // POST: /Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        model.AvailableRoles = await GetRoleNamesAsync();
        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser
        {
            UserName    = model.Email,
            Email       = model.Email,
            FullName    = model.FullName,
            IsActive    = true,
            EmailConfirmed = true,
            CreatedAt   = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description);
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, model.Role);
        _logger.LogInformation("Admin created user '{Email}' with role '{Role}'.", model.Email, model.Role);
        TempData["Success"] = $"User {model.Email} created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Users/Edit/{id}
    public async Task<IActionResult> Edit(Guid id)
    {
        ViewData["Title"] = "Edit User";
        ViewData["PageTitle"] = "Edit User";

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        return View(new EditUserViewModel
        {
            Id             = user.Id,
            FullName       = user.FullName,
            Email          = user.Email ?? "",
            Role           = roles.FirstOrDefault() ?? "Staff",
            IsActive       = user.IsActive,
            AvailableRoles = await GetRoleNamesAsync()
        });
    }

    // POST: /Users/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditUserViewModel model)
    {
        model.AvailableRoles = await GetRoleNamesAsync();
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        // Prevent deactivating or demoting the last admin
        var currentUserId = _userManager.GetUserId(User);
        if (user.Id.ToString() == currentUserId && (!model.IsActive || model.Role != "Admin"))
        {
            ModelState.AddModelError(string.Empty, "You cannot deactivate your own account or remove your own Admin role.");
            return View(model);
        }

        user.FullName = model.FullName;
        user.IsActive = model.IsActive;
        await _userManager.UpdateAsync(user);

        // Update role
        var existingRoles = await _userManager.GetRolesAsync(user);
        if (!existingRoles.Contains(model.Role))
        {
            await _userManager.RemoveFromRolesAsync(user, existingRoles);
            await _userManager.AddToRoleAsync(user, model.Role);
        }

        // Optionally reset password
        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var pwResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!pwResult.Succeeded)
            {
                foreach (var e in pwResult.Errors) ModelState.AddModelError(string.Empty, e.Description);
                return View(model);
            }
        }

        TempData["Success"] = $"User {user.Email} updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Users/ToggleStatus/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        var currentUserId = _userManager.GetUserId(User);
        if (user.Id.ToString() == currentUserId)
        {
            TempData["Error"] = "You cannot deactivate your own account.";
            return RedirectToAction(nameof(Index));
        }

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);
        TempData["Success"] = $"User {user.Email} {(user.IsActive ? "activated" : "deactivated")}.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<string>> GetRoleNamesAsync()
        => await _roleManager.Roles.Select(r => r.Name!).OrderBy(n => n).ToListAsync();
}
