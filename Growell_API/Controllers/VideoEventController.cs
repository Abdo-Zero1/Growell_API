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
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "images", "videos");
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
            var response = new
            {
                videoEvent.VideoEventId,
                videoEvent.VideoTitle,
                videoEvent.Description,
                videoEvent.AboutOfVideo,
                videoEvent.VideoUrl,
                videoEvent.VideoImagePath
            };
            return Ok(response);

        }
        [HttpPut]
        [Route("EditVideoEvent/{id}")]
        public IActionResult Edit(int id, [FromForm] UpdateVideoDTO videoDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingVideo = videoEventRepository.GetOne(expression: b => b.VideoEventId == id);
            if (existingVideo == null)
                return NotFound("The video with the specified ID does not exist.");

            if (!string.IsNullOrWhiteSpace(videoDTO.VideoTitle))
                existingVideo.VideoTitle = videoDTO.VideoTitle;

            if (!string.IsNullOrWhiteSpace(videoDTO.Description))
                existingVideo.Description = videoDTO.Description;

            if (!string.IsNullOrWhiteSpace(videoDTO.AboutOfVideo))
                existingVideo.AboutOfVideo = videoDTO.AboutOfVideo;

            if (!string.IsNullOrWhiteSpace(videoDTO.VideoUrl))
                existingVideo.VideoUrl = videoDTO.VideoUrl;

            if (videoDTO.VideoImage != null && videoDTO.VideoImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(videoDTO.VideoImage.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Only JPG and PNG image files are allowed.");

                string fileName = $"{Guid.NewGuid()}{fileExtension}";
                string folderPath = Path.Combine("wwwroot", "images", "Videos");
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderPath, fileName);

                Directory.CreateDirectory(folderPath);

                try
                {
                    if (!string.IsNullOrEmpty(existingVideo.VideoImagePath))
                    {
                        string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingVideo.VideoImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        videoDTO.VideoImage.CopyTo(stream);
                    }

                    existingVideo.VideoImagePath = $"/images/Videos/{fileName}";
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            videoEventRepository.Edit(existingVideo);
            videoEventRepository.Commit();

            return Ok(existingVideo);
        }



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
