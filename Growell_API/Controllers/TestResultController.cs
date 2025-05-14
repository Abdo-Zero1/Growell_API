using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult Index() {
            var testResult = testResultRepository.Get().ToList();
            return Ok(testResult);
        }
    }
}
