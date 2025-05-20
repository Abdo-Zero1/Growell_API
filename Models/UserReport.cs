using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class UserReport
    {
        public int Id { get; set; } // المفتاح الأساسي
        public string UserID { get; set; }  
        public int TotalScore { get; set; }  
        public int MaxScore { get; set; }   
        public double Percentage { get; set; }  
        public string Classification { get; set; } 
        public List<TestResultDetails> TestDetails { get; set; } = new List<TestResultDetails>();  
    }
    public class TestResultDetails
    {
        public int Id { get; set; } // المفتاح الأساسي
        public int TestID { get; set; } 
        public int Score { get; set; } 
        public DateTime TakenAt { get; set; } 
    }
}
