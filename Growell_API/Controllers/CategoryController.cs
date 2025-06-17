using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq.Expressions;
using System.Security.Claims;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = $"{SD.DoctorRole}")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly ITestRepository testRepository;
        private readonly IDoctorRepository doctorRepository;

        public CategoryController(ICategoryRepository categoryRepository, ITestRepository testRepository, IDoctorRepository doctorRepository)
        {
            this.categoryRepository = categoryRepository;
            this.testRepository = testRepository;
            this.doctorRepository = doctorRepository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
            {
                return Unauthorized(new { message = "Invalid token or DoctorID is missing." });
            }

            var categories = categoryRepository.Get(
                Include: new Expression<Func<Category, object>>[] { t => t.Tests },
                expression: c => c.DoctorID == doctorId
            ).ToList();

            if (!categories.Any())
            {
                return Ok(new { message = "No categories found for the current doctor." });
            }

            return Ok(categories);
        }


        [HttpPost("Create")]
        public IActionResult Create([FromBody] Category category)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
            {
                return Unauthorized(new { message = "Invalid token or DoctorID is missing." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            category.DoctorID = doctorId; 

            try
            {
                categoryRepository.Create(category);
                categoryRepository.Commit();

                return CreatedAtAction(nameof(GetCategory), new { CategoryId = category.CategoryID }, category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the category.", error = ex.Message });
            }
        }

        [HttpGet("{CategoryId}")]
        public IActionResult GetCategory(int CategoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
            {
                return Unauthorized(new { message = "Invalid token or DoctorID is missing." });
            }

            var category = categoryRepository.GetOne(expression: c => c.CategoryID == CategoryId && c.DoctorID == doctorId);
            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            return Ok(category);
        }

        [HttpPut("Edit/{id}")]
        public IActionResult Edit(int id, [FromBody] Category category)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
            {
                return Unauthorized(new { message = "Invalid token or DoctorID is missing." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var existingCategory = categoryRepository.GetOne(expression: c => c.CategoryID == id && c.DoctorID == doctorId);
            if (existingCategory == null)
            {
                return NotFound(new { message = "Category not found or you are not authorized to edit it." });
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            try
            {
                categoryRepository.Edit(existingCategory);
                categoryRepository.Commit();

                return Ok(new { message = "Category updated successfully.", category = existingCategory });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the category.", error = ex.Message });
            }
        }



        [HttpDelete("{CategoryId}")]
        public IActionResult Delete(int CategoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int doctorId))
            {
                return Unauthorized(new { message = "Invalid token or DoctorID is missing." });
            }

            var category = categoryRepository.GetOne(expression: c => c.CategoryID == CategoryId && c.DoctorID == doctorId);
            if (category == null)
            {
                return NotFound(new { message = "Category not found or you are not authorized to delete it." });
            }

            try
            {
                categoryRepository.Delete(category);
                categoryRepository.Commit();

                return Ok(new { message = "Category deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the category.", error = ex.Message });
            }
        }
    }
}
