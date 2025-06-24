using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorHomeController : ControllerBase
    {
        private readonly IBookingRepository bookingRepository;
        private readonly IDoctorRepository doctorRepository;
        private readonly ITestRepository testRepository;
        private readonly ICategoryRepository categoryRepository;

        public DoctorHomeController(IBookingRepository bookingRepository,IDoctorRepository doctorRepository, ITestRepository testRepository, ICategoryRepository categoryRepository)
        {
            this.bookingRepository = bookingRepository;
            this.doctorRepository = doctorRepository;
            this.testRepository = testRepository;
            this.categoryRepository = categoryRepository;
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
            var doctor = doctorRepository.Get(
                    Include: new Expression<Func<Doctor, object>>[]
                    {
                d => d.Categories 
                    }
                )
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
                    Categories = d.Categories.Select(c => new
                    {
                        CategoryID = c.CategoryID,
                        Name = c.Name,
                        Description = c.Description,
                        Tests = testRepository.Get(
                            expression: t => t.CategoryID == c.CategoryID 
                        )
                        .Select(t => new
                        {
                            TestID = t.TestID,
                            TestName = t.TestName
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefault();

            if (doctor == null)
            {
                return NotFound(new { Message = "Doctor not found" });
            }

            return Ok(doctor);
        }

        [Authorize]
        [HttpPost("{doctorId}/book")]
        public async Task<IActionResult> BookDoctor(int doctorId, [FromBody] BookingDTO request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "User ID not found in token." });

                var userName = User.FindFirstValue(ClaimTypes.Name);
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name not found in token." });

                var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == doctorId);
                if (doctor == null)
                    return NotFound(new { Message = "Doctor not found." });

                var booking = new Booking
                {
                    UserID = userId,
                    CreatedByUserName = userName,
                    DoctorID = doctorId,
                    CreatedAt = DateTime.Now,
                    IsConfirmed = request.IsConfirmed,
                    Notes = request.Notes ?? string.Empty,
                    BookingDoctorName = $"{doctor.FirstName} {doctor.LastName}",
                    TastName = request.TestName,
                    Score = request.Score,
                    AppointmentDate = request.AppointmentDate != default ? request.AppointmentDate : DateTime.Now.AddDays(1) 
                };

                bookingRepository.Create(booking);
                bookingRepository.Commit();

                return Ok(new
                {
                    Message = "Booking successful",
                    BookingID = booking.BookingID,
                    DoctorName = booking.BookingDoctorName,
                    CreatedAt = booking.CreatedAt,
                    Notes = booking.Notes,
                    UserName = booking.CreatedByUserName,
                    TestName = booking.TastName,
                    Score = booking.Score,
                    AppointmentDate = booking.AppointmentDate,
                    IsConfirmed = booking.IsConfirmed ? "Confirmed" : "Pending"

                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while creating the booking.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
        }





        [HttpGet("GetTestsByCategory/{categoryId}")]
        public IActionResult GetTestsByCategory(int categoryId)
        {
            var category = categoryRepository.Get(
                expression: c => c.CategoryID == categoryId
            ).FirstOrDefault();

            if (category == null)
            {
                return NotFound(new { Message = "Category not found" });
            }

            var tests = testRepository.Get(
                expression: t => t.CategoryID == categoryId
            )
            .Select(t => new
            {
                TestID = t.TestID,
                TestName = t.TestName,
                Description = t.Description
            }).ToList();

            return Ok(new
            {
                CategoryID = category.CategoryID,
                CategoryName = category.Name,
                Tests = tests
            });
        }

        //[HttpGet("api/photo-url")]
        //public IActionResult GetPhotoUrl()

        //{

        //    var imageUrl = $"{Request.Scheme}://{Request.Host}/images/photo.jpg";

        //    return Ok(new { url = imageUrl });

        //}
        [Authorize]

        [HttpGet("photo/{doctorId}")]
        public IActionResult GetPhotoUrl(int doctorId)
        {
            var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == doctorId);
            if (doctor == null)
            {
                return NotFound(new { Message = "Doctor not found" });
            }

            var imageUrl = string.IsNullOrEmpty(doctor.ImgUrl)
                ? $"{Request.Scheme}://{Request.Host}/images/Photo.JPG"
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
