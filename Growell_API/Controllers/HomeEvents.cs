using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeEvents : ControllerBase
    {
       
        private readonly IBookEventRepository bookEventRepository;
        private readonly IVideoEventRepository videoEventRepository;

        public HomeEvents (IBookEventRepository bookEventRepository, IVideoEventRepository videoEventRepository)
        {
            this.bookEventRepository = bookEventRepository;
            this.videoEventRepository = videoEventRepository;
        }

        [HttpGet("video/photo/{videoId}")]
        public IActionResult GetVideoPhotoUrl(int videoId)
        {
            var video = videoEventRepository.GetOne(expression: v => v.VideoEventId == videoId);
            if (video == null)
            {
                return NotFound(new { Message = "Video not found" });
            }

            if (string.IsNullOrEmpty(video.VideoImagePath))
            {
                return NotFound(new { Message = "Video image not found" });
            }

            var imageUrl = $"{Request.Scheme}://{Request.Host}/{video.VideoImagePath}";

            return Ok(new { url = imageUrl });
        }


        [HttpGet("photo/{bookId}")]
        public IActionResult GetPhotoUrl(int bookId)
        {
            var book = bookEventRepository.GetOne(expression: b => b.BookEventId == bookId);
            if (book == null)
            {
                return NotFound(new { Message = "Book not found" });
            }

             var imageUrl = $"{Request.Scheme}://{Request.Host}/{book.BookImagePath}";


            return Ok(new { url = imageUrl });
        }




        [HttpGet("GetBooks")]
        public IActionResult GetBooks(int pageNum = 1, int pageSize = 10)
        {
            try
            {
                var query = bookEventRepository.Get();
                var totalRecords = query.Count();
                var books = query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();

                var responseData = books.Select(book => new
                {
                    book.BookEventId,
                    book.BookTitle,
                    book.Description,
                    book.AboutOfBook,
                    book.BookUrl,
                    BookImagePath = string.IsNullOrEmpty(book.BookImagePath)
                        ? null
                        : $"{Request.Scheme}://{Request.Host}/api/HomeEvents/GetImage/{book.BookImagePath}"
                }).ToList();

                var response = new
                {
                    TotalCount = totalRecords,
                    PageNumber = pageNum,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                    Data = responseData
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error.");
            }
        }
        [HttpGet("GetbookID")]
        public IActionResult GetId(int bookId)
        {
            var book = bookEventRepository.GetOne(expression: b => b.BookEventId == bookId);
            if (book == null)
            {
                return NotFound(new { Message = "Book not found" });
            }
           
            return Ok(book);
        }

        [HttpGet("GetVidoes")]
        public IActionResult GetVidoes(int pageNum = 1, int pageSize = 10)
        {
            try
            {
                var query = videoEventRepository.Get();
                var totalRecords = query.Count();
                var videos = query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();

                var responseData = videos.Select(video => new
                {
                    video.VideoEventId,
                    video.VideoTitle,
                    video.Description,
                    video.AboutOfVideo,
                    video.VideoUrl,
                    VideoImagePath = string.IsNullOrEmpty(video.VideoImagePath)
                        ? null
                        : $"{Request.Scheme}://{Request.Host}/api/HomeEvents/GetImage/{video.VideoImagePath}"
                }).ToList();

                var response = new
                {
                    TotalCount = totalRecords,
                    PageNumber = pageNum,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                    Data = responseData
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error.");
            }
        }
        [HttpGet("GetVideoID")]
        public IActionResult GetVideoId(int videoId)
        {
            var video = videoEventRepository.GetOne(expression: v => v.VideoEventId == videoId);
            if (video == null)
            {
                return NotFound(new { Message = "Video not found" });
            }
            return Ok(video);
        }



    }
}
