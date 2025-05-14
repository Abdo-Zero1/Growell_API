using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class VideoEvent
    {
        public int VideoEventId { get; set; } 
        public int TestResultID { get; set; } // إشارة إلى نتيجة الاختبار
        public DateTime EventDateTime { get; set; } = DateTime.Now; // تاريخ ووقت الحدث
        public string VideoTitle { get; set; } // عنوان الفيديو
        public string? Topic { get; set; } // موضوع الفيديو
        public string? VideoFilePath { get; set; }
        public int? DevelopmentStatusID { get; set; }

        public TestResult TestResult { get; set; } // العلاقة مع جدول نتائج الاختبارات
        public DevelopmentStatus? DevelopmentStatus { get; set; }// العلاقة مع جدول الاختبارات
    }
}
