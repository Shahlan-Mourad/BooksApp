using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookApp.API.Data;
using BookApp.API.DTOs;
using BookApp.API.Models;
using BookApp.API.Services;
using System.Security.Cryptography;
using System.Text;

namespace BookApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Register a new user.
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                return BadRequest(new { message = "Username is already registered" });

            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return BadRequest(new { message = "The email address is already registered." });

            using var hmac = new HMACSHA512();

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponseDTO(
                token,
                user.Username,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Id,
                DateTime.UtcNow.AddHours(3)
            );

            return CreatedAtAction(nameof(Register), response);
        }

        /// <summary>
        /// Login with username/email and password.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Try to find user by username or email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username || u.Email == loginDto.Username);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            if (!computedHash.SequenceEqual(user.PasswordHash))
                return Unauthorized(new { message = "Invalid username or password" });

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponseDTO(
                token,
                user.Username,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Id,
                DateTime.UtcNow.AddHours(3)
            ));
        }
    }
}