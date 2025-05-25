using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class TestResult
    {
        public int TestResultID { get; set; }
        public int TestID { get; set; }
        public int Score { get; set; }
        public DateTime TakenAt { get; set; } = DateTime.Now;
        public string UserID { get; set; }
        public int CategoryID { get; set; } 

        public int? DoctorID { get; set; }
        [JsonIgnore]
        public Doctor Doctor { get; set; }
        [JsonIgnore]
        public ApplicationUser applicationUser { get; set; }
        [JsonIgnore]
        public Test Test { get; set; }
    }
}
