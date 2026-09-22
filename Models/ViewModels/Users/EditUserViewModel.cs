using System.ComponentModel.DataAnnotations;

namespace inventoryms.Models.ViewModels.Users;

public class EditUserViewModel
{
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "Full Name")]
    [StringLength(150, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Role")]
    public string Role { get; set; } = "Staff";

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least {2} characters.")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password (leave blank to keep current)")]
    public string? NewPassword { get; set; }

    public List<string> AvailableRoles { get; set; } = new();
}
