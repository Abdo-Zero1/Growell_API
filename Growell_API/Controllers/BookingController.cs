using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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

        public BookingController(IBookingRepository bookingRepository,
            IDoctorRepository doctorRepository,
            UserManager<ApplicationUser> userManager,
            ITestResultRepository testResultRepository, IQuestionRepository questionRepository, ITestRepository testRepository )
        {
            this.bookingRepository = bookingRepository;
            this.doctorRepository = doctorRepository;
            this.userManager = userManager;
            this.testResultRepository = testResultRepository;
            this.questionRepository = questionRepository;
            this.testRepository = testRepository;
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

            var bookings = bookingRepository.Get(expression: b => b.DoctorID == doctor.DoctorID);

            if (!bookings.Any())
            {
                return NotFound(new { message = "No bookings found for the doctor." });
            }

            var result = bookings.Select(b => new
            {
                b.BookingID,
                DoctorName = $"{b.Doctor.FirstName} {b.Doctor.LastName}",
                PatientName = b.User?.UserName,
                AppointmentDate = b.AppointmentDate,
                Notes = b.Notes,
                IsConfirmed = b.IsConfirmed
            });

            return Ok(result);
        }


        [HttpPost("CreateBooking")]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookingDoctor = doctorRepository.Get(expression: d => d.DoctorID == booking.DoctorID).FirstOrDefault();
            if (bookingDoctor == null)
                return NotFound(new { message = "Doctor not found for this booking." });

            var currentUser = await userManager.FindByIdAsync(booking.UserID);
            if (currentUser == null)
                return NotFound(new { message = "User not found." });

            var testResult = booking.TestResultID.HasValue
                ? testResultRepository.Get(expression: r => r.TestResultID == booking.TestResultID).FirstOrDefault()
                : null;

            string testDoctorName = "Not available";
            string testName = "Not available";
            int? score = null;
            double? percentage = null;

            if (testResult != null)
            {
                if (testResult.DoctorID.HasValue)
                {
                    var testDoctor = doctorRepository.Get(expression: d => d.DoctorID == testResult.DoctorID).FirstOrDefault();
                    if (testDoctor != null)
                    {
                        testDoctorName = $"{testDoctor.FirstName} {testDoctor.LastName}";
                    }
                }

                // تفاصيل الاختبار
                var test = testRepository.GetOne(expression: t => t.TestID == testResult.TestID);
                if (test != null)
                {
                    testName = test.TestName;

                    var totalQuestions = questionRepository.Get(expression: q => q.TestID == test.TestID).Count();
                    percentage = totalQuestions > 0 ? (double)testResult.Score / totalQuestions * 100 : 0;
                    score = testResult.Score;
                }
            }

            // إعداد معلومات الحجز
            booking.CreatedAt = DateTime.Now;
            booking.IsConfirmed = false;
            booking.BookingDoctorName = $"{bookingDoctor.FirstName} {bookingDoctor.LastName}";
            booking.TestDoctorName = testDoctorName;
            booking.CreatedByUserName = currentUser.UserName;

            // تخزين الحجز
            bookingRepository.Create(booking);
            bookingRepository.Commit();

            // إرجاع الرد
            return Ok(new
            {
                message = "Booking created successfully",
                bookingId = booking.BookingID,
                bookingDoctorName = booking.BookingDoctorName,
                testDoctorName = booking.TestDoctorName,
                createdByUserName = booking.CreatedByUserName,
                appointmentDate = booking.AppointmentDate,
                isConfirmed = booking.IsConfirmed,
                testResult = new
                {
                    testName = testName,
                    score = score,
                    percentage = percentage,
                    classificationEn = percentage.HasValue ? GetDelayClassificationEn(percentage.Value) : null,
                    classificationAr = percentage.HasValue ? GetDelayClassificationAr(percentage.Value) : null
                }
            });
        }



        private string GetDelayClassificationEn(double percentage)
        {
            if (percentage == 100)
                return "No Delay";
            else if (percentage >= 70)
                return "Mild Delay";
            else if (percentage >= 50)
                return "Mild to moderate delay detected. It is advisable to consult a doctor to prevent worsening";
            else
                return "Severe delay detected. It is highly recommended to contact a doctor immediately";
        }

        private string GetDelayClassificationAr(double percentage)
        {
            if (percentage == 100)
                return "لا يوجد تأخر";
            else if (percentage >= 70)
                return "تأخر خفيف";
            else if (percentage >= 50)
                return "تم الكشف عن تأخر خفيف إلى متوسط. من الأفضل استشارة طبيب لتجنب تدهور الحالة";
            else
                return "تم الكشف عن تأخر شديد. يُنصح بشدة بالتواصل مع طبيب فوراً";
        }









    }
}
