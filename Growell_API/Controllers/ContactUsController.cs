using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using System.Linq.Expressions;
using Models;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsRepository contactUsRepository;

        public ContactUsController(IContactUsRepository contactUsRepository)
        {
            this.contactUsRepository = contactUsRepository;
        }
        [HttpPost]
        public IActionResult SubmitComplaint([FromBody] ContactUs contactUs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            contactUs.CreatedAt = DateTime.UtcNow;

            contactUsRepository.Create(contactUs);
            contactUsRepository.Commit();

            return Ok(new { Message = "Complaint submitted successfully." });
        }

        [HttpGet]
        public IActionResult GetComplaints()
        {
            var contacts = contactUsRepository.Get(
                new Expression<Func<ContactUs, object>>[] { c => c.User }, 
                null, 
                true  
            );

            return Ok(contacts);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateComplaintStatus(int id, [FromBody] ContactUs updatedContact)
        {
            var complaint = contactUsRepository.Get(
                new Expression<Func<ContactUs, object>>[] { c => c.User },
                expression: c => c.Id == id,
                tracked: true
            ).FirstOrDefault();

            if (complaint == null)
            {
                return NotFound(new { Message = "Complaint not found." });
            }

            complaint.IsResolved = updatedContact.IsResolved;
            complaint.IsViewed = updatedContact.IsViewed;

            contactUsRepository.Edit(complaint);
            contactUsRepository.Commit();

            return Ok(new { Message = "Complaint status updated successfully." });
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteComplaint(int id)
        {
            var contact = contactUsRepository.Get(
                expression: c => c.Id == id
            ).FirstOrDefault();

            if (contact == null)
            {
                return NotFound(new { Message = "Complaint not found." });
            }

            contactUsRepository.Delete(contact);
            contactUsRepository.Commit();

            return Ok(new { Message = "Complaint deleted successfully." });
        }

    }
}
