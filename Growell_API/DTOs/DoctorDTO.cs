using Models;

namespace Growell_API.DTOs
{
    public class DoctorDTO
    {
        public string FirstName { get; set; } = null!;
        public string? SecondName { get; set; }
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string? Bio { get; set; }
        public string? AboutMe { get; set; }
        public string Description { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Specialization { get; set; } = null!;
        public int YearsOfExperience { get; set; }
        public string Education { get; set; } = null!;
        public int Age { get; set; }
        public string Address { get; set; } 
        public IFormFile? ImgUrl { get; set; }
        public string? AboutOfKids { get; set; }
        public string? TargetAgeGroup { get; set; }
        public string? UserID { get; set; } = null!;




    }
}
