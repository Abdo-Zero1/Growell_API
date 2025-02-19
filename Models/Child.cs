using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Models
{
    public class Child
    {
        public int ChildID { get; set; }
        public int ParentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImgUrl { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string IQScore { get; set; }
        public DateTime LastTestDate { get; set; }
        public int DevelopmentStatusID { get; set; }
        public string Note { get; set; }

        // علاقة مع الطبيب المعالج
        public int DoctorID { get; set; } // المفتاح الخارجي للطبيب
        public Doctor Doctor { get; set; } // العلاقة مع الطبيب

        // العلاقات الأخرى
        public Parent Parent { get; set; }
        public DevelopmentStatus DevelopmentStatus { get; set; }
        public ICollection<Session> Sessions { get; set; }
        public ICollection<TestResult> TestResults { get; set; }
        public ICollection<BookEvent> BookEvents { get; set; }

    }
}
