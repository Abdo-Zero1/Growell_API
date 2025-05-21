using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = $"{SD.AdminRole},{SD.DoctorRole}")]

    public class BookEventController : ControllerBase
    {
        private readonly IBookEventRepository bookEventRepository;

        public BookEventController(IBookEventRepository bookEventRepository)
        {
            this.bookEventRepository = bookEventRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var Book = bookEventRepository.Get().ToList();
            return Ok(Book);
        }

        [HttpPost]
        [Route("CreateBookEvent")]
        public IActionResult Create([FromForm]BookDTO bookDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(bookDTO.BookImage==null||bookDTO.BookImage.Length==0)
                return BadRequest("Book data and image are required.");

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(bookDTO.BookImage.FileName);
            string FolderPath=Path.Combine(Directory.GetCurrentDirectory(), "images", "Books");
            Directory.CreateDirectory(FolderPath);
            string filePath= Path.Combine(FolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                bookDTO.BookImage.CopyTo(stream);
            }


            var Book = new BookEvent
            {
                BookTitle = bookDTO.BookTitle,
                Description = bookDTO.Description,
                BookUrl = bookDTO.BookUrl,
                BookImagePath = $"/images/Books/{fileName}"
            };


            bookEventRepository.Create(Book);
            bookEventRepository.Commit();

            return Ok(Book);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var bookEvent = bookEventRepository.GetOne(expression: e=>e.BookEventId == id);
            if (bookEvent == null)
            {
                return NotFound("BookEvent not found");
            }
            return Ok(bookEvent);

        }
   

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var bookEvent = bookEventRepository.GetOne(expression: e=>e.BookEventId== id);
            if (bookEvent == null)
            {
                return NotFound("BookEvent not found.");
            }

            if (!string.IsNullOrEmpty(bookEvent.BookImagePath))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", bookEvent.BookImagePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath); 
                }

                bookEventRepository.Delete(bookEvent);
                bookEventRepository.Commit();
                return Ok("success deleted");
            }

           

            return NoContent(); 
        }



    }
}
