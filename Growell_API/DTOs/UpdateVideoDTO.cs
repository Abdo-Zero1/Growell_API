using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class UpdateVideoDTO
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Video title must be between 3 and 100 characters.")]
        public string VideoTitle { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "About the video cannot exceed 500 characters.")]
        public string AboutOfVideo { get; set; } = string.Empty;

        [Url(ErrorMessage = "The video URL must be valid.")]
        public string VideoUrl { get; set; } = string.Empty;

        public IFormFile? VideoImage { get; set; }
    }
}
