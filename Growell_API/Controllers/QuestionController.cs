using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq.Expressions;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   //thorize(Roles = $"{SD.DoctorRole}")]

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
        [HttpGet("Index")]
        public IActionResult Index(int? doctorId)
        {
            try
            {
                var questions = doctorId.HasValue
                    ? questionRepository.Get(expression: q => q.DoctorID == doctorId.Value)
                    : questionRepository.Get();

                if (!questions.Any())
                {
                    return NotFound(new { message = "No questions found." });
                }

                var doctorIds = questions
                    .Where(q => q.DoctorID.HasValue)
                    .Select(q => q.DoctorID.Value)    
                    .Distinct()
                    .ToList();

                var doctors = doctorRepository.Get(expression: d => doctorIds.Contains(d.DoctorID))
                    .ToDictionary(d => d.DoctorID);

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
                    Doctor = q.DoctorID.HasValue && doctors.ContainsKey(q.DoctorID.Value) ? new
                    {
                        doctors[q.DoctorID.Value].DoctorID,
                        doctors[q.DoctorID.Value].FirstName,
                        doctors[q.DoctorID.Value].LastName,
                        doctors[q.DoctorID.Value].ImgUrl
                    } : null
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
                var relatedTest = testRepository.GetOne(null, t => t.TestID == question.TestID);
                if (relatedTest == null)
                {
                    return NotFound(new { message = "The associated test does not exist." });
                }


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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var question = questionRepository.GetOne(null, g => g.QuestionID == id);

            if (question == null)
            {
                return NotFound(new
                {
                    message = "Question not found.",
                    questionId = id
                });
            }

            // جلب بيانات الدكتور بناءً على DoctorID الخاص بالسؤال
            var doctor = question.DoctorID != null
                ? doctorRepository.GetOne(null, d => d.DoctorID == question.DoctorID)
                : null;

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



        [HttpPut("Edit{id}")]
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

            try
            {
                var oldQuestion = questionRepository.GetOne(expression: q => q.QuestionID == id);
                if (oldQuestion == null)
                {
                    return NotFound(new { message = "Question not found." });
                }

                var doctorExists = doctorRepository.GetOne(expression: d => d.DoctorID == updatedQuestion.DoctorID);
                if (doctorExists == null)
                {
                    return NotFound(new { message = "The selected doctor does not exist." });
                }

                oldQuestion.QuestionText = updatedQuestion.QuestionText;
                oldQuestion.AnswerOption1 = updatedQuestion.AnswerOption1;
                oldQuestion.AnswerOption2 = updatedQuestion.AnswerOption2;
                oldQuestion.AnswerOption3 = updatedQuestion.AnswerOption3;
                oldQuestion.AnswerOption4 = updatedQuestion.AnswerOption4;
                oldQuestion.CorrectAnswer = updatedQuestion.CorrectAnswer;
                oldQuestion.OrderNumber = updatedQuestion.OrderNumber;
                oldQuestion.TestID = updatedQuestion.TestID;
                oldQuestion.DoctorID = updatedQuestion.DoctorID;

                questionRepository.Edit(oldQuestion);
                questionRepository.Commit();

                return Ok(new { message = "Question updated successfully.", question = oldQuestion });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the question.", error = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var question = questionRepository.GetOne(
                    includeProps: new Expression<Func<Question, object>>[]
                    {
                q => q.Doctor 
                    },
                    expression: q => q.QuestionID == id
                );

                if (question == null)
                {
                    return NotFound(new { message = "Question not found." });
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
