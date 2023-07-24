using System.ComponentModel.DataAnnotations;

namespace ActivelyApp.Models.Authentication.Password
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "Old password is required")]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "NewPassword and confirmation password do not match")]
        [Required(ErrorMessage = "ConfirmPassword is required")]
        public string? ConfirmPassword { get; set; } = null!;
    }
}
