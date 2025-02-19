using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Parent
    {
        public int ParentID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; } = null;
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PhoneNumber { get; set; }
        public string Occupation { get; set; }


        // العلاقة مع جدول الأطفال
        public ICollection<Child> Children { get; set; }

    }
}
