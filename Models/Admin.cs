using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Models
{
    public class Admin
    {
        public int AdminID { get; set; }
        public int UserID { get; set; }
        public string AdminRole { get; set; }
        public DateTime AssignedAt { get; set; }
        public string Permissions { get; set; }

        // العلاقات مع الجداول الأخرى
        public ICollection<Test> Tests { get; set; }
        public ICollection<Doctor> Doctors { get; set; }
        public User User { get; set; }
    }
}
