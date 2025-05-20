using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   [Authorize(Roles = $"{SD.AdminRole},{SD.DoctorRole}")]
    public class TestResultController : ControllerBase
    {
        private readonly ITestResultRepository testResultRepository;

        public TestResultController(ITestResultRepository testResultRepository)
        {
            this.testResultRepository = testResultRepository;
        }

        [HttpGet("Get")]
        public IActionResult Index()
        {
            var testResults = testResultRepository.Get(
        new Expression<Func<TestResult, object>>[]
        {
            tr => tr.Doctor,
            tr => tr.applicationUser,
            tr => tr.Test,
            
        },
        null,
        true
    ).ToList();

            return Ok(testResults);
        }


    }
}
