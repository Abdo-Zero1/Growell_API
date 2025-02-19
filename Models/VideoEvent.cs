using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class VideoEvent
    {
        public int VideoEventId { get; set; } // المفتاح الأساسي
        public int ChildId { get; set; } // إشارة إلى الطفل
        public int TestId { get; set; } // إشارة إلى الاختبار
        public DateTime EventDateTime { get; set; } = DateTime.Now; // تاريخ ووقت الحدث
        public string VideoTitle { get; set; } // عنوان الفيديو
        public string? Topic { get; set; } // موضوع الفيديو

        public string? VideoFilePath { get; set; }
        public int? DevelopmentStatusID { get; set; }
        // العلاقات
        public DevelopmentStatus? DevelopmentStatus { get; set; }
        public Child Child { get; set; } // العلاقة مع جدول الأطفال
        public Test Test { get; set; } // العلاقة مع جدول الاختبارات
    }
}
