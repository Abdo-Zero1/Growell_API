using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<Doctor> Doctors { get; set; }
        public ICollection<Parent> Parents { get; set; }
        public ICollection<Admin> Admins { get; set; }
    }
}
