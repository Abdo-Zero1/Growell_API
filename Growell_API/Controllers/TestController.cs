using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq.Expressions;
using System.Security.Claims;
using Utility;
using static System.Net.Mime.MediaTypeNames;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class TestController : ControllerBase
    {
        private readonly ITestRepository testRepository;
        private readonly IDoctorRepository doctorRepository;
        private readonly IQuestionRepository questionRepository;
        private readonly ICategoryRepository categoryRepository;

        public TestController(ITestRepository testRepository, IDoctorRepository doctorRepository,IQuestionRepository questionRepository, ICategoryRepository categoryRepository)
        {
            this.testRepository = testRepository;
            this.doctorRepository = doctorRepository;
            this.questionRepository = questionRepository;
            this.categoryRepository = categoryRepository;
        }
        [Authorize]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
                {
                    return Unauthorized(new { message = "Invalid token or DoctorID is missing." });
                }

                var tests = testRepository.Get(
                    Include: new Expression<Func<Test, object>>[]
                    {
                t => t.Doctor,
                t => t.Questions
                    },
                    expression: t => t.DoctorID == doctorId
                ).ToList();

                if (!tests.Any())
                {
                    return NotFound(new { message = "No tests found for the current doctor." });
                }

                var result = tests.Select(test => new
                {
                    TestId = test.TestID,
                    TestName = test.TestName,
                    Description = test.Description,
                    NumberOfQuestions = test.Questions?.Count ?? 0,
                    IsActive = test.IsActive,
                    DoctorName = test.Doctor != null ? $"{test.Doctor.FirstName} {test.Doctor.LastName}" : null,
                    ImageUrl = test.Doctor?.ImgUrl,
                    Bio = test.Doctor?.Bio,
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving tests.",
                    error = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost("Create")]
        public IActionResult Create([FromBody] Test test)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
            {
                return Unauthorized(new { message = "Invalid token or DoctorID is missing." });
            }

            test.DoctorID = doctorId;

            if (test.CategoryID == 0)
                return BadRequest(new { message = "CategoryID is required." });

            var doctor = doctorRepository.GetOne(null, d => d.DoctorID == doctorId);
            if (doctor == null)
                return BadRequest(new { message = "Doctor not found." });

            var category = categoryRepository.GetOne(null, c => c.CategoryID == test.CategoryID);
            if (category == null)
                return BadRequest(new { message = "Category not found." });

            try
            {
                testRepository.Create(test);
                testRepository.Commit();

                return Ok(new { message = "Test created successfully", testId = test.TestID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the test", error = ex.Message });
            }
        }


        [Authorize]
        [HttpPut("Edit/{id}")]
        public IActionResult Edit(int id, [FromBody] Test test)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
                {
                    return Unauthorized(new { message = "Invalid token or DoctorID is missing." });
                }

                var existingTest = testRepository.GetOne(expression: t => t.TestID == id);

                if (existingTest == null)
                {
                    return NotFound(new { message = "Test not found" });
                }

                if (existingTest.DoctorID != doctorId)
                {
                    return Forbid("You are not authorized to edit this test.");
                }

                existingTest.TestName = test.TestName;
                existingTest.Description = test.Description;
                existingTest.CategoryID = test.CategoryID;
                existingTest.NumberOfQuestions = test.NumberOfQuestions;
                existingTest.IsActive = test.IsActive;
                existingTest.DoctorID = doctorId;  

                testRepository.Edit(existingTest);
                testRepository.Commit();

                return Ok(new { message = "Test updated successfully", testId = existingTest.TestID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the test", error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
                {
                    return Unauthorized(new { message = "Invalid token or DoctorID is missing." });
                }

                var test = testRepository.GetOne(expression: t => t.TestID == id);

                if (test == null)
                {
                    return NotFound(new { message = "Test not found" });
                }

                if (test.DoctorID != doctorId)
                {
                    return Forbid("You are not authorized to delete this test.");
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
