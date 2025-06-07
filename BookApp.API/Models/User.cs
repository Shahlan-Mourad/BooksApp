using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookApp.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        [Required]
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Samling av böcker som användaren har lagt till
        public ICollection<Book> AddedBooks { get; set; } = new List<Book>();

        // Samling av citat som användaren har lagt till
        public ICollection<Quote> Quotes { get; set; } = new List<Quote>();

        // Samling av användarens favoritböcker
        public ICollection<UserFavoriteBook> FavoriteBooks { get; set; } = new List<UserFavoriteBook>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
    }
} 