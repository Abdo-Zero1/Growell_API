using Microsoft.AspNetCore.Http;
using System;

namespace Growell_API.DTOs
{
    public class RegisterDoctor
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; } 
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Bio { get; set; }
        public string AboutMe { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string AboutOfKids { get; set; }
        public string TargetAgeGroup { get; set; }
        public string Specialization { get; set; }
        public string? Address { get; set; } 
        public int YearsOfExperience { get; set; }
        public int AveRating { get; set; }
        public int Age { get; set; }
        public string? Education { get; set; }
        public IFormFile? ImgUrl { get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
