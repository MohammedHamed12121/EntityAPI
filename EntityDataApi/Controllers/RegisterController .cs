using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EntityDataApi.Data;
using EntityDataApi.Models;
using EntityDataApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EntityDataApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController  : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public RegisterController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        #region Register
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the username is already taken
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                return BadRequest("Username is already taken");
            }

            // Create a new User object
            var user = new User
            {
                Username = model.Username,
                PasswordHash = HashPassword(model.Password),
                FullName = model.FullName 
            };

            // Add the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user.Id.ToString(), "user");

            // Return JWT token as response
            return Ok(new { token });
        }
        #endregion

        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the user by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            // Check if user exists and verify the password
            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user.Id.ToString(), "user");
            

            // Return JWT token as response
            return Ok(new { token });
        }
        #endregion

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private string GenerateJwtToken(string userId, string userRole)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, userRole)
                },
                expires: DateTime.Now.AddMinutes(30), 
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return hashedPassword == HashPassword(password);
        }
    }
}