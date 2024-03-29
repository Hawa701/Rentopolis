﻿using System.ComponentModel.DataAnnotations;

namespace Rentopolis.Models.Entitiy
{
    public class RegisterationViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Register as")]
        public string Role { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePic { get; set; }
        public string? ProfilePicUrl { get; set; } = "";
    }
}
