using Models;
using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class BookDTO
    {
        [Required(ErrorMessage = "The book title is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Book title must be between 3 and 100 characters.")]
        public string BookTitle { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "About the book cannot exceed 500 characters.")]
        public string AboutOfBook { get; set; } = string.Empty;

        [Required(ErrorMessage = "The book URL is required.")]
        [Url(ErrorMessage = "The book URL must be valid.")]
        public string BookUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "The book image is required.")]
        public IFormFile BookImage { get; set; }
    }
}
