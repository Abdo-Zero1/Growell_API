using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class ProfileDTO
    {

        [StringLength(100, ErrorMessage = "UserName can't be longer than 100 characters.")]
        public string? UserName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [RegularExpression(@"^\+?(\d{7,15})$", ErrorMessage = "Phone number must be between 7 and 15 digits and can start with +")]
        public string? PhoneNumber { get; set; }
        public string? Adderss { get; set; }


        public IFormFile? ProfilePicture { get; set; }

    }
}
