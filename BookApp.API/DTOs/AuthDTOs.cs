using System.ComponentModel.DataAnnotations;

namespace BookApp.API.DTOs
{
    /// <summary>
    /// DTO for user registration.
    /// </summary>
    public class RegisterDTO
    {
        [Required]
        [StringLength(50)]
        public string Username { get; init; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; init; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; init; }

        [Required]
        [StringLength(50)]
        public string LastName { get; init; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; init; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [Compare("Password")]
        public string ConfirmPassword { get; init; }

        public RegisterDTO(string username, string email, string firstName, string lastName, string password, string confirmPassword)
        {
            Username = username;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Password = password;
            ConfirmPassword = confirmPassword;
        }
    }

    /// <summary>
    /// DTO for user login.
    /// Username can be either username or email.
    /// </summary>
    public class LoginDTO
    {
        [Required]
        public string Username { get; init; } // Can be username or email

        [Required]
        public string Password { get; init; }

        public LoginDTO(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    /// <summary>
    /// DTO for authentication response.
    /// </summary>
    public class AuthResponseDTO
    {
        public string Token { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public int Id { get; init; }
        public DateTime Expiration { get; init; }

        public AuthResponseDTO(string token, string username, string email, string? firstName, string? lastName, int id, DateTime expiration)
        {
            Token = token;
            Username = username;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Id = id;
            Expiration = expiration;
        }
    }
}