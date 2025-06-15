using System.ComponentModel.DataAnnotations;
namespace Growell_API.DTOs
{
    public class UpdateBookDTO
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Book title must be between 3 and 100 characters.")]
        public string BookTitle { get; set; } = string.Empty;
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "About the book cannot exceed 500 characters.")]
        public string AboutOfBook { get; set; } = string.Empty;
        [Url(ErrorMessage = "The book URL must be valid.")]
        public string BookUrl { get; set; } = string.Empty;
        public IFormFile? BookImage { get; set; }
    }
}
