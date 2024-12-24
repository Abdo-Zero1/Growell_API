namespace Models
{
    public class Doctor
    {
        public int DoctorID { get; set; } // المفتاح الأساسي
        public int UserID { get; set; } // علاقة مع جدول المستخدمين (Users)
        public string Specialization { get; set; } // التخصص
        public int YearsOfExperience { get; set; } // عدد سنوات الخبرة
                                                   // public string PhoneNumber { get; set; } // رقم الهاتف
        public string Education { get; set; } // المؤهلات التعليمية
        public int ApprovedBy { get; set; } // ID الخاص بالمشرف الذي وافق على الطبيب
        public int Age { get; set; }
        // العلاقات
        public User User { get; set; } // العلاقة مع جدول المستخدمين
        public Admin ApprovedByAdmin { get; set; } // العلاقة مع جدول المدراء


    }
}
