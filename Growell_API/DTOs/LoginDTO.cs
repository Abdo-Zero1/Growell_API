using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class LoginDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email address format")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 50 characters")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
