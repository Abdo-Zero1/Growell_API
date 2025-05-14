
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq.Expressions;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = $"{SD.AdminRole}")]

    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly ITestRepository testRepository;

        public DoctorController(IDoctorRepository doctorRepository, ITestRepository testRepository)
        {
            this.doctorRepository = doctorRepository;
            this.testRepository = testRepository;
        }
        [HttpGet]
       
        public IActionResult Index()
        {
            var Doc = testRepository.Get(Include: new Expression<Func<Test, object>>[]
                           {
        t => t.Doctor,
        t => t.TestResults
                           }).ToList();
            return Ok(Doc); 
        }







    }
}
