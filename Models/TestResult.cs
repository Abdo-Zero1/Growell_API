using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TestResult
    {
        public int TestResultID { get; set; }
        public int TestID { get; set; }
        public int ChildID { get; set; }
        public int Score { get; set; }
        public DateTime TakenAt { get; set; }

        // العلاقة مع الجداول الأخرى
        public Test Test { get; set; }
        public Child Child { get; set; }
    }
}
