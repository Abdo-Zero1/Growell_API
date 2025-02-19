using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class GameEvent
    {
        public int GameEventId { get; set; } // المفتاح الأساسي
        public int ChildId { get; set; } // إشارة إلى الطفل
        public int TestId { get; set; } // إشارة إلى الاختبار
        public DateTime EventDateTime { get; set; } = DateTime.Now; // تاريخ ووقت الحدث
        public string GameName { get; set; } // اسم اللعبة
        public string? Level { get; set; } // المستوى داخل اللعبة (اختياري)
        public int? Score { get; set; } // النتيجة في اللعبة (اختياري)

        // مسار الصورة الخاصة باللعبة
        public string? GameImagePath { get; set; }

        // مسار ملف اللعبة (اختياري)
        public string? GameFilePath { get; set; }
        public int? DevelopmentStatusID { get; set; }

        // العلاقات
        public DevelopmentStatus? DevelopmentStatus { get; set; }
        public Child Child { get; set; } // العلاقة مع جدول الأطفال
        public Test Test { get; set; } // العلاقة مع جدول الاختبارات
    }
}
