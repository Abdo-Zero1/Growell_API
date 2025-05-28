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

        [Authorize]
        [HttpGet("GetReport")]
        public IActionResult GetReport()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorIdFromToken))
            {
                return Unauthorized(new { message = "Invalid token or DoctorID missing." });
            }

            try
            {
                // تحديد إذا كان المستخدم طبيبًا بناءً على UserID
                var doctor = doctorRepository.GetOne(expression: d => d.UserID == userId);

                IEnumerable<TestResult> testResults;

                if (doctor != null)
                {
                    // إذا كان المستخدم طبيبًا، إرجاع تقارير المرضى الخاصة بالطبيب
                    testResults = testResultRepository.Get(expression: r => r.DoctorID == doctor.DoctorID).ToList();
                }
                else
                {
                    // إذا كان المستخدم عاديًا، إرجاع تقاريره فقط
                    testResults = testResultRepository.Get(expression: r => r.UserID == userId).ToList();
                }

                if (!testResults.Any())
                {
                    return NotFound(new
                    {
                        messageEn = "No test results found for the user.",
                        messageAr = "لا توجد نتائج اختبارات لهذا المستخدم."
                    });
                }

                var userIds = testResults.Select(r => r.UserID).Distinct().ToList();
                var users = userManager.Users.Where(u => userIds.Contains(u.Id))
                                             .Select(u => new { u.Id, u.UserName, u.ProfilePicturePath })
                                             .ToList();

                var report = testResults.Select(r =>
                {
                    var test = testRepository.GetOne(expression: t => t.TestID == r.TestID);
                    var category = CategoryRepository.GetOne(expression: c => c.CategoryID == test.CategoryID);
                    var totalQuestions = questionRepository.Get(expression: q => q.TestID == r.TestID).Count();
                    double percentage = totalQuestions > 0 ? (double)r.Score / totalQuestions * 100 : 0;

                    var user = users.FirstOrDefault(u => u.Id == r.UserID);
                    var doctorName = doctor != null
                        ? $"{doctor.FirstName ?? "غير متوفر"} {doctor.SecondName ?? ""} {doctor.LastName ?? ""}".Trim()
                        : "Doctor not found";

                    return new
                    {
                        UserName = user?.UserName,
                        ProfilePicture = user?.ProfilePicturePath,
                        TestName = test?.TestName,
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the report.", error = ex.Message });
            }
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
