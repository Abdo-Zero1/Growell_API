using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = $"{SD.AdminRole},{SD.DoctorRole}")]
    public class VideoEventController : ControllerBase
    {
        private readonly IVideoEventRepository videoEventRepository;

        public VideoEventController(IVideoEventRepository videoEventRepository)
        {
            this.videoEventRepository = videoEventRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var video = videoEventRepository.Get().ToList();
            return Ok(video);
        }
        [HttpPost("CreateVideoEvent")]
        public IActionResult CreateVideoEvent([FromForm] VideoDTO videoDTO)
        {
            if (ModelState.IsValid)
            {
                if (videoDTO.VideoFile != null && videoDTO.VideoFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(videoDTO.VideoFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images","videos", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        videoDTO.VideoFile.CopyTo(stream);
                    }

                    videoDTO.VideoEvent.VideoFilePath = fileName;
                }

                videoEventRepository.Create(videoDTO.VideoEvent);
                videoEventRepository.Commit();
                return Ok(videoDTO.VideoEvent);
            }

            return BadRequest(ModelState);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int Id)
        {
            var videoEvent = videoEventRepository.GetOne(expression: e => e.VideoEventId == Id);
            if(videoEvent == null)
            {
                return NotFound("Video Event not found");
            }
            return Ok(videoEvent);

        }
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] VideoDTO videoDTO)
        {
            if (id != videoDTO.VideoEvent.VideoEventId)
            {
                return BadRequest("Invalid Video Event ID.");
            }

            var videoEvent = videoEventRepository.GetOne(expression: e => e.VideoEventId == id);
            if (videoEvent == null)
            {
                return NotFound("Video Event not found.");
            }

            videoEvent.TestResult = videoDTO.VideoEvent.TestResult;
            videoEvent.EventDateTime = videoDTO.VideoEvent.EventDateTime;
            videoEvent.VideoTitle = videoDTO.VideoEvent.VideoTitle;
            videoEvent.Topic = videoDTO.VideoEvent.Topic;

            if (videoDTO.VideoFile != null && videoDTO.VideoFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(videoEvent.VideoFilePath))
                {
                    var oldVideoPath = Path.Combine(Directory.GetCurrentDirectory(), "images","Video", videoEvent.VideoFilePath);
                    if (System.IO.File.Exists(oldVideoPath))
                    {
                        System.IO.File.Delete(oldVideoPath);
                    }
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(videoDTO.VideoFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images","Video", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                     videoDTO.VideoFile.CopyToAsync(stream);
                }

                videoEvent.VideoFilePath = fileName;
            }

               videoEventRepository.Edit(videoEvent);
               videoEventRepository.Commit();

            return Ok(videoEvent);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int Id)
        {
            var bookEvent = videoEventRepository.GetOne(expression: e => e.VideoEventId == Id);
            if (bookEvent == null)
            {
                return NotFound("BookEvent not found.");
            }

            if (!string.IsNullOrEmpty(bookEvent.VideoFilePath))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", bookEvent.VideoFilePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            videoEventRepository.Delete(bookEvent);
            videoEventRepository.Commit();

            return NoContent();
        }


    }
}
