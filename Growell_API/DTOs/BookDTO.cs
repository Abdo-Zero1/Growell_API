using Models;

namespace Growell_API.DTOs
{
    public class BookDTO
    {
        public BookEvent BookEvent { get; set; }
        public IFormFile ImgUrl { get; set; }
    }
}
