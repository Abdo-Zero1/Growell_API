using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevelopmentStatusController : ControllerBase
    {
        private readonly IDevelopmentStatusRepository developmentStatusRepository;

        public DevelopmentStatusController(IDevelopmentStatusRepository developmentStatusRepository)
        {
            this.developmentStatusRepository = developmentStatusRepository;
        }
        [HttpGet]
        public IActionResult Index() {
            var development = developmentStatusRepository.Get().ToList();
            return Ok(development);
        }

    }
}
