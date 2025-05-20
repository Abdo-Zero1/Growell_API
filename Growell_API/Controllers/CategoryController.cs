using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Data;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   [Authorize(Roles = $"{SD.DoctorRole}")]

    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly ITestRepository testRepository;

        public CategoryController(ICategoryRepository categoryRepository, ITestRepository testRepository)
        {
            this.categoryRepository = categoryRepository;
            this.testRepository = testRepository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var Category = categoryRepository.Get(Include: [t=>t.Tests]).ToList();
            return Ok(Category);
        }
        

        [HttpPost]
        [Route("Create")]
        public IActionResult Create([FromBody]Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            categoryRepository.Create(category);
            categoryRepository.Commit();

            return CreatedAtAction(nameof(GetCategory), new { CategoryId = category.CategoryID }, category);
        }

        [HttpGet("{CategoryId}")]
        public IActionResult GetCategory(int CategoryId)
        {
            var category = categoryRepository.GetOne( expression: e => e.CategoryID == CategoryId);
            if (category != null)
                return Ok(category); 

            return NotFound(new { message = "Category not found." }); 
        }

        [HttpPut]
        [Route("Edit")]
        public IActionResult Edit([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var existingCategory = categoryRepository.GetOne(expression: e => e.CategoryID == category.CategoryID);
            if (existingCategory == null)
            {
                return NotFound(new { message = "Category not found." });
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

        [HttpDelete("{categoryId}")]
        public IActionResult Delete(int categoryId)
        {
            var category = categoryRepository.GetOne(expression: e => e.CategoryID == categoryId);
            if (category == null)
                return NotFound(new { message = "Category not found." });

            categoryRepository.Delete(category);
            categoryRepository.Commit();

            return Ok(new { message = "Category deleted successfully." });
        }


    }
}
