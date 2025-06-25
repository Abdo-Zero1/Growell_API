using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;



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
                return Ok(new
                {
                    message = "No bookings found for the current doctor.",
                    Doctor = new
                    {
                        doctor.DoctorID,
                        doctor.FirstName,
                        doctor.LastName,
                        doctor.Bio,
                        doctor.ImgUrl
                    }
                });

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
                gender = b.User.Gender

            });

            return Ok(result);
        }
        [Authorize]
        [HttpDelete("DeleteBooking/{id}")]
        public IActionResult delete(int id)
        {
            try
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

                var booking = bookingRepository.GetOne(expression: b => b.BookingID == id);
                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found." });
                }
                if(doctor.DoctorID != booking.DoctorID )
                {
                    return Unauthorized("You can only delete your own bookings.");
                }
                bookingRepository.Delete(booking);
                bookingRepository.Commit();
                return Ok(new { message = "Booking deleted successfully." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting booking with ID {BookingId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the booking.", details = ex.Message });
            }
        }

        
    }
}
