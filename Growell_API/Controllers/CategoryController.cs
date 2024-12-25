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
    [Authorize(Roles = $"{SD.AdminRole}")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var Category = categoryRepository.Get().ToList();
            return Ok(Category);
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                
                categoryRepository.Create(category);
                categoryRepository.Commit();

                
                return CreatedAtAction(nameof(Category), new { id = category }, category);
            }
            return Ok(category);  
        }

    }
}
