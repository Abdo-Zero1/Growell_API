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
                AboutOfBook = bookDTO.AboutOfBook,
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

        [HttpPut]
        [Route("EditBookEvent/{id}")]
        public IActionResult Edit(int id, [FromForm] BookDTO bookDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingBook = bookEventRepository.GetOne(expression: b => b.BookEventId == id);
            if (existingBook == null)
                return NotFound("The book with the specified ID does not exist.");

            if (!string.IsNullOrWhiteSpace(bookDTO.BookTitle))
                existingBook.BookTitle = bookDTO.BookTitle;

            if (bookDTO.Description != null)
                existingBook.Description = bookDTO.Description;

            if (bookDTO.AboutOfBook != null)
                existingBook.AboutOfBook = bookDTO.AboutOfBook;

            if (!string.IsNullOrWhiteSpace(bookDTO.BookUrl))
                existingBook.BookUrl = bookDTO.BookUrl;

            if (bookDTO.BookImage != null && bookDTO.BookImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                if (!allowedExtensions.Contains(Path.GetExtension(bookDTO.BookImage.FileName).ToLower()))
                    return BadRequest("Only JPG and PNG image files are allowed.");

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(bookDTO.BookImage.FileName);
                string folderName = Path.Combine("wwwroot", "images", "Books");
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName, fileName);

                Directory.CreateDirectory(folderName);

                try
                {
                    if (!string.IsNullOrEmpty(existingBook.BookImagePath))
                    {
                        string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingBook.BookImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        bookDTO.BookImage.CopyTo(stream);
                    }

                    existingBook.BookImagePath = $"/images/Books/{fileName}";
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            bookEventRepository.Edit(existingBook);
            bookEventRepository.Commit();

            return Ok(existingBook);
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
