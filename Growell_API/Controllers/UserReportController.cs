using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public UserReportController(
            ITestResultRepository testResultRepository,
            ICategoryRepository CategoryRepository,
            IDoctorRepository doctorRepository,
            IQuestionRepository questionRepository)
        {
            this.testResultRepository = testResultRepository;
            this.CategoryRepository = CategoryRepository;
            this.doctorRepository = doctorRepository;
            this.questionRepository = questionRepository;
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

                testResults = testResultRepository.Get( expression: r=> r.DoctorID == doctor.DoctorID).ToList();
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

            var report = testResults.Select(r =>
            {
                var category = CategoryRepository.GetOne(expression: c => c.CategoryID == r.CategoryID);
                var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == r.DoctorID);

                var totalQuestions = questionRepository.Get(expression: q => q.TestID == r.TestID).Count();

                double percentage = 0;
                if (totalQuestions > 0)
                    percentage = (double)r.Score / totalQuestions * 100;

                return new
                {
                    TestID = r.TestID,
                    Score = r.Score,
                    TakenAt = r.TakenAt,
                    CategoryName = category?.Name,
                    DoctorName = doctor?.FirstName,
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
