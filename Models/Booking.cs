using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class Booking
    {
        public int BookingID { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorID { get; set; }
        [JsonIgnore]
        public Doctor? Doctor { get; set; }

        [ForeignKey("User")]
        public string? UserID { get; set; }
        [JsonIgnore]
        public ApplicationUser? User { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string Notes { get; set; }
        public bool IsConfirmed { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        public string TastName { get; set; }
        public int Score { get; set; } 

        public string CreatedByUserName { get; set; } = string.Empty; 
        public string BookingDoctorName { get; set; } = string.Empty; 
    }
}
