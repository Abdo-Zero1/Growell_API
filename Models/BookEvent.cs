using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BookEvent
    {
        public int BookEventId { get; set; } // المفتاح الأساسي
        public int TestResultID { get; set; } // إشارة إلى نتيجة الاختبار
        public DateTime EventDateTime { get; set; } = DateTime.Now; // تاريخ ووقت الحدث
        public string BookTitle { get; set; } // عنوان الكتاب
        public string? BookImagePath { get; set; } // مسار الصورة
        public string? BookFilePath { get; set; } // مسار الملف
        public int? DevelopmentStatusID { get; set; }

        // العلاقات
        public TestResult TestResult { get; set; } // العلاقة مع جدول نتائج الاختبارات
        public DevelopmentStatus? DevelopmentStatus { get; set; }


    }

}
