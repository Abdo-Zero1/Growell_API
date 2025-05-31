using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestHomeController : ControllerBase
    {
        private readonly ITestRepository testRepository;
        private readonly ITestResultRepository testResultRepository;

        public TestHomeController(ITestRepository testRepository, ITestResultRepository testResultRepository)
        {
            this.testRepository = testRepository;
            this.testResultRepository = testResultRepository;
        }

        [HttpGet("category/{categoryId}/tests")]
        public IActionResult GetTestsByCategory(int categoryId)
        {
            var tests = testRepository.Get(
                expression: t => t.CategoryID == categoryId && t.IsActive,
                tracked: true
            ).Select(t => new
            {
                t.TestID,
                t.TestName,
                t.Description,
                t.NumberOfQuestions,
                t.IsActive
            }).ToList();

            if (!tests.Any())
            {
                return NotFound(new { Message = "No tests found for this category." });
            }

            return Ok(tests);
        }

        [HttpGet("{testId}/questions")]
        public IActionResult GetQuestionsByTest(int testId)
        {
            var test = testRepository.Get(
                new Expression<Func<Test, object>>[] { t => t.Questions }, 
                t => t.TestID == testId, 
                true 
            ).FirstOrDefault();

            if (test == null)
            {
                return NotFound(new { Message = "Test not found" });
            }

            var questions = test.Questions.Select(q => new
            {
                QuestionID = q.QuestionID,
                QuestionText = q.QuestionText,
                AnswerOptions = new[] { q.AnswerOption1, q.AnswerOption2, q.AnswerOption3, q.AnswerOption4 },
            }).ToList();

            return Ok(questions);
        }



        [HttpPost("{testId}/submit")]
        public IActionResult SubmitTest(int testId, [FromBody] List<string> userAnswers)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var existingResult = testResultRepository.Get(
                expression: tr => tr.UserID == userId && tr.TestID == testId,
                tracked: false
            ).FirstOrDefault();

            if (existingResult != null)
            {
                return BadRequest(new { Message = "You have already submitted this test." });
            }

            var tests = testRepository.Get(
                new Expression<Func<Test, object>>[] { t => t.Questions },
                t => t.TestID == testId,
                true
            );

            var test = tests.FirstOrDefault();

            if (test == null)
            {
                return NotFound(new { Message = "Test not found" });
            }

            var questions = test.Questions.OrderBy(q => q.OrderNumber).ToList();

            if (questions.Count != userAnswers.Count)
            {
                return BadRequest(new { Message = "Number of answers does not match the number of questions" });
            }

            int score = 0;

            for (int i = 0; i < questions.Count; i++)
            {
                if (string.Equals(questions[i].CorrectAnswer?.Trim(), userAnswers[i]?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    score++;
                }
            }

            SaveResult(userId, testId, score);

            return Ok(new
            {
                TotalQuestions = questions.Count,
                CorrectAnswers = score,
                Score = $"{score}/{questions.Count}"
            });
        }

        private void SaveResult(string userId, int testId, int score)
        {
            var test = testRepository.GetOne(expression: t => t.TestID == testId);
            if (test == null)
            {
                throw new Exception("Test not found");
            }

            var result = new TestResult
            {
                UserID = userId,
                TestID = testId,
                DoctorID = test.DoctorID, 
                Score = score,
                TakenAt = DateTime.UtcNow
            };

            testResultRepository.Create(result);
            testResultRepository.Commit();
        }





    }
}
