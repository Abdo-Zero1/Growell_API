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
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new { error = "Authorization error", message = "User ID is missing or invalid." });

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { error = "Not Found", message = "The requested user does not exist." });

            var userData = new
            {
                user.UserName,
                user.Email,
                user.PhoneNumber,
                ProfilePicturePath = user.ProfilePicturePath ?? "/images/default-profile.png"
            };

            return Ok(userData);
        }


        [HttpPost("Profile/Update")]
        public async Task<IActionResult> UpdateProfile([FromForm] ProfileDTO profileDTO)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new { error = "Authorization error", message = "User ID is missing or invalid." });

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { error = "Not Found", message = "The requested user does not exist." });

            if (!string.IsNullOrEmpty(profileDTO.UserName))
            {
                user.UserName = profileDTO.UserName;
            }

            if (!string.IsNullOrEmpty(profileDTO.Email))
            {
                user.Email = profileDTO.Email;
            }

            if (!string.IsNullOrEmpty(profileDTO.PhoneNumber))
            {
                user.PhoneNumber = profileDTO.PhoneNumber;
            }

            if (profileDTO.ProfilePicture != null && profileDTO.ProfilePicture.Length > 0)
            {
                try
                {
                    user.ProfilePicturePath = await SaveProfilePicture(profileDTO.ProfilePicture);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = "Image Upload Error", message = ex.Message });
                }
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
                        user.PhoneNumber,
                        user.ProfilePicturePath
                    }
                });
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Failed to update profile.", errors });
            }
        }


        private async Task<string> SaveProfilePicture(IFormFile profilePicture)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(profilePicture.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Invalid file type. Only .jpg, .jpeg, .png, and .gif are allowed.");

            if (profilePicture.Length > 5 * 1024 * 1024)
                throw new Exception("File size exceeds 5MB.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Profile");

            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePicture.CopyToAsync(stream);
            }

            return $"/Images/Profile/{fileName}";
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
