namespace Growell_API.DTOs
{
    public class DoctorEditDTO
    {

        public string FirstName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public IFormFile? ImgUrl { get; set; }
    }
}
