using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOS
{
    public class SimpleDoctorsDTO
    {
   
        public int DoctorID { get; set; } 
       
        public string Name { get; set; }
        public string Description { get; set; }
        public string Bio { get; set; }
        public int AveRating { get; set; }
        public string Specialization { get; set; }
        public string ImgUrl { get; set; }
    }
}
