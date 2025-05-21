using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorHomeController : ControllerBase
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly ITestRepository testRepository;

        public DoctorHomeController(IDoctorRepository doctorRepository, ITestRepository testRepository)
        {
            this.doctorRepository = doctorRepository;
            this.testRepository = testRepository;
        }

        [HttpGet]
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var topDoctors = doctorRepository.Get()
                .OrderByDescending(d => d.AveRating)
                .Select(d => new
                {
                    ID = d.DoctorID,
                    Image = d.ImgUrl,
                    FullName = $"{d.FirstName} {d.LastName}",
                    Bio = d.Bio,
                    Specialization = d.Specialization,
                    AveRating = d.AveRating,
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(topDoctors);
        }

        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var doctor = doctorRepository.Get(Include: [t => t.Tests])
                .Where(d => d.DoctorID == id)
                .Select(d => new
                {
                    id = d.DoctorID,
                    ImgUrl = d.ImgUrl,
                    FullName = $"{d.FirstName} {d.LastName}",
                    Email = d.Email,
                    Gender = d.Gender,
                    Bio = d.Bio,
                    AboutMe = d.AboutMe,
                    Description = d.Description,
                    AveRating = d.AveRating,
                    CreatedAt = d.CreatedAt,
                    PhoneNumber = d.PhoneNumber,
                    Specialization = d.Specialization,
                    YearsOfExperience = d.YearsOfExperience,
                    Education = d.Education,
                    Age = d.Age,
                    AboutOfKids = d.AboutOfKids,
                    TargetAgeGroup = d.TargetAgeGroup,
                    Tests = d.Tests.Select(t => new { t.TestID, t.TestName }),
                })
                .FirstOrDefault();

            if (doctor == null)
            {
                return NotFound(new { Message = "Doctor not found" });
            }

            return Ok(doctor);
        }

        [HttpGet("photo/{doctorId}")]
        public IActionResult GetPhotoUrl(int doctorId)
        {
            var doctor = doctorRepository.GetOne(expression:d=>d.DoctorID==doctorId);
            if (doctor == null)
            {
                return NotFound(new { Message = "Doctor not found" });
            }

            var imageUrl = string.IsNullOrEmpty(doctor.ImgUrl)
                ? $"{Request.Scheme}://{Request.Host}/images/default.jpg"
                : $"{Request.Scheme}://{Request.Host}/{doctor.ImgUrl}";

            return Ok(new { url = imageUrl });
        }

        [HttpGet("{doctorId}/tests")]
        public IActionResult GetTestsByDoctor(int doctorId)
        {
            var tests = testRepository.Get(
                expression: t => t.DoctorID == doctorId,
                Include: [t => t.Category, t => t.Doctor]
            ).Select(t => new
            {
                TestID = t.TestID,
                TestName = t.TestName,
                Description = t.Description,
                NumberOfQuestions = t.NumberOfQuestions,
                IsActive = t.IsActive,
                Category = t.CategoryID,
                CategoryName = t.Category.Name,
            }).ToList();

            if (!tests.Any())
            {
                return NotFound(new { Message = "No tests found for this doctor" });
            }

            return Ok(tests);
        }

        [HttpGet("search")]
        public IActionResult Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { Message = "Search term cannot be empty" });
            }

            searchTerm = searchTerm.ToLower();

            var doctors = doctorRepository.Get()
                .Where(d =>
                    (d.FirstName != null && d.FirstName.ToLower().Contains(searchTerm)) ||
                    (d.LastName != null && d.LastName.ToLower().Contains(searchTerm)))
                .Select(d => new
                {
                    ID = d.DoctorID,
                    ImgUrl = d.ImgUrl,
                    FullName = $"{d.FirstName} {d.LastName}",
                    Bio = d.Bio,
                    Specialization = d.Specialization,
                    Rating = d.AveRating
                })
                .OrderByDescending(d => d.Rating)
                .ToList();

            if (doctors.Count == 0)
            {
                return NotFound(new { Message = "No doctors found" });
            }

            return Ok(doctors);
        }
    }
}
