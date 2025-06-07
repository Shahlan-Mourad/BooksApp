using System.ComponentModel.DataAnnotations;

namespace BookApp.API.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime Expiration { get; set; }
    }
} 