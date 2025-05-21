using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.IO;
using System;
using System.Linq;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository doctorRepository;

        public DoctorController(IDoctorRepository doctorRepository)
        {
            this.doctorRepository = doctorRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Get")]
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var query = doctorRepository.Get(Include: [t => t.Tests]);

            int totalCount = query.Count();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var doctors = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                currentPage = page,
                pageSize = pageSize,
                totalPages = totalPages,
                totalDoctors = totalCount,
                data = doctors
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateDoctor([FromForm] DoctorDTO doctorDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string imgFileName = "images/images.jpg"; // صورة الديفولت

            if (doctorDTO.ImgUrl != null && doctorDTO.ImgUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images", "Doctor");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                imgFileName = Path.Combine("images", "Doctor", Guid.NewGuid().ToString() + Path.GetExtension(doctorDTO.ImgUrl.FileName));
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), imgFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    doctorDTO.ImgUrl.CopyTo(stream);
                }
            }

            var doctor = new Doctor
            {
                FirstName = doctorDTO.FirstName,
                SecondName = doctorDTO.SecondName,
                LastName = doctorDTO.LastName,
                Email = doctorDTO.Email,
                Gender = doctorDTO.Gender,
                Bio = doctorDTO.Bio,
                AboutMe = doctorDTO.AboutMe,
                Description = doctorDTO.Description,
                PhoneNumber = doctorDTO.PhoneNumber,
                Specialization = doctorDTO.Specialization,
                YearsOfExperience = doctorDTO.YearsOfExperience,
                Education = doctorDTO.Education,
                Age = doctorDTO.Age,
                ImgUrl = imgFileName,
                CreatedAt = DateTime.UtcNow
            };

            doctorRepository.Create(doctor);
            doctorRepository.Commit();

            return Ok(new { message = "Doctor created successfully", doctorId = doctor.DoctorID });
        }

        [Authorize(Roles = "Doctor")]
        [HttpPut("{id}")]
        public IActionResult EditDoctor(int id, [FromForm] DoctorEditDTO dto)
        {
            var existingDoctor = doctorRepository.GetOne(expression: d => d.DoctorID == id);
            if (existingDoctor == null)
                return NotFound(new { message = "Doctor not found" });

            existingDoctor.FirstName = dto.FirstName;
            existingDoctor.PhoneNumber = dto.PhoneNumber;

            if (dto.ImgUrl != null && dto.ImgUrl.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "images", "Doctor");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImgUrl.FileName);
                var fullPath = Path.Combine(folderPath, newFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    dto.ImgUrl.CopyTo(stream);
                }

                existingDoctor.ImgUrl = Path.Combine("images", "Doctor", newFileName);
            }

            doctorRepository.Edit(existingDoctor);
            doctorRepository.Commit();

            return Ok(new { message = "Doctor updated successfully", doctorId = existingDoctor.DoctorID });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteDoctor(int id)
        {
            var doctor = doctorRepository.GetOne(expression: d => d.DoctorID == id);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found" });

            if (!string.IsNullOrEmpty(doctor.ImgUrl) && doctor.ImgUrl != "images/images.jpg")
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), doctor.ImgUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            doctorRepository.Delete(doctor);
            doctorRepository.Commit();

            return Ok(new { message = "Doctor deleted successfully" });
        }

        [HttpGet("GetPhotoUrl")]
        public IActionResult GetPhotoUrl(string? fileName = null)
        {
            var defaultImage = "images/images.jpg";

            var imagePath = string.IsNullOrEmpty(fileName) ? defaultImage : Path.Combine("images/Doctor", fileName);

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), imagePath);
            if (!System.IO.File.Exists(fullPath))
            {
                imagePath = defaultImage;
            }

            var imageUrl = $"{Request.Scheme}://{Request.Host}/{imagePath}";

            return Ok(new { url = imageUrl });
        }
    }
}
