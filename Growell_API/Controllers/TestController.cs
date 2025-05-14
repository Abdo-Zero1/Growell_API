using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq.Expressions;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = $"{SD.AdminRole},{SD.DoctorRole}")]

    public class TestController : ControllerBase
    {
        private readonly ITestRepository testRepository;
        private readonly IDoctorRepository doctorRepository;
        private readonly IQuestionRepository questionRepository;

        public TestController(ITestRepository testRepository, IDoctorRepository doctorRepository,IQuestionRepository questionRepository)
        {
            this.testRepository = testRepository;
            this.doctorRepository = doctorRepository;
            this.questionRepository = questionRepository;
        }
        [HttpGet("Index")]
        public IActionResult Index()
        {
            var tests = testRepository.Get(Include: new Expression<Func<Test, object>>[]
            {
                t => t.Doctor,
                t => t.Questions
            }).ToList();

            return Ok(tests);
        }

        // Create a new test
        [HttpPost("Create")]
        public IActionResult Create([FromBody] Test test)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid data provided", errors = ModelState });
                }

                testRepository.Create(test);
                testRepository.Commit();

                return Ok(new { message = "Test created successfully", testId = test.TestID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the test", error = ex.Message });
            }
        }

        // Edit an existing test
        [HttpPut("Edit/{id}")]
        public IActionResult Edit(int id, [FromBody] Test test)
        {
            try
            {
                if (id != test.TestID)
                {
                    return BadRequest(new { message = "Test ID mismatch" });
                }

                var existingTest = testRepository.GetOne(expression: t => t.TestID == id);
                if (existingTest == null)
                {
                    return NotFound(new { message = "Test not found" });
                }

                existingTest.TestName = test.TestName;
                existingTest.Description = test.Description;
                existingTest.CategoryID = test.CategoryID;
                existingTest.NumberOfQuestions = test.NumberOfQuestions;
                existingTest.IsActive = test.IsActive;
                existingTest.DoctorID = test.DoctorID;

                testRepository.Edit(existingTest);
                testRepository.Commit();

                return Ok(new { message = "Test updated successfully", testId = test.TestID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the test", error = ex.Message });
            }
        }

        // Delete an existing test
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var test = testRepository.GetOne(expression: t => t.TestID == id);
                if (test == null)
                {
                    return NotFound(new { message = "Test not found" });
                }

                testRepository.Delete(test);
                testRepository.Commit();

                return Ok(new { message = "Test deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the test", error = ex.Message });
            }
        }


    }
}
