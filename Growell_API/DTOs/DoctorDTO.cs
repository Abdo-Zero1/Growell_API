using Models;
namespace Growell_API.DTOs
{
    public class DoctorDTO
    {
        public int DoctorID { get; set; } // المفتاح الأساسي
        public string FirstName { get; set; }
        public string SecondName { get; set; } 
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
        public int AveRating { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; } // التخصص
        public int YearsOfExperience { get; set; } // عدد سنوات الخبرة
        public string Education { get; set; } // المؤهلات التعليمية
        public int Age { get; set; }
        public string ImgUrl { get; set; }
        
        public DoctorDTO(Doctor Doc) {
            this.DoctorID = Doc.DoctorID;
            this.FirstName = Doc.FirstName;
            this.LastName = Doc.LastName;
            this.Email = Doc.Email;
                this.Gender = Doc.Gender;
            this.Description = Doc.Description;
          
            this.AveRating = Doc.AveRating;
            this.PhoneNumber = Doc.PhoneNumber;
            this.Specialization = Doc.Specialization;
            this.CreatedAt = Doc.CreatedAt;
            this.YearsOfExperience = Doc.YearsOfExperience;
            this.Education = Doc.Education;
            this.Age = Doc.Age;
                this.ImgUrl = Doc.ImgUrl;
        
        }

    }
}
