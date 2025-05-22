using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = $"{SD.DoctorRole}")]
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
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (videoDTO.VideoImage == null || videoDTO.VideoImage.Length == 0)
                return BadRequest("Video data and image are required.");

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(videoDTO.VideoImage.FileName);
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "images", "videos");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, fileName);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                videoDTO.VideoImage.CopyTo(stream);
            }


            var video = new VideoEvent
            {
                VideoTitle = videoDTO.VideoTitle,
                Description = videoDTO.Description,
                AboutOfVideo = videoDTO.AboutOfVideo,
                VideoUrl = videoDTO.VideoUrl,
                VideoImagePath = $"/images/videos/{fileName}"
            };

           
            videoEventRepository.Create(video);
            videoEventRepository.Commit();

            return Ok(video);
        }




        [HttpGet("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var videoEvent = videoEventRepository.GetOne(expression: e => e.VideoEventId == id);
            if (videoEvent == null)
            {
                return NotFound("Video Event not found");
            }
            return Ok(videoEvent);

        }
        //[HttpPut("{id}")]
        //public IActionResult Put(int id, [FromForm] VideoDTO videoDTO)
        //{
        //    if (id != videoDTO.VideoEvent.VideoEventId)
        //    {
        //        return BadRequest("Invalid Video Event ID.");
        //    }

        //    var videoEvent = videoEventRepository.GetOne(expression: e => e.VideoEventId == id);
        //    if (videoEvent == null)
        //    {
        //        return NotFound("Video Event not found.");
        //    }

        //    //videoEvent.TestResult = videoDTO.VideoEvent.TestResult;
        //    videoEvent.VideoTitle = videoDTO.VideoEvent.VideoTitle;
        //    videoEvent.Description = videoDTO.VideoEvent.Description;

        //    if (videoDTO.VideoImage != null && videoDTO.VideoImage.Length > 0)
        //    {
        //        if (!string.IsNullOrEmpty(videoEvent.VideoImagePath))
        //        {
        //            var oldVideoPath = Path.Combine(Directory.GetCurrentDirectory(), "images","Video", videoEvent.VideoImagePath);
        //            if (System.IO.File.Exists(oldVideoPath))
        //            {
        //                System.IO.File.Delete(oldVideoPath);
        //            }
        //        }

        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(videoDTO.VideoImage.FileName);
        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images","Video", fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //             videoDTO.VideoImage.CopyToAsync(stream);
        //        }

        //        videoEvent.VideoImagePath = fileName;
        //    }

        //       videoEventRepository.Edit(videoEvent);
        //       videoEventRepository.Commit();

        //    return Ok(videoEvent);
        //}

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var videoEvent = videoEventRepository.GetOne(expression: e => e.VideoEventId == id);
            if (videoEvent == null)
            {
                return NotFound("BookEvent not found.");
            }

            if (!string.IsNullOrEmpty(videoEvent.VideoImagePath))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", "videos", videoEvent.VideoImagePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }


                videoEventRepository.Delete(videoEvent);
                videoEventRepository.Commit();
                return Ok("The deleted was success");
            }

        

            return NoContent();
        }


    }
}
