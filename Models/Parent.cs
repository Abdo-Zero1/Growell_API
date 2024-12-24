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
        public int UserID { get; set; }
        //public string Phone { get; set; }
        public string Occupation { get; set; }
        public string ParentType { get; set; }

        // العلاقة مع جدول الأطفال
        public ICollection<Child> Children { get; set; }
        public User User { get; set; }

    }
}
