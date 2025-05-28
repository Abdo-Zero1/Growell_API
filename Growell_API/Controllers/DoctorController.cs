//using DataAccess.Repository;
//using DataAccess.Repository.IRepository;
//using Growell_API.DTOs;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Models;
//using System.IO;
//using System;
//using System.Linq;

//namespace Growell_API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class DoctorController : ControllerBase
//    {
//        private readonly IDoctorRepository doctorRepository;
//        private readonly ITestRepository testRepository;
//        private readonly ITestResultRepository testResultRepository;

//        public DoctorController(IDoctorRepository doctorRepository,ITestRepository testRepository, ITestResultRepository testResultRepository)
//        {
//            this.doctorRepository = doctorRepository;
//            this.testRepository = testRepository;
//            this.testResultRepository = testResultRepository;
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpGet("Get")]
//        public IActionResult Index(int page = 1, int pageSize = 10)
//        {
//            var query = doctorRepository.Get(Include: [t => t.Tests]);

//            int totalCount = query.Count();
//            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

//            var doctors = query
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .ToList();

//            return Ok(new
//            {
//                currentPage = page,
//                pageSize = pageSize,
//                totalPages = totalPages,
//                totalDoctors = totalCount,
//                data = doctors
//            });
//        }

//        [HttpPost]
//        // [Authorize(Roles = "Admin")]
//        public IActionResult CreateDoctor([FromForm] DoctorDTO doctorDTO)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            string imgFileName = "/wwwroot/images/Photo.JPG"; 

//            if (doctorDTO.ImgUrl != null && doctorDTO.ImgUrl.Length > 0)
//            {
//                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Doctor");

//                if (!Directory.Exists(uploadsFolder))
//                    Directory.CreateDirectory(uploadsFolder);

//                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(doctorDTO.ImgUrl.FileName);
//                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

//                using (var stream = new FileStream(filePath, FileMode.Create))
//                {
//                    doctorDTO.ImgUrl.CopyTo(stream);
//                }

//                imgFileName = $"/images/Doctor/{uniqueFileName}";
//            }

//            var doctor = new Doctor
//            {
//                UserID = doctorDTO.UserID,
//                FirstName = doctorDTO.FirstName,
//                SecondName = doctorDTO.SecondName,
//                LastName = doctorDTO.LastName,
//                Email = doctorDTO.Email,
//                Gender = doctorDTO.Gender,
//                Bio = doctorDTO.Bio,
//                AboutMe = doctorDTO.AboutMe,
//                Description = doctorDTO.Description,
//                PhoneNumber = doctorDTO.PhoneNumber,
//                Specialization = doctorDTO.Specialization,
//                YearsOfExperience = doctorDTO.YearsOfExperience,
//                Education = doctorDTO.Education,
//                Age = doctorDTO.Age,
//                ImgUrl = imgFileName, 
//                AboutOfKids = doctorDTO.AboutOfKids,
//                TargetAgeGroup = doctorDTO.TargetAgeGroup,
//                CreatedAt = DateTime.UtcNow
//            };

//            doctorRepository.Create(doctor);
//            doctorRepository.Commit();

//            return Ok(new { message = "Doctor created successfully", doctorId = doctor.DoctorID });
//        }


//        [Authorize(Roles = "Doctor")]
//        [HttpPut("{id}")]
//        public IActionResult EditDoctor(int id, [FromForm] DoctorEditDTO dto)
//        {
//            var existingDoctor = doctorRepository.GetOne(expression: d => d.DoctorID == id);
//            if (existingDoctor == null)
//                return NotFound(new { message = "Doctor not found" });

//            if(!string.IsNullOrEmpty(dto.FirstName)) existingDoctor.FirstName = dto.FirstName;
//            if(!string.IsNullOrEmpty(dto.SecondName)) existingDoctor.SecondName = dto.SecondName;
//            if (!string.IsNullOrEmpty(dto.LatestName)) existingDoctor.LastName = dto.LatestName;
//            if (!string.IsNullOrEmpty(dto.Description)) existingDoctor.Description = dto.Description;
//            if (!string.IsNullOrEmpty(dto.Education)) existingDoctor.Education = dto.Education;
//            if (dto.Age.HasValue) existingDoctor.Age = dto.Age.Value;
//            if (!string.IsNullOrEmpty(dto.Specialization)) existingDoctor.Specialization = dto.Specialization;
//            if (dto.YearsOfExperience.HasValue) existingDoctor.YearsOfExperience = dto.YearsOfExperience.Value;
//            if (!string.IsNullOrEmpty(dto.AboutMe)) existingDoctor.AboutMe = dto.AboutMe;
//            if (!string.IsNullOrEmpty(dto.AboutOfKids)) existingDoctor.AboutOfKids = dto.AboutOfKids;
//            if (!string.IsNullOrEmpty(dto.TargetAgeGroup)) existingDoctor.TargetAgeGroup = dto.TargetAgeGroup;
//            if (!string.IsNullOrEmpty(dto.Bio)) existingDoctor.Bio = dto.Bio;
//            if (!string.IsNullOrEmpty(dto.PhoneNumber)) existingDoctor.PhoneNumber = dto.PhoneNumber;

//            if (dto.ImgUrl != null && dto.ImgUrl.Length > 0)
//            {
//                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Doctor");

//                if (!Directory.Exists(folderPath))
//                    Directory.CreateDirectory(folderPath);

//                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImgUrl.FileName);
//                var fullPath = Path.Combine(folderPath, newFileName);

//                using (var stream = new FileStream(fullPath, FileMode.Create))
//                {
//                    dto.ImgUrl.CopyTo(stream);
//                }

//                if (!string.IsNullOrEmpty(existingDoctor.ImgUrl))
//                {
//                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingDoctor.ImgUrl.TrimStart('/'));
//                    if (System.IO.File.Exists(oldFilePath))
//                    {
//                        System.IO.File.Delete(oldFilePath);
//                    }
//                }

//                existingDoctor.ImgUrl = $"/images/Doctor/{newFileName}";
//            }

//            doctorRepository.Edit(existingDoctor);
//            doctorRepository.Commit();

//            return Ok(new { message = "Doctor updated successfully", doctorId = existingDoctor.DoctorID });
//        }


//        [Authorize(Roles = "Admin")]
//        [HttpDelete("{id}")]
//        public IActionResult DeleteDoctor(int id)
//        {
//            var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == id);
//            if (doctor == null)
//                return NotFound(new { message = "Doctor not found" });

//            if (!string.IsNullOrEmpty(doctor.ImgUrl) && doctor.ImgUrl != "wwwroot/images/Photo.JPG")
//            {
//                try
//                {
//                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), doctor.ImgUrl);
//                    if (System.IO.File.Exists(imagePath))
//                    {
//                        System.IO.File.Delete(imagePath);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    return BadRequest(new { message = "Error deleting image", error = ex.Message });
//                }
//            }

//            if (doctor.Tests != null && doctor.Tests.Any())
//            {
//                foreach (var test in doctor.Tests.ToList())
//                {
//                    testRepository.Delete(test);
//                }
//            }

//            if (doctor.TestResults != null && doctor.TestResults.Any())
//            {
//                foreach (var testResult in doctor.TestResults.ToList())
//                {
//                    testResultRepository.Delete(testResult);
//                }
//            }

//            doctorRepository.Delete(doctor);
//            doctorRepository.Commit();

//            return Ok(new { message = "Doctor and related data deleted successfully" });
//        }

//        [Authorize]
//        [HttpGet("GetPhotoUrl")]
//        public IActionResult GetPhotoUrl(int id)
//        {
//            var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == id);
//            if (doctor == null)
//            {
//                return NotFound(new { message = "Doctor not found" });
//            }


//            var imageUrl = string.IsNullOrEmpty(doctor.ImgUrl)
//             ? $"{Request.Scheme}://{Request.Host}/wwwroot/images/Photo.JPG"
//                : $"{Request.Scheme}://{Request.Host}/{doctor.ImgUrl}";

//            return Ok(new { url = imageUrl });
//        }
//    }
//}
