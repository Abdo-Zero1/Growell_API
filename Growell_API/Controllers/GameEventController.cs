using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{SD.AdminRole},{SD.DoctorRole}")]

    public class GameEventController : ControllerBase
    {
        private readonly IGameEventRepository gameEventRepository;

        public GameEventController(IGameEventRepository gameEventRepository)
        {
            this.gameEventRepository = gameEventRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var Game = gameEventRepository.Get().ToList();
            return Ok(Game);
        }
        [HttpPost("CreateGameEvent")]
        public IActionResult Create([FromForm] GameDTO gameDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var gameEvent = new GameEvent
            {
                ChildId = gameDTO.GameEvent.ChildId,
                TestId = gameDTO.GameEvent.TestId,
                EventDateTime = gameDTO.GameEvent.EventDateTime,
                GameName = gameDTO.GameEvent.GameName,
                Level = gameDTO.GameEvent.Level,
                Score = gameDTO.GameEvent.Score,
                DevelopmentStatusID = gameDTO.GameEvent.DevelopmentStatusID
            };

            if (gameDTO.GameImage != null && gameDTO.GameImage.Length > 0)
            {
                var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(gameDTO.GameImage.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "images", "Game", imageFileName);

                Directory.CreateDirectory(Path.GetDirectoryName(imagePath)); 
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                     gameDTO.GameImage.CopyToAsync(stream);
                }

                gameEvent.GameImagePath = imageFileName;
            }

            if (gameDTO.GameFile != null && gameDTO.GameFile.Length > 0)
            {
                var fileFileName = Guid.NewGuid().ToString() + Path.GetExtension(gameDTO.GameFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", "files", fileFileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)); 
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    gameDTO.GameFile.CopyToAsync(stream);
                }

                gameEvent.GameFilePath = fileFileName;
            }

            gameEventRepository.Create(gameEvent);
            gameEventRepository.Commit();

            return Ok(gameEvent);
        }
        [HttpGet("{id}")]
        public IActionResult Get(int Id)
        {
            var game = gameEventRepository.GetOne(expression: g=>g.GameEventId == Id);
            if(game == null)
            {
                return NotFound("GameEvent not found");
            }
            return Ok(game);

        }

        [HttpPut("{id}")]
        public IActionResult Edit(int id, [FromForm] GameDTO gameDTO)
        {
            var gameEvent = gameEventRepository.GetOne(expression: g=>g.GameEventId == id);
            if (gameEvent == null)
            {
                return NotFound("Game event not found.");
            }

            gameEvent.GameName = gameDTO.GameEvent.GameName ?? gameEvent.GameName;
            gameEvent.Level = gameDTO.GameEvent.Level ?? gameEvent.Level;
            gameEvent.Score = gameDTO.GameEvent.Score ?? gameEvent.Score;
            gameEvent.DevelopmentStatusID = gameDTO.GameEvent.DevelopmentStatusID ?? gameEvent.DevelopmentStatusID;

            if (gameDTO.GameImage != null && gameDTO.GameImage.Length > 0)
            {
                if (!string.IsNullOrEmpty(gameEvent.GameImagePath))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "images", "Game", gameEvent.GameImagePath);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var newImageFileName = Guid.NewGuid().ToString() + Path.GetExtension(gameDTO.GameImage.FileName);
                var newImagePath = Path.Combine(Directory.GetCurrentDirectory(), "images", "Game", newImageFileName);

                Directory.CreateDirectory(Path.GetDirectoryName(newImagePath)); 
                using (var stream = new FileStream(newImagePath, FileMode.Create))
                {
                     gameDTO.GameImage.CopyToAsync(stream);
                }

                gameEvent.GameImagePath = newImageFileName;
            }

            if (gameDTO.GameFile != null && gameDTO.GameFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(gameEvent.GameFilePath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "images", "file", gameEvent.GameFilePath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                var newFileFileName = Guid.NewGuid().ToString() + Path.GetExtension(gameDTO.GameFile.FileName);
                var newFilePath = Path.Combine(Directory.GetCurrentDirectory(), "images", "file", newFileFileName);

                Directory.CreateDirectory(Path.GetDirectoryName(newFilePath)); 
                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    gameDTO.GameFile.CopyToAsync(stream);
                }

                gameEvent.GameFilePath = newFileFileName;
            }

            gameEventRepository.Edit(gameEvent);
            gameEventRepository.Commit();

            return Ok(gameEvent);
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var gameEvent = gameEventRepository.GetOne(expression: g=>g.GameEventId == id);
            if (gameEvent == null)
            {
                return NotFound("Game event not found.");
            }

            if (!string.IsNullOrEmpty(gameEvent.GameImagePath))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "images", "Game", gameEvent.GameImagePath);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            if (!string.IsNullOrEmpty(gameEvent.GameFilePath))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", "file", gameEvent.GameFilePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            gameEventRepository.Delete(gameEvent);
            gameEventRepository.Commit();
            return Ok("Game event deleted successfully.");
        }


    }
}
