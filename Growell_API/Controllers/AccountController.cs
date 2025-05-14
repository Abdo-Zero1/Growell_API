using AutoMapper;
using Growell_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Utility;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager, IMapper mapper, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] ApplicationUserDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(SD.AdminRole));
                await roleManager.CreateAsync(new IdentityRole(SD.DoctorRole));
                await roleManager.CreateAsync(new IdentityRole(SD.UserRole));
            }

            var user = mapper.Map<ApplicationUser>(userDTO);

            if (userDTO.ProfilePicturePath == null)
            {
                user.ProfilePicturePath = "/images/images.jpg"; 
            }
            else
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(userDTO.ProfilePicturePath.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    return BadRequest("Invalid file type. Only .jpg, .jpeg, .png, and .gif are allowed.");

                if (userDTO.ProfilePicturePath.Length > 2 * 1024 * 1024)
                    return BadRequest("File size exceeds 2MB.");

                var fileName = $"{Guid.NewGuid()}{extension}";
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
                Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await userDTO.ProfilePicturePath.CopyToAsync(stream);
                }

                user.ProfilePicturePath = $"/images/{fileName}";
            }

            var result = await userManager.CreateAsync(user, userDTO.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await userManager.AddToRoleAsync(user, SD.UserRole);

            var token = await GenerateJwtToken(user);
            return Ok(new
            {
                Token = token,
                Message = "The user has been successfully registered!"
            });
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByEmailAsync(loginDto.EmailAddress);
            if (user == null)
                return NotFound(new { Message = "Invalid email or password." });

            var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
                return BadRequest(new { Message = "Invalid email or password." });

            var token = await GenerateJwtToken(user);
            return Ok(new
            {
                Token = token,
                Message = "The user has been successfully logged in!"
            });
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["JwtSettings:ExpiryInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok(new { Message = "User logged out successfully" });
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
                    message = "Profile updated successfully",
                    userData = new
                    {
                        user.UserName,
                        user.Email,
                        user.ProfilePicturePath
                    }
                });
            }

            return BadRequest("Failed to update user profile");
        }

        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirst("sub")?.Value;
            if (userId == null)
                return Unauthorized("User not authorized");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "Account deleted successfully" });
            }

            return BadRequest("Failed to delete account");
        }
    }
}
