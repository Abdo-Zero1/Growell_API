using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using DataAccess.Repository;
using System.Linq.Expressions;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountDoctorController : ControllerBase
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IConfiguration _configuration;
        private readonly ITestRepository testRepository;
        private readonly ITestResultRepository testResultRepository;
        private readonly IQuestionRepository questionRepository;
        private readonly ISessionRepository sessionRepository;

        public AccountDoctorController(IDoctorRepository doctorRepository,
            IConfiguration configuration, ITestRepository testRepository,
            ITestResultRepository testResultRepository,
            IQuestionRepository questionRepository, ISessionRepository sessionRepository)
        {
           this.doctorRepository = doctorRepository;
            _configuration = configuration;
            this.testRepository = testRepository;
            this.testResultRepository = testResultRepository;
            this.questionRepository = questionRepository;
            this.sessionRepository = sessionRepository;
            this.doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterDoctor registerDto)
        {
            if (registerDto == null)
                return BadRequest("Invalid data.");

            if (registerDto.Password != registerDto.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var existingDoctor = doctorRepository.GetDoctorByEmail(registerDto.Email);
            if (existingDoctor != null)
                return BadRequest("Doctor with this email already exists.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            string imgUrl;
            if (registerDto.ImgUrl != null && registerDto.ImgUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Doctor");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(registerDto.ImgUrl.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await registerDto.ImgUrl.CopyToAsync(fileStream);
                }

                imgUrl = "/images/Doctor/" + uniqueFileName; 
            }
            else
            {
                imgUrl = "/images/Photo.JPG";
            }

            var doctor = new Doctor
            {
                FirstName = registerDto.FirstName,
                SecondName = registerDto.SecondName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                Address = registerDto.Address,
                Gender = registerDto.Gender,
                Bio = registerDto.Bio,
                AboutMe = registerDto.AboutMe,
                Description = registerDto.Description,
                PhoneNumber = registerDto.PhoneNumber,
                AboutOfKids = registerDto.AboutOfKids,
                TargetAgeGroup = registerDto.TargetAgeGroup,
                Specialization = registerDto.Specialization,
                YearsOfExperience = registerDto.YearsOfExperience,
                AveRating = registerDto.AveRating,
                CreatedAt = DateTime.UtcNow,
                Age = registerDto.Age,
                Education = registerDto.Education,
                ImgUrl = imgUrl,
                passwordHash = passwordHash
            };

            doctorRepository.Create(doctor);

            doctorRepository.Commit();

            var token = GenerateJwtToken(doctor);

            return Ok(new
            {
                message = "Doctor registered successfully",
                token = token
            });
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDoctor loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return BadRequest("Email and password are required.");

            var doctor = doctorRepository.GetDoctorByEmail(loginDto.Email);

            if (doctor == null || string.IsNullOrEmpty(doctor.passwordHash) || !BCrypt.Net.BCrypt.Verify(loginDto.Password, doctor.passwordHash))
                return Unauthorized("Invalid email or password.");

            var token = GenerateJwtToken(doctor);
            return Ok(new
            {
                message = "Doctor Login successfully",
                Token = token
            });
        }
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in.");

            var sessions = sessionRepository.Get(expression: s => s.SessionId == int.Parse(userId));
            foreach (var session in sessions)
            {
                sessionRepository.Delete(session);
            }

            sessionRepository.Commit();

            return Ok(new { Message = "Logged out successfully." });
        }



        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == int.Parse(userId));

            if (doctor == null)
                return NotFound("Doctor not found.");

            return Ok(new
            {
                doctor.FirstName,
                doctor.SecondName,
                doctor.LastName,
                doctor.Email,
                doctor.PhoneNumber,
                doctor.Specialization,
                doctor.YearsOfExperience,
                doctor.Education,
                doctor.Age,
                doctor.Bio,
                doctor.AboutMe,
                doctor.Description,
                doctor.ImgUrl,
                doctor.Address
            });
        }

       [Authorize]
[HttpPut("update-profile")]
public async Task<IActionResult> UpdateProfile([FromForm] DoctorEditDTO updateProfileDto)
{
    try
    {
        // الحصول على معرف المستخدم
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { error = "Authorization error", message = "User ID is missing or invalid." });

        if (!int.TryParse(userId, out var doctorId))
            return BadRequest(new { error = "Validation error", message = "Invalid user ID format." });

        // جلب الطبيب
        var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == doctorId);
        if (doctor == null)
            return NotFound(new { error = "Not Found", message = "Doctor not found." });

        // تحديث الحقول
        if (!string.IsNullOrEmpty(updateProfileDto.FirstName)) doctor.FirstName = updateProfileDto.FirstName;
        if (!string.IsNullOrEmpty(updateProfileDto.SecondName)) doctor.SecondName = updateProfileDto.SecondName;
        if (!string.IsNullOrEmpty(updateProfileDto.LatestName)) doctor.LastName = updateProfileDto.LatestName;
        if (!string.IsNullOrEmpty(updateProfileDto.PhoneNumber)) doctor.PhoneNumber = updateProfileDto.PhoneNumber;
        if (!string.IsNullOrEmpty(updateProfileDto.Specialization)) doctor.Specialization = updateProfileDto.Specialization;
        if (!string.IsNullOrEmpty(updateProfileDto.Education)) doctor.Education = updateProfileDto.Education;
        if (!string.IsNullOrEmpty(updateProfileDto.Bio)) doctor.Bio = updateProfileDto.Bio;
        if (!string.IsNullOrEmpty(updateProfileDto.Address)) doctor.Address = updateProfileDto.Address;
        if (updateProfileDto.Age.HasValue) doctor.Age = updateProfileDto.Age.Value;

        // معالجة الصورة
        if (updateProfileDto.ImgUrl != null && updateProfileDto.ImgUrl.Length > 0)
        {
            try
            {
                if (!string.IsNullOrEmpty(doctor.ImgUrl))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doctor.ImgUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                doctor.ImgUrl = await SaveDoctorImage(updateProfileDto.ImgUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Image Upload Error", message = ex.Message });
            }
        }

        // حفظ التعديلات
        doctorRepository.Edit(doctor);
        doctorRepository.Commit();

        return Ok(new
        {
            message = "Profile updated successfully",
            doctorData = new
            {
                doctor.FirstName,
                doctor.SecondName,
                doctor.LastName,
                doctor.PhoneNumber,
                doctor.Specialization,
                doctor.Education,
                doctor.Bio,
                doctor.Address,
                doctor.Age,
                doctor.ImgUrl
            }
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = "Internal Server Error", message = ex.Message });
    }
}

private async Task<string> SaveDoctorImage(IFormFile imgFile)
{
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
    var extension = Path.GetExtension(imgFile.FileName).ToLower();

    if (!allowedExtensions.Contains(extension))
        throw new Exception("Invalid file type. Only .jpg, .jpeg, .png, and .gif are allowed.");

    if (imgFile.Length > 5 * 1024 * 1024)
        throw new Exception("File size exceeds 5MB.");

    var fileName = $"{Guid.NewGuid()}{extension}";
    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Doctor");

    Directory.CreateDirectory(folderPath);

    var filePath = Path.Combine(folderPath, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await imgFile.CopyToAsync(stream);
    }

    return $"/images/Doctor/{fileName}";
}




        [Authorize]
        [HttpPut("change-password")]
        public IActionResult ChangePassword([FromBody] UpdatePasswordDto updatePasswordDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctorId = int.Parse(userId);

            var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == doctorId);

            if (doctor == null)
                return NotFound("Doctor not found.");

            if (!BCrypt.Net.BCrypt.Verify(updatePasswordDto.OldPassword, doctor.passwordHash))
                return BadRequest("Current password is incorrect.");

            doctor.passwordHash = BCrypt.Net.BCrypt.HashPassword(updatePasswordDto.NewPassword);
            doctorRepository.Edit(doctor);
            doctorRepository.Commit();

            return Ok("Password changed successfully.");
        }


        [Authorize]
        [HttpDelete("delete-account")]
        public IActionResult DeleteAccount()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token.");

                if (!int.TryParse(userId, out var doctorId))
                    return BadRequest("Invalid User ID.");

                var doctor = doctorRepository.GetOne(
                    includeProps: new Expression<Func<Doctor, object>>[]
                    {
                d => d.Tests,
                d => d.TestResults,
                d => d.Questions
                    },
                    expression: d => d.DoctorID == doctorId
                );

                if (doctor == null)
                    return NotFound("Doctor not found.");

                var tests = doctor.Tests?.ToList() ?? new List<Test>();
                foreach (var test in tests)
                {
                    testRepository.Delete(test);
                }
                var testResults = doctor.TestResults?.ToList() ?? new List<TestResult>();
                foreach (var tr in testResults)
                {
                    testResultRepository.Delete(tr);
                }

                var questions = doctor.Questions?.ToList() ?? new List<Question>();
                foreach (var q in questions)
                {
                    questionRepository.Delete(q);
                }

                doctorRepository.Delete(doctor);

                doctorRepository.Commit();

                return Ok("Account deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the account.");
            }
        }



        private string GenerateJwtToken(Doctor doctor)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, doctor.DoctorID.ToString()),
        new Claim(ClaimTypes.Email, doctor.Email),
        new Claim(ClaimTypes.Name, $"{doctor.FirstName} {doctor.LastName}"),
        new Claim(ClaimTypes.Role, "Doctor") 
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiryInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
