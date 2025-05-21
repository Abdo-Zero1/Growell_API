using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeEvents : ControllerBase
    {
       
        private readonly IBookEventRepository bookEventRepository;
        private readonly IVideoEventRepository videoEventRepository;

        public HomeEvents (IBookEventRepository bookEventRepository, IVideoEventRepository videoEventRepository)
        {
            this.bookEventRepository = bookEventRepository;
            this.videoEventRepository = videoEventRepository;
        }

        [HttpGet("GetBooks")]
        public IActionResult GetBooks(int pageNum = 1, int pageSize = 10)
        {
            try
            {
                var query = bookEventRepository.Get();
                var totalRecords = query.Count();
                var book = query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();

                var response = new
                {
                    TotalCount = totalRecords,
                    PageNumber = pageNum,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                    Data = book
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
               
                return StatusCode(StatusCodes.Status500InternalServerError, "Error .");
            }
        }


        [HttpGet("GetVidoes")]
        public IActionResult GetVidoes(int pageNum = 1, int pageSize = 10)
        {
            try
            {
                var query = videoEventRepository.Get();
                var totalRecords = query.Count();
                var Vidoe = query.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();

                var response = new
                {
                    TotalCount = totalRecords,
                    PageNumber = pageNum,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                    Data = Vidoe
                };
                return Ok(response);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error .");
            }
        }

    }
}
