using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Growell_API.DTOs
{
    public class DoctorDTO
    {
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "Second name cannot exceed 50 characters.")]
        public string? SecondName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Gender is required.")]
        [MaxLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]
        public string Gender { get; set; } = null!;

        [MaxLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
        public string? Bio { get; set; }

        [MaxLength(250, ErrorMessage = "About Me cannot exceed 250 characters.")]
        public string? AboutMe { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Specialization is required.")]
        [MaxLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
        public string Specialization { get; set; } = null!;

        [Required(ErrorMessage = "Years of experience is required.")]
        [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50.")]
        public int YearsOfExperience { get; set; }

        [Required(ErrorMessage = "Education is required.")]
        [MaxLength(200, ErrorMessage = "Education cannot exceed 200 characters.")]
        public string Education { get; set; } = null!;

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
        public int Age { get; set; }

        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = null!;

        [FileExtensions(Extensions = "jpg,jpeg,png,gif", ErrorMessage = "Only .jpg, .jpeg, .png, or .gif files are allowed.")]
        public IFormFile? ImgUrl { get; set; }

        [MaxLength(250, ErrorMessage = "About Of Kids cannot exceed 250 characters.")]
        public string? AboutOfKids { get; set; }

        [MaxLength(250, ErrorMessage = "Target Age Group cannot exceed 250 characters.")]
        public string? TargetAgeGroup { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public string? UserID { get; set; } = null!;
    }
}
