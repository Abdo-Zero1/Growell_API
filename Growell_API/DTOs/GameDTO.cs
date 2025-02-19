using Models;

namespace Growell_API.DTOs
{
    public class GameDTO
    {
        public GameEvent GameEvent { get; set; }
        public IFormFile? GameImage { get; set; } 
        public IFormFile? GameFile { get; set; }
    }
}
