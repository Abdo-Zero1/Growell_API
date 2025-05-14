using DataAccess.DTOS;

using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using Growell_API.Services;
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
        public IActionResult Index() {
            var Doc = doctorRepository.Get().ToList();
            return Ok(Doc);
        }

   




    }
}
