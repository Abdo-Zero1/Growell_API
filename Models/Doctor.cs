using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models
{
    public class Doctor
    {
        public int DoctorID { get; set; } 
        public string FirstName { get; set; }
        public string SecondName { get; set; } = null;
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; } 
        public string Gender { get; set; }
        public string Bio { get; set; } = null;

        public string AboutMe { get; set; } = null;
        public string? AboutOfKids { get; set; }
        public string TargetAgeGroup { get; set; } 
        public string Description { get; set; } 
        public int AveRating { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; } 
        public int YearsOfExperience { get; set; } 
        public string Education { get; set; } 
        public int Age { get; set; }
        public string? ImgUrl { get; set; }
        public string passwordHash { get; set; }

        public string? UserID { get; set; }

        public ICollection<Test> Tests { get; set; }
        public ICollection<TestResult> TestResults { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<Category> Categories { get; set; } 

    }
}
