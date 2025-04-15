using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = $"{SD.AdminRole}")]

    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository doctorRepository;

        public DoctorController(IDoctorRepository doctorRepository)
        {
            this.doctorRepository = doctorRepository;
        }


        [HttpGet]
        public ActionResult<IEnumerable<SimpleDoctorDTO>> Index() {
            var Doc = doctorRepository.Get().ToList();
            var Doctors = new List<SimpleDoctorDTO>();
            foreach (var doc in Doc)
            {
                SimpleDoctorDTO simpleDoctor = new SimpleDoctorDTO
                {
                    DoctorID = doc.DoctorID,
                    Name = doc.FirstName + " " + doc.LastName,
                    Description = doc.Description,
                    AveRating = doc.AveRating,
                    ImgUrl=doc.ImgUrl,

                };
                Doctors.Add(simpleDoctor);
            }
            return Ok(Doctors);
            // return Ok();
        }

        [HttpGet("Id/{DoctorID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DoctorDTO> GetCategory(int DoctorID)
        {
            var Doctor = doctorRepository.GetOne(expression: e => e.DoctorID == DoctorID);
            if (Doctor == null)

            DoctorDTO Doc = new DoctorDTO(Doctor); 
            return Ok(Doc);

             
        }

   




    }
}
