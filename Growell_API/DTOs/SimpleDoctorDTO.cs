namespace Growell_API.DTOs
{
    /// <summary>
    /// Simple Doctor Info
    /// </summary>
    public class SimpleDoctorDTO
    {
        /// <Examp>1</Examp>
        public int DoctorID { get; set; } // المفتاح الأساسي
          /// <Examp>1</Examp>
        public string Name { get; set; }
        public string Description { get; set; }
        public int AveRating { get; set; }
        public string Specialization { get; set; }
        public string ImgUrl { get; set; }
    }
}
