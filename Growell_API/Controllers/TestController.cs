﻿using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
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
        [HttpGet("Get")]
        public IActionResult Get()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
                {
                    return Unauthorized(new { message = "Invalid token or DoctorID is missing." });
                }

                var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == doctorId);
                if (doctor == null)
                {
                    return NotFound(new { message = "Doctor not found." });
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
                    return Ok(new
                    {
                        message = "No tests found for the current doctor.",
                        Doctor = new
                        {
                            doctor.DoctorID,
                            doctor.FirstName,
                            doctor.LastName,
                            doctor.Bio,
                            doctor.ImgUrl
                        }
                    });
                }

                var result = tests.Select(test => new
                {
                    TestId = test.TestID,
                    TestName = test.TestName,
                    Description = test.Description,
                    NumberOfQuestions = test.Questions?.Count ?? 0,
                    IsActive = test.IsActive,
                    Doctor = new
                    {
                        DoctorName = test.Doctor != null ? $"{test.Doctor.FirstName} {test.Doctor.LastName}" : null,
                        ImageUrl = test.Doctor?.ImgUrl,
                        Bio = test.Doctor?.Bio,

                    }
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
        public IActionResult Create([FromBody] CreateTestDTO test)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
            {
                return Unauthorized(new { message = "Invalid token or DoctorID is missing." });
            }

            var doctor = doctorRepository.GetOne(null, d => d.DoctorID == doctorId);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            var category = categoryRepository.GetOne(null, c => c.CategoryID == test.CategoryID);
            if (category == null)
                return NotFound(new { message = "Category not found." });

            try
            {
                var newTest = new Test
                {
                    TestName = test.TestName,
                    Description = test.Description,
                    CategoryID = test.CategoryID,
                    DoctorID = doctorId,
                    NumberOfQuestions = test.NumberOfQuestions,
                    IsActive = test.IsActive,
                };

                testRepository.Create(newTest);
                testRepository.Commit();

                return Ok(new { message = "Test created successfully.", testId = newTest.TestID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the test.", error = ex.Message });
            }
        }


        [Authorize]
        [HttpGet("GetById/{testId}")]
        public IActionResult GetById(int testId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
                {
                    return Unauthorized(new { message = "Invalid token or DoctorID missing." });
                }

                var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == doctorId);
                if (doctor == null)
                {
                    return NotFound(new { message = "Doctor not found." });
                }

                var test = testRepository.GetOne(expression: t => t.TestID == testId && t.DoctorID == doctorId);

                if (test != null)
                {
                    test.Doctor = doctorRepository.GetOne(expression: d => d.DoctorID == test.DoctorID);
                    test.Questions = questionRepository.Get( expression: q => q.TestID == test.TestID).ToList();
                }


                var result = new
                {
                    TestId = test.TestID,
                    TestName = test.TestName,
                    Description = test.Description,
                    NumberOfQuestions = test.Questions?.Count ?? 0,
                    IsActive = test.IsActive,
                    CategoryID = test.CategoryID,
                   
                    Doctor = new
                    {
                        doctor.DoctorID,
                        doctor.FirstName,
                        doctor.LastName,
                        doctor.Bio,
                        doctor.ImgUrl
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving the test.",
                    error = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPut("Edit/{id}")]
        public IActionResult Edit(int id, [FromBody] EditTestDTO updatedTest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

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
                    return NotFound(new { message = "Test not found." });
                }

                if (existingTest.DoctorID != doctorId)
                {
                    return Forbid("You are not authorized to edit this test." );
                }

                if (updatedTest.CategoryID != 0)
                {
                    var category = categoryRepository.GetOne(null, c => c.CategoryID == updatedTest.CategoryID);
                    if (category == null)
                    {
                        return NotFound(new { message = "Category not found." });
                    }
                    existingTest.CategoryID = updatedTest.CategoryID;
                }

                if (!string.IsNullOrEmpty(updatedTest.TestName)) existingTest.TestName = updatedTest.TestName;
                if (!string.IsNullOrEmpty(updatedTest.Description)) existingTest.Description = updatedTest.Description;
                if (updatedTest.NumberOfQuestions > 0) existingTest.NumberOfQuestions = updatedTest.NumberOfQuestions;
                existingTest.IsActive = updatedTest.IsActive;

                testRepository.Edit(existingTest);
                testRepository.Commit();

                return Ok(new { message = "Test updated successfully.", testId = existingTest.TestID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the test.", error = ex.Message });
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
