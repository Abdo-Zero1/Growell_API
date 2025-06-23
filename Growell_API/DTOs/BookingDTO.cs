using System.Text.Json.Serialization;

namespace Growell_API.DTOs
{
    public class BookingDTO
    {
        public int? BookingID { get; set; }
        [JsonIgnore]
        public string UserID { get; set; } = string.Empty;
        [JsonIgnore]
        public string CreatedByUserName { get; set; } = string.Empty;
        
        [JsonIgnore]
        public string BookingDoctorName { get; set; } = string.Empty;

        public string TestName { get; set; } = string.Empty;    
        public int Score { get; set; } 
        public DateTime AppointmentDate { get; set; }
        public bool IsConfirmed { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
