using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Models
{
    public class Question
    {
        public int QuestionID { get; set; }
        public int TestID { get; set; }
        public string QuestionText { get; set; }
        public string AnswerOption1 { get; set; }
        public string AnswerOption2 { get; set; }
        public string AnswerOption3 { get; set; }
        public string AnswerOption4 { get; set; }
        public string CorrectAnswer { get; set; }
        public int OrderNumber { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // العلاقة مع جدول الاختبارات
        public Test Test { get; set; }
    }
}
