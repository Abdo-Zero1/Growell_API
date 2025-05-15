using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BookEvent
    {
        public int BookEventId { get; set; } 
        public DateTime EventDateTime { get; set; } = DateTime.Now; 
        public string BookTitle { get; set; } // عنوان الكتاب
        public string? BookImagePath { get; set; } // مسار الصورة
        public string? BookFilePath { get; set; } // مسار الملف

        // العلاقات
        public int TestResultID { get; set; } // إشارة إلى نتيجة الاختبار
        public TestResult TestResult { get; set; } // العلاقة مع جدول نتائج الاختبارات
        public int? DevelopmentStatusID { get; set; }
        public DevelopmentStatus? DevelopmentStatus { get; set; }


    }

}
