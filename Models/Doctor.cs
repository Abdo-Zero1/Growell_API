namespace Models
{
    public class Doctor
    {
        public int DoctorID { get; set; } 
        public string FirstName { get; set; }
        public string SecondName { get; set; } = null;
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; } 
        public int YearsOfExperience { get; set; } 
        public string Education { get; set; } 
        public int Age { get; set; }
        public string ImgUrl { get; set; }

        public ICollection<Test> Tests { get; set; } 



    }
}
