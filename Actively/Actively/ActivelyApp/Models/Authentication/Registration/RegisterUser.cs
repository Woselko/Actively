using System.ComponentModel.DataAnnotations;

namespace ActivelyApp.Models.Authentication.Registration
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        [Required(ErrorMessage = "ConfirmPassword is required")]
        public string? ConfirmPassword { get; set; } = null!;
    }
}
