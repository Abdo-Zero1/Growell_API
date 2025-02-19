using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{SD.AdminRole},{SD.DoctorRole}")]

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
        public IActionResult Create(BookDTO bookDTO)
        {
            if(ModelState.IsValid)
            {
                if(bookDTO.ImgUrl != null && bookDTO.ImgUrl.Length>0)
                {
                    var fileName = Guid.NewGuid().ToString()+Path.GetExtension(bookDTO.ImgUrl.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        bookDTO.ImgUrl.CopyTo(stream);
                    }
                    bookDTO.BookEvent.BookImagePath = fileName;
                }
                bookEventRepository.Create(bookDTO.BookEvent);
                bookEventRepository.Commit();
                return Ok(bookDTO.BookEvent);
            }
            return BadRequest("There was an error with the provided data");
        }
        [HttpGet("{id}")]
        public IActionResult Get(int Id)
        {
            var bookEvent = bookEventRepository.GetOne(expression: e=>e.BookEventId == Id);
            if (bookEvent == null)
            {
                return NotFound("BookEvent not found");
            }
            return Ok(bookEvent);

        }
        [HttpPut("{id}")]
        public IActionResult Put(int Id, [FromBody] BookDTO bookDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingBook = bookEventRepository.GetOne(expression:e=>e.BookEventId == Id);
            if (existingBook == null)
            {
                return NotFound("BookEvent not found");
            }
            existingBook.BookTitle = bookDTO.BookEvent.BookTitle;
            existingBook.TestId = bookDTO.BookEvent.TestId;
            existingBook.ChildId = bookDTO.BookEvent.ChildId;
            existingBook.DevelopmentStatusID = bookDTO.BookEvent.DevelopmentStatusID;

            if (bookDTO.ImgUrl != null && bookDTO.ImgUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bookDTO.ImgUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    bookDTO.ImgUrl.CopyTo(stream);
                }

                existingBook.BookImagePath = fileName;
            }

            bookEventRepository.Edit(existingBook);
            bookEventRepository.Commit();

            return Ok(existingBook); 
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int Id)
        {
            var bookEvent = bookEventRepository.GetOne(expression: e=>e.BookEventId== Id);
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
            }

            bookEventRepository.Delete(bookEvent);
            bookEventRepository.Commit();

            return NoContent(); 
        }



    }
}
