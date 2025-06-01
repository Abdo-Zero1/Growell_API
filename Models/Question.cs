using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Models
{
    public class Question
    {
        public int QuestionID { get; set; }

        [Required(ErrorMessage = "Question text is required.")]
        [StringLength(500, ErrorMessage = "Question text can't exceed 500 characters.")]
        public string QuestionText { get; set; }

        [Required(ErrorMessage = "Answer option 1 is required.")]
        [StringLength(200, ErrorMessage = "Answer option can't exceed 200 characters.")]
        public string AnswerOption1 { get; set; }

        [Required(ErrorMessage = "Answer option 2 is required.")]
        [StringLength(200, ErrorMessage = "Answer option can't exceed 200 characters.")]
        public string AnswerOption2 { get; set; }

        [Required(ErrorMessage = "Answer option 3 is required.")]
        [StringLength(200, ErrorMessage = "Answer option can't exceed 200 characters.")]
        public string AnswerOption3 { get; set; }

        [Required(ErrorMessage = "Answer option 4 is required.")]
        [StringLength(200, ErrorMessage = "Answer option can't exceed 200 characters.")]
        public string AnswerOption4 { get; set; }

        [Required(ErrorMessage = "Correct answer is required.")]
        [StringLength(200, ErrorMessage = "Correct answer can't exceed 200 characters.")]
        public string CorrectAnswer { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Order number must be a positive value.")]
        public int OrderNumber { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

        [Required(ErrorMessage = "Test ID is required.")]
        public int TestID { get; set; }

        [JsonIgnore]
        public Test? Test { get; set; }

        [JsonIgnore]
        public int? DoctorID { get; set; }

        [JsonIgnore]
        public Doctor? Doctor { get; set; }
    }

}
