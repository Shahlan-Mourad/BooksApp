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

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register(RegisterDTO registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return BadRequest("Användarnamnet är redan taget");
            }

            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest("E-postadressen är redan registrerad");
            }

            using var hmac = new HMACSHA512();

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDTO
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Expiration = DateTime.Now.AddHours(3)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
            {
                return Unauthorized("Ogiltigt användarnamn eller lösenord");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Ogiltigt användarnamn eller lösenord");
                }
            }

            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDTO
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Expiration = DateTime.Now.AddHours(3)
            };
        }
    }
} 