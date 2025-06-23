using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Linq.Expressions;



namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository bookingRepository;
        private readonly IDoctorRepository doctorRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITestResultRepository testResultRepository;
        private readonly IQuestionRepository questionRepository;
        private readonly ITestRepository testRepository;
        private readonly ILogger<BookingController> logger;

        public BookingController(IBookingRepository bookingRepository,
            IDoctorRepository doctorRepository,
            UserManager<ApplicationUser> userManager,
            ITestResultRepository testResultRepository, IQuestionRepository questionRepository,
            ITestRepository testRepository, ILogger<BookingController> logger )
        {
            this.bookingRepository = bookingRepository;
            this.doctorRepository = doctorRepository;
            this.userManager = userManager;
            this.testResultRepository = testResultRepository;
            this.questionRepository = questionRepository;
            this.testRepository = testRepository;
            this.logger = logger;
        }

        [HttpGet("GetAllBookings")]
        public IActionResult GetAllBookings()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { message = "Invalid token or email missing." });
            }

            var doctor = doctorRepository.GetOne(expression: d => d.Email == email);

            if (doctor == null)
            {
                return Unauthorized(new { message = "Doctor not found." });
            }

            var bookings = bookingRepository.Get(
                new Expression<Func<Booking, object>>[] { b => b.User, b => b.Doctor }, 
                b => b.DoctorID == doctor.DoctorID);

            if (!bookings.Any())
            {
                return NotFound(new { message = "No bookings found for the doctor." });
            }

            var result = bookings.Select(b => new
            {
                b.BookingID,
                ImageUser = b.User?.ProfilePicturePath,
                ImageDoctor = b.Doctor?.ImgUrl,
                DoctorName = $"{b.Doctor.FirstName} {b.Doctor.LastName}",
                aboutME = b.Doctor?.AboutMe ?? "No information available",
                PatientName = b.User?.UserName ?? "Unknown",
                AppointmentDate = b.AppointmentDate,
                Notes = b.Notes,
                IsConfirmed = b.IsConfirmed,
                TestName = b.TastName,
                Score = b.Score,

            });

            return Ok(result);
        }

        //[HttpPost("CreateBooking")]
        //public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        if (string.IsNullOrEmpty(userId))
        //            return Unauthorized(new { message = "Authorization error. User ID not found in token." });

        //        var currentUser = await userManager.FindByIdAsync(userId);
        //        if (currentUser == null)
        //            return BadRequest(new { message = "User ID does not exist in the database." });

        //        var bookingDoctor = doctorRepository.Get(expression: d => d.DoctorID == booking.DoctorID).FirstOrDefault();
        //        if (bookingDoctor == null)
        //            return NotFound(new { message = "Doctor not found for this booking." });

        //        booking.UserID = userId; 
        //        booking.CreatedAt = DateTime.Now;
        //        booking.IsConfirmed = false;
        //        booking.BookingDoctorName = $"{bookingDoctor.FirstName} {bookingDoctor.LastName}";
        //        booking.CreatedByUserName = currentUser.UserName;
        //        booking.Notes = booking.Notes ?? string.Empty; 

        //        bookingRepository.Create(booking);
        //        bookingRepository.Commit();

        //        var response = new DTOs.BookingDTO
        //        {
        //            BookingID = booking.BookingID,
        //            UserID = booking.UserID,
        //            CreatedByUserName = booking.CreatedByUserName,
        //            TestDoctorName = booking.TestDoctorName,
        //            BookingDoctorName = booking.BookingDoctorName,
        //            AppointmentDate = booking.AppointmentDate,
        //            IsConfirmed = booking.IsConfirmed,
        //            Notes = booking.Notes
        //        };

        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            message = "An error occurred while creating the booking.",
        //            details = ex.InnerException?.Message ?? ex.Message
        //        });
        //    }
        //}


    }
}
