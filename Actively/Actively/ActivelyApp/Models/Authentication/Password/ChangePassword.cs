using System.ComponentModel.DataAnnotations;

namespace ActivelyApp.Models.Authentication.Password
{
    public class ChangePassword
    {
        [Required]
        public string? OldPassword;

        [Required(ErrorMessage = "Password is required")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "NewPassword and confirmation password do not match")]
        [Required(ErrorMessage = "ConfirmPassword is required")]
        public string? ConfirmPassword { get; set; } = null!;
    }
}
