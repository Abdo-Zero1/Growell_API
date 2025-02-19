using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{SD.AdminRole},{SD.DoctorRole}")]

    public class TestController : ControllerBase
    {
        private readonly ITestRepository testRepository;

        public TestController(ITestRepository testRepository)
        {
            this.testRepository = testRepository;
        }
        [HttpGet("Index")]
        public IActionResult Index()
        {
            var test = testRepository.Get();
            return Ok(test);
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] Test test)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid data provided.", errors = ModelState });
                }

                // إضافة الاختبار الجديد إلى قاعدة البيانات
                testRepository.Create(test);
                testRepository.Commit();

                return Ok(new { message = "Test created successfully.", testId = test.TestID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the test.", error = ex.Message });
            }
        }


    }
}
