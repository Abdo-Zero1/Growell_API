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
        private readonly IChildRepository childRepository;

        public DevelopmentStatusController(IDevelopmentStatusRepository developmentStatusRepository, IChildRepository childRepository)
        {
            this.developmentStatusRepository = developmentStatusRepository;
            this.childRepository = childRepository;
        }
        [HttpGet]
        public IActionResult Index() {
            var development = developmentStatusRepository.Get(Include: [c=>c.Children]).ToList();
            return Ok(development);
        }

    }
}
