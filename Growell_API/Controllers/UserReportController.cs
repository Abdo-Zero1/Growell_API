using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class UserReportController : ControllerBase
    {
        private readonly ITestResultRepository testResultRepository;
        private readonly ICategoryRepository CategoryRepository;
        private readonly IDoctorRepository doctorRepository;
        private readonly IQuestionRepository questionRepository;
        private readonly ITestRepository testRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public UserReportController(
            ITestResultRepository testResultRepository,
            ICategoryRepository CategoryRepository,
            IDoctorRepository doctorRepository,
            IQuestionRepository questionRepository,
            ITestRepository testRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.testResultRepository = testResultRepository;
            this.CategoryRepository = CategoryRepository;
            this.doctorRepository = doctorRepository;
            this.questionRepository = questionRepository;
            this.testRepository = testRepository;
            this.userManager = userManager;
        }

        [HttpGet("GetReport")]
        public IActionResult GetReport()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            IEnumerable<TestResult> testResults;

            if (roles.Contains(SD.AdminRole))
            {
                testResults = testResultRepository.Get().ToList();
            }
            else if (roles.Contains(SD.DoctorRole))
            {
                var doctor = doctorRepository.GetOne(expression: d => d.UserID == userId);
                if (doctor == null)
                    return BadRequest(new { message = "Doctor not found for this user." });

                testResults = testResultRepository.Get(expression: r => r.DoctorID == doctor.DoctorID).ToList();
            }
            else
            {
                testResults = testResultRepository.Get(expression: r => r.UserID == userId).ToList();
            }

            if (testResults == null || !testResults.Any())
                return NotFound(new
                {
                    messageEn = "No test results found for the user.",
                    messageAr = "لا توجد نتائج اختبارات لهذا المستخدم."
                });

            var userIds = testResults.Select(r => r.UserID).Distinct().ToList();
            var users = userManager.Users.Where(u => userIds.Contains(u.Id))
                                         .Select(u => new { u.Id, u.UserName })
                                         .ToList();

            var report = testResults.Select(r =>
            {
                var test = testRepository.GetOne(expression: t => t.TestID == r.TestID);
                var category = CategoryRepository.GetOne(expression: c => c.CategoryID == test.CategoryID);
                var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == test.DoctorID);
                var totalQuestions = questionRepository.Get(expression: q => q.TestID == r.TestID).Count();
                double percentage = totalQuestions > 0 ? (double)r.Score / totalQuestions * 100 : 0;

                var userName = users.FirstOrDefault(u => u.Id == r.UserID)?.UserName ?? "Unknown User";
                var doctorName = doctor != null
                    ? $"{doctor.FirstName ?? "غير متوفر"} {doctor.SecondName ?? ""} {doctor.LastName ?? ""}".Trim()
                    : "Doctor not found";
               
                return new
                {
                    //userId = r.UserID,
                    userName = userName,
                    TestName = test?.TestName ,
                   // TestID = r.TestID,
                    Score = r.Score,
                    TakenAt = r.TakenAt,
                    CategoryName = category?.Name,
                    DoctorName = doctorName,
                    Percentage = percentage,
                    ClassificationEn = GetDelayClassificationEn(percentage),
                    ClassificationAr = GetDelayClassificationAr(percentage)
                };
            }).ToList();

            return Ok(report);
        }



        private string GetDelayClassificationEn(double percentage)
        {
            if (percentage == 100)
                return "No Delay";
            else if (percentage >= 70)
                return "Mild Delay";
            else if (percentage >= 50)
                return "Mild to moderate delay detected. It is advisable to consult a doctor to prevent worsening";
            else
                return "Severe delay detected. It is highly recommended to contact a doctor immediately";
        }

        private string GetDelayClassificationAr(double percentage)
        {
            if (percentage == 100)
                return "لا يوجد تأخر";
            else if (percentage >= 70)
                return "تأخر خفيف";
            else if (percentage >= 50)
                return "تم الكشف عن تأخر خفيف إلى متوسط. من الأفضل استشارة طبيب لتجنب تدهور الحالة";
            else
                return "تم الكشف عن تأخر شديد. يُنصح بشدة بالتواصل مع طبيب فوراً";
        }

    }
}
