﻿using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class LoginDoctor
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; } = false; 
    }

}
