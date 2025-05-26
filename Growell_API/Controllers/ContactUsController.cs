using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsRepository contactUsRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public ContactUsController(IContactUsRepository contactUsRepository, UserManager<ApplicationUser> userManager)
        {
            this.contactUsRepository = contactUsRepository;
            this.userManager = userManager;
        }

        [HttpPost]
        public IActionResult SubmitComplaint([FromBody] ContactUs contactUs)
        {
            var userId = userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            contactUs.UserId = userId;
            contactUs.CreatedAt = DateTime.UtcNow;

            contactUsRepository.Create(contactUs);
            contactUsRepository.Commit();

            return Ok(new { Message = "Complaint submitted successfully." });
        }


        [HttpGet]
        [Authorize(Roles = $"{SD.DoctorRole},{SD.AdminRole}")]
        public IActionResult GetComplaints()
        {
            var contacts = contactUsRepository.Get(
                new Expression<Func<ContactUs, object>>[] { c => c.User },
                null,
                true
            ).Select(c => new
            {
                id= c.Id,
                UserName = c.User?.UserName, 
                Email = c.User?.Email,
                Title= c.Title,
                phone = c.User.PhoneNumber,
                Address = c.User.Adderss,
                description= c.Description,
                isResolved = c.IsResolved,
                isViewed = c.IsViewed,
                date = c.CreatedAt
               
            }).ToList();
            

            return Ok(contacts);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = $"{SD.DoctorRole},{SD.AdminRole}")]

        public async Task<IActionResult> UpdateComplaintStatus(int id, [FromBody] ContactUs updatedContact)
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

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            var roles = await userManager.GetRolesAsync(user);

            if (!roles.Contains("Doctor"))
            {
                return Forbid("Only admins can update complaint status.");
                
                //return Forbid("Only admins can update complaint status.");
            }

            complaint.IsResolved = updatedContact.IsResolved;
            complaint.IsViewed = updatedContact.IsViewed;

            contactUsRepository.Edit(complaint);
            contactUsRepository.Commit();

            return Ok(new { Message = "Complaint status updated successfully." });
        }

            [HttpDelete("{id}")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.DoctorRole}")]

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
