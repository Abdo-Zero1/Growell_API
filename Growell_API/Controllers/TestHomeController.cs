using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq.Expressions;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public IActionResult SubmitTest(int UserId ,int testId, [FromBody] List<int> userAnswers)
        {
            var tests = testRepository.Get(
                /*includeProps:*/ new Expression<Func<Test, object>>[] { t => t.Questions },
                expression: t => t.TestID == testId,
                tracked: true
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
                if (questions[i].CorrectAnswer == userAnswers[i].ToString())
                {
                    score++;
                }
            }
            saveReslut(UserId, testId, score);

            return Ok(new
            {
                TotalQuestions = questions.Count,
                CorrectAnswers = score,
                Score = $"{score}/{questions.Count}"
            });
        }
        
        private async void saveReslut(int userid, int testId, int score)
        {
            testResultRepository.Create(new TestResult
            {
                UserID = userid, 
                TestID = testId,
                Score = score,
                TakenAt = DateTime.Now
            });

         testResultRepository.Commit();
        }


    }
}
