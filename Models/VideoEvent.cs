using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class VideoEvent
    {
        public int VideoEventId { get; set; }

        [Required]
        public string VideoTitle { get; set; }

        public string? Description { get; set; }

        [Required]
        [Url]
        public string VideoUrl { get; set; }

        public string? VideoImagePath { get; set; }

    }
}
