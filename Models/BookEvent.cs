using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BookEvent
    {
        public int BookEventId { get; set; } 
        public string BookTitle { get; set; }
        public string? Description { get; set; }

        [Required]
        [Url]
        public string BookUrl { get; set; }

        public string? BookImagePath { get; set; }

       


    }

}
