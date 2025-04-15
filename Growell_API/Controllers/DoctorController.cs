using DataAccess.DTOS;

using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using Growell_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = $"{SD.AdminRole}")]

    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly DoctorService _doctorService;
        
        public DoctorController(IDoctorRepository doctorRepository, DoctorService doctorService)
        {
            this.doctorRepository = doctorRepository;
            this._doctorService = doctorService;
          

        }


        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<SimpleDoctorsDTO>>> GetAllDoctors(int PageNumber=1,int PageSize=10) {

            if (PageNumber < 1 || PageSize < 1)
                return BadRequest("Page Number and Page size Must be greater than 0");

            var result = await doctorRepository.GetPagedDoctorsAsync(PageNumber, PageSize);
            if (result.Data.Count == 0)
            {
                return NotFound(new
                {
                    Message = "No Doctors found for the requested Page",
                    PageNumber=PageNumber,
                    PageSize=PageSize
                });
            }
            
            return Ok(result);
           
        }


        [HttpGet("Id/{DoctorID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorDTO>> GetCategory(int DoctorID)
        {
           var Doctor= await _doctorService.GetDoctorWithAppointment(DoctorID);
            if (Doctor == null) return NotFound($"The doctor with id {DoctorID} not found");

            return Ok(Doctor);
             
        }

   




    }
}
