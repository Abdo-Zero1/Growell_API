namespace Growell_API.DTOs
{
    public class DoctorEditDTO
    {

        public string? FirstName { get; set; } 
        public string? SecondName { get; set; } 
        public string? LatestName { get; set; } 
        public string? Description { get; set; } 
        public string? AboutMe { get; set; } 
        public string? Bio { get; set; } 
        public int? Age { get; set; }
        public string? Specialization { get; set; } 
        public string? Education { get; set; } 
        public int? YearsOfExperience { get; set; }
        public string? AboutOfKids { get; set; }
        public string? TargetAgeGroup { get; set; } 
        public string? PhoneNumber { get; set; } 
        public IFormFile? ImgUrl { get; set; }
    }
}
