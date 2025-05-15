using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger, IDoctorRepository doctorRepository)
        {
            this.logger = logger;
            this.doctorRepository = doctorRepository;
        }

        [HttpGet]
        public IActionResult Index(int pageNum = 1, int pageSize = 10)
        {
            try
            {
                var query = doctorRepository.Get();
                var totalRecords = query.Count();
                var doctor = query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
                var doctors = doctorRepository.Get().ToList();

                var response = new
                {
                    TotalCount = totalRecords,
                    PageNumber = pageNum,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                    Data = doctor
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error fetching doctors: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching data from database.");
            }
        }
    }
}
