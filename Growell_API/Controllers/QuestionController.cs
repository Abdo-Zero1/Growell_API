using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{SD.DoctorRole}")]

    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository questionRepository;
        private readonly ITestRepository testRepository;

        public QuestionController(IQuestionRepository questionRepository, ITestRepository testRepository)
        {
            this.questionRepository = questionRepository;
            this.testRepository = testRepository;
        }
        [HttpGet("Index")]
        public IActionResult Index()
        {
            var Question = questionRepository.Get();
            return Ok(Question);
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

            return Ok(new
            {
                message = "Question retrieved successfully.",
                question
            });
        }


        [HttpPut("Edit{id}")]
        public IActionResult Edit(int id, [FromBody] Question updatedQuestion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var oldquestion = questionRepository.GetOne(expression: q=>q.QuestionID == id);

                    if (oldquestion == null)
                    {
                        return NotFound(new { message = "Question not found." });
                    }

                    oldquestion.QuestionText = updatedQuestion.QuestionText;
                    oldquestion.AnswerOption1 = updatedQuestion.AnswerOption1;
                    oldquestion.AnswerOption2 = updatedQuestion.AnswerOption2;
                    oldquestion.AnswerOption3 = updatedQuestion.AnswerOption3;
                    oldquestion.AnswerOption4 = updatedQuestion.AnswerOption4;
                    oldquestion.CorrectAnswer = updatedQuestion.CorrectAnswer;
                    oldquestion.OrderNumber = updatedQuestion.OrderNumber;
                    oldquestion.TestID = updatedQuestion.TestID;

                    questionRepository.Edit(oldquestion);
                    questionRepository.Commit();

                    return Ok(new { message = "Question updated successfully.", question = oldquestion });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "An error occurred while updating the question.", error = ex.Message });
                }
            }

            return BadRequest(new { message = "Invalid data.", errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var question = questionRepository.GetOne(expression: q => q.QuestionID == id);

                if (question == null)
                {
                    return NotFound(new { message = "Question not found." });
                }

                questionRepository.Delete(question);
                questionRepository.Commit();

                return Ok(new { message = "Question deleted successfully.", questionId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the question.", error = ex.Message });
            }
        }



    }
}
