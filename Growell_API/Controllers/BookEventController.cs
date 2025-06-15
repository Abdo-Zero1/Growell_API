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
            string FolderPath=Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Books");
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
            var response = new
            {
                bookEvent.BookEventId,
                bookEvent.BookTitle,
                bookEvent.Description,
                bookEvent.AboutOfBook,
                bookEvent.BookUrl,
            };
            return Ok(response);

        }

        [HttpPut]
        [Route("EditBookEvent/{id}")]
        public IActionResult Edit(int id, [FromForm] UpdateBookDTO updateBookDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingBook = bookEventRepository.GetOne(expression: b => b.BookEventId == id);
            if (existingBook == null)
                return NotFound("The book with the specified ID does not exist.");

            if (!string.IsNullOrWhiteSpace(updateBookDTO.BookTitle))
                existingBook.BookTitle = updateBookDTO.BookTitle;

            if (!string.IsNullOrWhiteSpace(updateBookDTO.Description))
                existingBook.Description = updateBookDTO.Description;

            if (!string.IsNullOrWhiteSpace(updateBookDTO.AboutOfBook))
                existingBook.AboutOfBook = updateBookDTO.AboutOfBook;

            if (!string.IsNullOrWhiteSpace(updateBookDTO.BookUrl))
                existingBook.BookUrl = updateBookDTO.BookUrl;

            if (updateBookDTO.BookImage != null && updateBookDTO.BookImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(updateBookDTO.BookImage.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Only JPG and PNG image files are allowed.");

                string fileName = $"{Guid.NewGuid()}{fileExtension}";
                string folderPath = Path.Combine("wwwroot", "images", "Books");
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderPath, fileName);

                Directory.CreateDirectory(folderPath);

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
                        updateBookDTO.BookImage.CopyTo(stream);
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
