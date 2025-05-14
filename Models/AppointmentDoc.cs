using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AppointmentDoc
    {
        public int Id { get; set; }
        public int DoctorID { get; set; }
        public string Day { get; set; }
        public TimeSpan StartWith { get; set; }
        public TimeSpan EndWith { get; set; }

        public Doctor Doctor { get; set; }
    }
}
