using AutoMapper;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/Account[controller]")]
    [ApiController]
   
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(ApplicationUserDTO userDTO)
        {
            if (roleManager.Roles.IsNullOrEmpty())
            {
                await roleManager.CreateAsync(new(SD.AdminRole));
                await roleManager.CreateAsync(new(SD.DoctorRole));
                await roleManager.CreateAsync(new(SD.UserRole));
            }
            if (ModelState.IsValid)
            {
                //ApplicationUser user = new()
                //{
                //    UserName = $"{userDTO.FristName}_{userDTO.LastName}",
                //    Email = userDTO.Email,

                //};

                var user = mapper.Map<ApplicationUser>(userDTO);
                var result = await userManager.CreateAsync(user, userDTO.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, SD.UserRole);
                    await signInManager.SignInAsync(user, false);
                    return Ok();
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(userDTO);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTo)
        {
            var user = await userManager.FindByEmailAsync(loginDTo.EmailAddress);
            if (user != null)
            {
                var result = await userManager.CheckPasswordAsync(user, loginDTo.Password);
                if (result)
                {
                    await signInManager.SignInAsync(user, false);
                    return Ok();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "There are errors");
                }
            }
            return NotFound();
        }


        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await userManager.FindByEmailAsync(changePasswordDTO.Email);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }
            var isOldPasswordValid = await userManager.CheckPasswordAsync(user, changePasswordDTO.OldPassword);
            if (!isOldPasswordValid)
            {
                return BadRequest(new { Message = "Old password is incorrect." });
            }
            var result = await userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Password changed successfully." });
            }
            return BadRequest(result.Errors);
        }



    }
}
