using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Models
{
    public class Category
    {

        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // العلاقة مع جدول الاختبارات
        public ICollection<Test> Tests { get; set; }

    }
}
