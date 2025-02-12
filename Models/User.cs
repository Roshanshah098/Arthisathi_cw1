
using System.ComponentModel.DataAnnotations;

namespace arthiksathi.Models;

public class User
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Preferred Currency is required")]
    public string PreferredCurrency { get; set; } = "USD";
}