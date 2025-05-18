using DataAccess.Repository;
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
    //[Authorize(Roles = $"{SD.DoctorRole}")]

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

        [HttpPost("Create")]
        public IActionResult Create([FromBody] Test test)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (test.DoctorID == 0 || test.CategoryID == 0)
                return BadRequest(new { message = "DoctorID and CategoryID are required." });

            var doctor = doctorRepository.GetOne(null, d => d.DoctorID == test.DoctorID);
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

        [HttpPut("Edit/{id}")]
        public IActionResult Edit(int id, [FromBody] Test test)
        {
            try
            {

                var existingTest = testRepository.GetOne(expression: t => t.TestID == id);
                //if (id != test.TestID)
                //{
                //    return BadRequest(new { message = "Test ID mismatch" });
                //}

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
