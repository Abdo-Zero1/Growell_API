using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class CreateQuestionDTO
    {
        [Required(ErrorMessage = "Question text is required")]
        [StringLength(500, ErrorMessage = "Question text cannot exceed 500 characters.")]
        public string QuestionText { get; set; }

        [Required(ErrorMessage = "Answer option 1 is required.")]
        [StringLength(200, ErrorMessage = "Answer option 1 cannot exceed 200 characters.")]
        public string AnswerOption1 { get; set; }

        [Required(ErrorMessage = "Answer option 2 is required.")]
        [StringLength(200, ErrorMessage = "Answer option 2 cannot exceed 200 characters.")]
        public string AnswerOption2 { get; set; }

        [Required(ErrorMessage = "Answer option 3 is required.")]
        [StringLength(200, ErrorMessage = "Answer option 3 cannot exceed 200 characters.")]
        public string AnswerOption3 { get; set; }

        [Required(ErrorMessage = "Answer option 4 is required.")]
        [StringLength(200, ErrorMessage = "Answer option 4 cannot exceed 200 characters.")]
        public string AnswerOption4 { get; set; }

        [Required(ErrorMessage = "Correct answer is required.")]
        [StringLength(200, ErrorMessage = "Correct answer cannot exceed 200 characters.")]
        public string CorrectAnswer { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Order number must be a positive value.")]
        public int OrderNumber { get; set; }

        [Required(ErrorMessage = "Test ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Test ID must be a positive value.")]
        public int TestID { get; set; }
    }
}
