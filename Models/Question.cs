using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string QuestionText { get; set; }

        [Required(ErrorMessage = "Answer option 1 is required.")]
        public string AnswerOption1 { get; set; }

        [Required(ErrorMessage = "Answer option 2 is required.")]
        public string AnswerOption2 { get; set; }

        [Required(ErrorMessage = "Answer option 3 is required.")]
        public string AnswerOption3 { get; set; }

        [Required(ErrorMessage = "Answer option 4 is required.")]
        public string AnswerOption4 { get; set; }

        [Required(ErrorMessage = "Correct answer is required.")]
        public string CorrectAnswer { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Order number must be a positive value.")]
        public int OrderNumber { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Test ID is required.")]
        public int TestID { get; set; }
        [JsonIgnore]
        public Test? Test { get; set; }
    }
}
