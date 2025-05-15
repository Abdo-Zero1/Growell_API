using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class ProfileDTO
    {
        [StringLength(50)]

        public string UserName { get; set; }
       
        public IFormFile ProfilePicture { get; set; }
    }
}
