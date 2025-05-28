namespace Growell_API.DTOs
{
    public class UpdateProfileDto
    {
        public string? FirstName { get; set; }  
        public string? SecondName { get; set; } 
        public string? LastName { get; set; }  
        public string? Specialization { get; set; }  
        public string? PhoneNumber { get; set; }  
        public string? Email { get; set; }  
        public string? Address { get; set; }  
        public string? Bio { get; set; }  
        public IFormFile ImgUrl { get; set; }  
        public int? Age { get; set; }
        public string? Education { get; set; }  
    }
}
