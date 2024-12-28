using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class ApplicationUserDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is Required")]
        [MinLength(3, ErrorMessage = "First Name must be at least 3 characters long")]
        [MaxLength(50, ErrorMessage = "First Name must not exceed 50 characters")]
        public string FristName { get; set; }

        [Required(ErrorMessage = "Last Name Is Required")]
        [MinLength(3, ErrorMessage = "Last Name must be at least 3 characters long")]
        [MaxLength(50, ErrorMessage = "Last Name Must Not Exceed 50 Characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address Format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address Is Required")]
        [StringLength(100, ErrorMessage = "Address Must Not Exceed 100 Characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [DataType(DataType.Password, ErrorMessage = "Invalid Password Format")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 50 Characters")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        public IFormFile ProfilePicturePath { get; set; }

    }
}
