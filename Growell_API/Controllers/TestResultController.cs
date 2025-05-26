using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{SD.AdminRole},{SD.DoctorRole}")]
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

        [HttpGet("Get")]
        public IActionResult Index()
        {
            var testResults = testResultRepository.Get().ToList();

            var report = testResults.Select(tr =>
            {
                var test = testRepository.GetOne(expression: t => t.TestID == tr.TestID);

                var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == test .DoctorID);
                var category = categoryRepository.GetOne(expression: c => c.CategoryID == test.CategoryID);
                var user = userManager.FindByIdAsync(tr.UserID).Result;
                var doctorName = doctor != null
                    ? $"{doctor.FirstName ?? "غير متوفر"} {doctor.SecondName ?? ""} {doctor.LastName ?? ""}".Trim()
                    : "Doctor not found";

                return new
                {
                    TestID = tr.TestID,
                    Score = tr.Score,
                    TakenAt = tr.TakenAt,
                    UserName = user?.UserName ,
                    DoctorName = doctorName ,
                    CategoryName = category?.Name,
                    Image = user?.ProfilePicturePath ,

                };
            }).ToList();

            return Ok(report);
        }
    }
}
