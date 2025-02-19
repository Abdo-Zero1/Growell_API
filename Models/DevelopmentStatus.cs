using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DevelopmentStatus
    {
        public int DevelopmentStatusID { get; set; }
        public string NameStatus { get; set; }
        public string Description { get; set; }

        // العلاقات
        public ICollection<Child> Children { get; set; }
        public ICollection<BookEvent> BookEvents { get; set; }

    }
}
