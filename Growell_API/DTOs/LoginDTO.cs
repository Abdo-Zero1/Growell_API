using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class LoginDTO
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
