using System.ComponentModel.DataAnnotations;

namespace inventoryms.Models.ViewModels.Users;

public class CreateUserViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    [StringLength(150, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least {2} characters.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Role")]
    public string Role { get; set; } = "Staff";

    public List<string> AvailableRoles { get; set; } = new();
}
