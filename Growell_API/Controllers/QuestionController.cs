using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq.Expressions;
using System.Security.Claims;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   [Authorize]

    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository questionRepository;
        private readonly ITestRepository testRepository;
        private readonly IDoctorRepository doctorRepository;

        public QuestionController(IQuestionRepository questionRepository, ITestRepository testRepository, IDoctorRepository doctorRepository)
        {
            this.questionRepository = questionRepository;
            this.testRepository = testRepository;
            this.doctorRepository = doctorRepository;
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

                var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == doctorId);
                if (doctor == null)
                {
                    return NotFound(new { message = "Doctor not found." });
                }

                var questions = questionRepository.Get(expression: q => q.DoctorID == doctorId);

                if (!questions.Any())
                {
                    return NotFound(new { message = "No questions found for the current doctor." });
                }

                var result = questions.Select(q => new
                {
                    q.QuestionID,
                    q.QuestionText,
                    q.AnswerOption1,
                    q.AnswerOption2,
                    q.AnswerOption3,
                    q.AnswerOption4,
                    q.CorrectAnswer,
                    q.OrderNumber,
                    q.CreatedAt,
                    Doctor = new
                    {
                        doctor.DoctorID,
                        doctor.FirstName,
                        doctor.LastName,
                        doctor.Bio,
                        doctor.ImgUrl
                    }
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving questions.",
                    error = ex.Message
                });
            }
        }


        [Authorize]
        [HttpPost("Create")]
        public IActionResult Create([FromBody] Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
                {
                    return Unauthorized(new { message = "Invalid token or DoctorID missing." });
                }

                // جلب التيست والتأكد انه يعود للطبيب صاحب التوكن
                var relatedTest = testRepository.GetOne(null, t => t.TestID == question.TestID && t.DoctorID == doctorId);
                if (relatedTest == null)
                {
                    return NotFound(new { message = "The associated test does not exist or does not belong to the current doctor." });
                }

                // ربط السؤال بالطبيب صاحب التوكن
                question.DoctorID = doctorId;
                question.CreatedAt = DateTime.Now;

                questionRepository.Create(question);
                questionRepository.Commit();

                return Ok(new
                {
                    message = "Question created successfully.",
                    question
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the question.",
                    error = ex.Message
                });
            }
        }


        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
            {
                return Unauthorized(new { message = "Invalid token or DoctorID missing." });
            }

            var question = questionRepository.GetOne(null, q => q.QuestionID == id && q.DoctorID == doctorId);

            if (question == null)
            {
                return NotFound(new
                {
                    message = "Question not found or you do not have access to it.",
                    questionId = id
                });
            }

            var doctor = doctorRepository.GetOne(null, d => d.DoctorID == doctorId);

            var result = new
            {
                question.QuestionID,
                question.QuestionText,
                question.AnswerOption1,
                question.AnswerOption2,
                question.AnswerOption3,
                question.AnswerOption4,
                question.CorrectAnswer,
                question.OrderNumber,
                question.CreatedAt,
                Doctor = doctor != null ? new
                {
                    doctor.DoctorID,
                    doctor.FirstName,
                    doctor.LastName,
                    doctor.ImgUrl
                } : null
            };

            return Ok(new
            {
                message = "Question retrieved successfully.",
                data = result
            });
        }
        [Authorize]
        [HttpPut("Edit/{id}")]
        public IActionResult Edit(int id, [FromBody] Question updatedQuestion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorIdFromToken))
            {
                return Unauthorized(new { message = "Invalid token or DoctorID missing." });
            }

            try
            {
                var oldQuestion = questionRepository.GetOne(expression: q => q.QuestionID == id && q.DoctorID == doctorIdFromToken);
                if (oldQuestion == null)
                {
                    return NotFound(new { message = "Question not found or you don't have permission to edit it." });
                }

                var doctorExists = doctorRepository.GetOne(expression: d => d.DoctorID == doctorIdFromToken);
                if (doctorExists == null)
                {
                    return NotFound(new { message = "Doctor associated with token does not exist." });
                }

                // تحديث باقي الحقول بدون تغيير DoctorID
                oldQuestion.QuestionText = updatedQuestion.QuestionText;
                oldQuestion.AnswerOption1 = updatedQuestion.AnswerOption1;
                oldQuestion.AnswerOption2 = updatedQuestion.AnswerOption2;
                oldQuestion.AnswerOption3 = updatedQuestion.AnswerOption3;
                oldQuestion.AnswerOption4 = updatedQuestion.AnswerOption4;
                oldQuestion.CorrectAnswer = updatedQuestion.CorrectAnswer;
                oldQuestion.OrderNumber = updatedQuestion.OrderNumber;
                oldQuestion.TestID = updatedQuestion.TestID;

                // تأكد DoctorID ثابت حسب التوكن
                oldQuestion.DoctorID = doctorIdFromToken;

                questionRepository.Edit(oldQuestion);
                questionRepository.Commit();

                return Ok(new { message = "Question updated successfully.", question = oldQuestion });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the question.", error = ex.Message });
            }
        }


        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorIdFromToken))
            {
                return Unauthorized(new { message = "Invalid token or DoctorID missing." });
            }

            try
            {
                var question = questionRepository.GetOne(
                    includeProps: new Expression<Func<Question, object>>[] { q => q.Doctor },
                    expression: q => q.QuestionID == id && q.DoctorID == doctorIdFromToken
                );

                if (question == null)
                {
                    return NotFound(new { message = "Question not found or you don't have permission to delete it." });
                }

                questionRepository.Delete(question);
                questionRepository.Commit();

                return Ok(new
                {
                    message = "Question deleted successfully.",
                    questionId = id,
                    questionText = question.QuestionText,
                    doctorName = question.Doctor != null
                        ? $"{question.Doctor.FirstName} {question.Doctor.LastName}"
                        : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while deleting the question.",
                    error = ex.Message
                });
            }
        }


    }
}
