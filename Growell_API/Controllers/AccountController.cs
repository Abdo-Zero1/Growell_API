using AutoMapper;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.Linq;
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
                user.ProfilePicturePath = "/images/images.jpg"; 

                var result = await userManager.CreateAsync(user, userDTO.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, SD.UserRole);
                    await signInManager.SignInAsync(user, false);
                    return Ok(new { message = "the user has been successfully registered!" });
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
                    await signInManager.SignInAsync(user, loginDTo.RememberMe);
                    return Ok(new { Message = "the user has been successfully login." });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "There are errors");
                }
            }
            else
            {
                ModelState.AddModelError("UserName", "Indalid UserName");

            }
            return NotFound();
        }


        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            //var userId = User.FindFirst("sub")?.Value;

            //if (userId == null)
            {
                return Ok(new { Message = "user logged out successfully" });
            }
            //else
            //{
            //    return Ok(new { Message = "User logged out successfully" });
            //}
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

        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst("sub")?.Value;
            if (userId == null)
                return Unauthorized("User not authorized.");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var userProfile = new ProfileDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                ProfilePicturePath = user.ProfilePicturePath
            };

            return Ok(userProfile);
        }


        [HttpPost("Profile/Update")]
        public async Task<IActionResult> UpdateProfile([FromForm] ApplicationUserDTO profileDTO)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (userId == null)
                return Unauthorized("User not authorized.");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            user.UserName = profileDTO.FristName ?? profileDTO.LastName;

            user.Email = profileDTO.Email ?? user.Email;

            if (!string.IsNullOrEmpty(profileDTO.Password))
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await userManager.ResetPasswordAsync(user, token, profileDTO.Password);
                if (!resetResult.Succeeded)
                {
                    return BadRequest("Failed to update password.");
                }
            }

            if (profileDTO.ProfilePicturePath != null && profileDTO.ProfilePicturePath.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(profileDTO.ProfilePicturePath.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                    return BadRequest("Invalid file type. Only .jpg, .jpeg, .png, and .gif are allowed.");

                if (profileDTO.ProfilePicturePath.Length > 2 * 1024 * 1024)
                    return BadRequest("File size exceeds 2MB.");

                var fileName = $"{Guid.NewGuid()}{extension}";
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
                Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileDTO.ProfilePicturePath.CopyToAsync(stream);
                }

                user.ProfilePicturePath = $"/images/{fileName}";
            }

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    message = "Profile updated successfully.",
                    userData = new
                    {
                        user.UserName,
                        user.Email,
                        user.ProfilePicturePath
                    }
                });
            }

            return BadRequest("Failed to update user profile.");
        }











    }
}
