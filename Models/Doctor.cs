namespace Models
{
    public class Doctor
    {
        public int DoctorID { get; set; } // المفتاح الأساسي
        public string FirstName { get; set; }
        public string SecondName { get; set; } = null;
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

        // علاقة مع الاختبارات التي يشرف عليها الطبيب
        public ICollection<Test> Tests { get; set; } // الاختبارات التي يشرف عليها الطبيب

        // علاقة مع الأطفال الذين يعالجهم الطبيب
        public ICollection<Child> Children { get; set; } // الأطفال الذين يعالجهم الطبيب


    }
}
