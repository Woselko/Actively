using System.ComponentModel.DataAnnotations;

namespace ActivelyApp.Models.Authentication.Password
{
    public class PasswordReset
    {
        [Required]
        public string Password { get; set; } = null!;
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
