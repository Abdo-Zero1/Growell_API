using Models;

namespace Growell_API.DTOs
{
    public class VideoDTO
    {
        public VideoEvent VideoEvent { get; set; }
        public IFormFile VideoFile { get; set; }
    }
}
