using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivelyDomain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Gender Gender { get; set; }

        public string? Address { get; set; }
        public string? RefreshToken { get; set; }
        public string? UserAvatar { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}
