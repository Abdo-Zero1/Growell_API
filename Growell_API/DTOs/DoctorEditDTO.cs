using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class DoctorEditDTO
{
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string? FirstName { get; set; }

    [MaxLength(50, ErrorMessage = "Second name cannot exceed 50 characters.")]
    public string? SecondName { get; set; }

    [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    public string? LatestName { get; set; }

    [MaxLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
    public string? Description { get; set; }

    [MaxLength(250, ErrorMessage = "About me cannot exceed 250 characters.")]
    public string? AboutMe { get; set; }

    [MaxLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
    public string? Bio { get; set; }

    [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
    public int? Age { get; set; }

    [MaxLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
    public string? Specialization { get; set; }

    [MaxLength(200, ErrorMessage = "Education cannot exceed 200 characters.")]
    public string? Education { get; set; }

    [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50.")]
    public int? YearsOfExperience { get; set; }

    [MaxLength(250, ErrorMessage = "About of kids cannot exceed 250 characters.")]
    public string? AboutOfKids { get; set; }

    [MaxLength(250, ErrorMessage = "Target age group cannot exceed 250 characters.")]
    public string? TargetAgeGroup { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string? PhoneNumber { get; set; }

    [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
    public string? Address { get; set; }

    public IFormFile? ImgUrl { get; set; }
}
