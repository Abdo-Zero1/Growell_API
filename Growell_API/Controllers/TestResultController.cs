using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;
using System.Security.Claims;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestResultController : ControllerBase
    {
        private readonly ITestResultRepository testResultRepository;
        private readonly IDoctorRepository doctorRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ITestRepository testRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public TestResultController(
            ITestResultRepository testResultRepository,
            IDoctorRepository doctorRepository,
            ICategoryRepository categoryRepository,
            ITestRepository testRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.testResultRepository = testResultRepository;
            this.doctorRepository = doctorRepository;
            this.categoryRepository = categoryRepository;
            this.testRepository = testRepository;
            this.userManager = userManager;
        }

        [HttpGet("GetReports")]
        public IActionResult GetReports()
        {
            // Get user email from the token
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { message = "Invalid token. Email not found." });
            }

            // Check if user is a doctor
            var doctor = doctorRepository.GetOne(expression: d => d.Email == email);

            List<TestResult> testResults;

            if (doctor != null)
            {
                // User is a doctor, retrieve reports for all their patients
                testResults = testResultRepository.Get(expression: r => r.DoctorID == doctor.DoctorID).ToList();
            }
            else
            {
                // User is not a doctor, retrieve their own test results
                var user = userManager.Users.FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    return Unauthorized(new { message = "User not found." });
                }

                testResults = testResultRepository.Get(expression: r => r.UserID == user.Id).ToList();
            }

            // Check if there are any results
            if (!testResults.Any())
            {
                return NotFound(new
                {
                    messageEn = "No test results found.",
                    messageAr = "لا توجد نتائج اختبارات."
                });
            }

            // Generate the report
            var report = testResults.Select(tr =>
            {
                var test = testRepository.GetOne(expression: t => t.TestID == tr.TestID);
                var category = categoryRepository.GetOne(expression: c => c.CategoryID == test.CategoryID);
                var user = userManager.Users.FirstOrDefault(u => u.Id == tr.UserID);

                return new
                {
                    TestID = tr.TestID,
                    Score = tr.Score,
                    TakenAt = tr.TakenAt,
                    UserName = user?.UserName,
                    CategoryName = category?.Name,
                    ProfilePicture = user?.ProfilePicturePath
                };
            }).ToList();

            return Ok(report);
        }
    }
}
