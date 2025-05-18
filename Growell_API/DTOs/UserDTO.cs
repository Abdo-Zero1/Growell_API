using System.ComponentModel.DataAnnotations;

namespace Growell_API.DTOs
{
    public class UserDTO
    {
        
        
            public string FirstName { get; set; }

            public string LastName { get; set; }

           
            public string Email { get; set; }

            public string Address { get; set; }

            public string DoctorName { get; set; }

            public string TestName { get; set; }

            public IFormFile ProfilePicture { get; set; }
        

    }
}
