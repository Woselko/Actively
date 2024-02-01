using System.ComponentModel.DataAnnotations;

namespace ActivelyApp.Models.AuthenticationDto.Login
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
