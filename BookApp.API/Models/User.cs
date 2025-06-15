using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookApp.API.Models
{
    /// <summary>
    /// Represents an application user.
    /// </summary>
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

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Collection of books added by the user
        public virtual ICollection<Book> AddedBooks { get; set; } = new List<Book>();

        // Collection of quotes added by the user
        public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();

       // Collection of the user's favorite books
        public virtual ICollection<UserFavoriteBook> FavoriteBooks { get; set; } = new List<UserFavoriteBook>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
    }
}