﻿using ActivelyDomain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ActivelyApp.Models.AuthenticationDto.Registration
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

        [Required(ErrorMessage = "FirstName is required")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }

        public string? Address { get; set; }
    }
}
