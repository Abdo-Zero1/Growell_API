using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class ApplicationUserDTO
    {
        public int Id { get; set; }
        [Required]
        public string FristName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
