
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq.Expressions;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = $"{SD.AdminRole}")]

    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly ITestRepository testRepository;

        public DoctorController(IDoctorRepository doctorRepository, ITestRepository testRepository)
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
                    Image = d.ImgUrl,
                    FullName = $"{d.FirstName} {d.LastName}",
                    Bio = d.Bio,
                    Specialization = d.Specialization,
                })
                .Skip((page - 1) * pageSize) 
                .Take(pageSize) 
                .ToList();

            return Ok(topDoctors);
        }
        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var doctor = doctorRepository.Get()
                .Where(d => d.DoctorID == id)
                .Select(d => new
                {
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
                    test= d.Tests,

                })
                .FirstOrDefault();

            if (doctor == null)
            {
                return NotFound(new { Message = "Doctor not found" });
            }

            return Ok(doctor);
        }









    }
}
