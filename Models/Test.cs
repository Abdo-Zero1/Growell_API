using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class Test
    {
        public int TestID { get; set; }

        [Required(ErrorMessage = "TestName is required.")]
        [StringLength(100, ErrorMessage = "TestName cannot exceed 100 characters.")]
        public string TestName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "CategoryID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "CategoryID must be a positive number.")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "NumberOfQuestions is required.")]
        [Range(1, 100, ErrorMessage = "NumberOfQuestions must be between 1 and 100.")]
        public int NumberOfQuestions { get; set; }

        public bool IsActive { get; set; }
        [JsonIgnore]
        public int DoctorID { get; set; }
        [JsonIgnore]
        public Doctor? Doctor { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }
        [JsonIgnore]
        public ICollection<Question>? Questions { get; set; }
        [JsonIgnore]
        public ICollection<TestResult>? TestResults { get; set; }

    }
}
