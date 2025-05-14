using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Session
    {
        public int SessionId { get; set; }
        public string SessionType { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
