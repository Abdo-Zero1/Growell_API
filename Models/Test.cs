using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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

        public int DoctorID { get; set; }
        [JsonIgnore]
        public Doctor Doctor { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }

        public ICollection<Question> Questions { get; set; }
        public ICollection<TestResult> TestResults { get; set; }

    }
}
