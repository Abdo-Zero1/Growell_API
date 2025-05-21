using Models;
using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class BookDTO
    {
        [Required]
        public string BookTitle { get; set; }

        public string? Description { get; set; }
        [Required]
        [Url]
        public string BookUrl { get; set; }
        [Required]
        public IFormFile BookImage { get; set; }
    }
}
