using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Test
    {
        public int TestID { get; set; }
        public string TestName { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public int NumberOfQuestions { get; set; }
        public bool IsActive { get; set; }
        public int AdminID { get; set; }

        // إضافة علاقة مع الطبيب الذي يشرف على الاختبار
        public int DoctorID { get; set; } // المفتاح الخارجي للطبيب
        public Doctor Doctor { get; set; } // العلاقة مع الطبيب

        // العلاقات الأخرى
        public Category Category { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<TestResult> TestResults { get; set; }
        public ICollection<BookEvent> BookEvents { get; set; }

    }
}
