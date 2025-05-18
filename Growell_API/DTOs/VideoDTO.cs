using Models;
using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class VideoDTO
    {
        [Required]
        public string VideoTitle { get; set; }

        public string? Description { get; set; }

        [Required]
        [Url]
        public string VideoUrl { get; set; }

        [Required]
        public IFormFile VideoImage { get; set; }
    }
}
