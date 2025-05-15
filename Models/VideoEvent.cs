using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class VideoEvent
    {
        public int VideoEventId { get; set; } 
        public string VideoTitle { get; set; } 
        public string? Topic { get; set; } 
        public string? VideoFilePath { get; set; }

    }
}
