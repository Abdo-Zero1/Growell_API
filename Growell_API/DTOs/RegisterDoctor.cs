using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class RegisterDoctor
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Second name cannot exceed 50 characters.")]
        public string SecondName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [RegularExpression("Male|Female|Other", ErrorMessage = "Gender must be Male, Female, or Other.")]
        public string Gender { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
        public string Bio { get; set; }

        [StringLength(500, ErrorMessage = "About Me section cannot exceed 500 characters.")]
        public string AboutMe { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [RegularExpression(@"^\+?(\d{7,15})$", ErrorMessage = "Phone number must be between 7 and 15 digits and can start with +.")]
        public string PhoneNumber { get; set; }

        [StringLength(500, ErrorMessage = "About Of Kids section cannot exceed 500 characters.")]
        public string AboutOfKids { get; set; }

        [StringLength(100, ErrorMessage = "Target age group cannot exceed 100 characters.")]
        public string TargetAgeGroup { get; set; }

        [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
        public string Specialization { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; }

        [Range(0, 100, ErrorMessage = "Years of experience must be between 0 and 100.")]
        public int YearsOfExperience { get; set; }

        [Range(0, 5, ErrorMessage = "Average rating must be between 0 and 5.")]
        public int AveRating { get; set; }

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
        public int Age { get; set; }

        [StringLength(300, ErrorMessage = "Education details cannot exceed 300 characters.")]
        public string? Education { get; set; }

        [FileExtensions(Extensions = "jpg,jpeg,png,gif", ErrorMessage = "Only .jpg, .jpeg, .png, or .gif files are allowed.")]
        public IFormFile? ImgUrl { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
