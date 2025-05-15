using Models;
namespace Growell_API.DTOs
{
    public class DoctorDTO
    {
        public int DoctorID { get; set; } 
        public string FirstName    { get; set; }
        public string SecondName  { get; set; } 
        public string LastName  { get; set; }
        public string Email        { get; set; }
        public string Gender    { get; set; }
        public string Description   { get; set; }
        public string Bio    { get; set; }
        public string  AboutMe   { get; set; }
        public int     AveRating  { get; set; }
        public DateTime CreatedAt    { get; set; }
        public string  PhoneNumber    { get; set; }
        public string  Specialization { get; set; } 
        public int     YearsOfExperience  { get; set; } 
        public string  Education  { get; set; } 
        public int     Age  { get; set; }
        public string  ImgUrl { get; set; }
        
        //public ICollection<AppointmentDTO> appointmentDTOs { get; set; }
  

    }
}
