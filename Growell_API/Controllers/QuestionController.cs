using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
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
        [HttpGet]
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
                    return Ok(new
                    {
                        message = "No questions found for the current doctor.",
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
        public IActionResult Create([FromBody] CreateQuestionDTO questionDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

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

                var test = testRepository.GetOne(expression: t => t.TestID == questionDTO.TestID && t.DoctorID == doctorId);
                if (test == null)
                {
                    return NotFound(new { message = "The associated test does not exist or does not belong to the current doctor." });
                }

                var question = new Question
                {
                    QuestionText = questionDTO.QuestionText,
                    AnswerOption1 = questionDTO.AnswerOption1,
                    AnswerOption2 = questionDTO.AnswerOption2,
                    AnswerOption3 = questionDTO.AnswerOption3,
                    AnswerOption4 = questionDTO.AnswerOption4,
                    CorrectAnswer = questionDTO.CorrectAnswer,
                    OrderNumber = questionDTO.OrderNumber,
                    TestID = questionDTO.TestID,
                    DoctorID = doctorId,
                    CreatedAt = DateTime.UtcNow
                };

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
                question.CreatedBy,
                question.TestID,
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
        public IActionResult Edit(int id, [FromBody] EditQuestionDTO updatedQuestion)
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

                if (doctorRepository.GetOne(expression: d => d.DoctorID == doctorIdFromToken) == null)
                {
                    return NotFound(new { message = "Doctor associated with token does not exist." });
                }


                oldQuestion.QuestionText = !string.IsNullOrEmpty(updatedQuestion.QuestionText) ? updatedQuestion.QuestionText : oldQuestion.QuestionText;
                oldQuestion.AnswerOption1 = !string.IsNullOrEmpty(updatedQuestion.AnswerOption1) ? updatedQuestion.AnswerOption1 : oldQuestion.AnswerOption1;
                oldQuestion.AnswerOption2 = !string.IsNullOrEmpty(updatedQuestion.AnswerOption2) ? updatedQuestion.AnswerOption2 : oldQuestion.AnswerOption2;
                oldQuestion.AnswerOption3 = !string.IsNullOrEmpty(updatedQuestion.AnswerOption3) ? updatedQuestion.AnswerOption3 : oldQuestion.AnswerOption3;
                oldQuestion.AnswerOption4 = !string.IsNullOrEmpty(updatedQuestion.AnswerOption4) ? updatedQuestion.AnswerOption4 : oldQuestion.AnswerOption4;
                oldQuestion.CorrectAnswer = !string.IsNullOrEmpty(updatedQuestion.CorrectAnswer) ? updatedQuestion.CorrectAnswer : oldQuestion.CorrectAnswer;
                oldQuestion.OrderNumber = updatedQuestion.OrderNumber.HasValue && updatedQuestion.OrderNumber > 0? updatedQuestion.OrderNumber.Value: oldQuestion.OrderNumber;
                oldQuestion.TestID = updatedQuestion.TestID.HasValue && updatedQuestion.TestID > 0? updatedQuestion.TestID.Value : oldQuestion.TestID;

                oldQuestion.CreatedBy = doctorIdFromToken;
                oldQuestion.CreatedAt = DateTime.Now;

                questionRepository.Edit(oldQuestion);
                questionRepository.Commit();

                return Ok(new
                {
                    message = "Question updated successfully.",
                    question = new
                    {
                        oldQuestion.QuestionID,
                        oldQuestion.QuestionText,
                        oldQuestion.AnswerOption1,
                        oldQuestion.AnswerOption2,
                        oldQuestion.AnswerOption3,
                        oldQuestion.AnswerOption4,
                        oldQuestion.CorrectAnswer,
                        oldQuestion.OrderNumber,
                        oldQuestion.TestID
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while updating the question.",
                    error = ex.Message
                });
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
