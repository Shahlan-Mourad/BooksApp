using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookApp.API.Models
{
    /// <summary>
    /// Represents a book entity in the system.
    /// </summary>
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime PublishedDate { get; set; }

        [Required]
        [StringLength(50)]
        public string ISBN { get; set; } = null!;

        public string? CoverImageUrl { get; set; }

        // Relation to user who added the book
        [ForeignKey("AddedByUser")]
        public int AddedByUserId { get; set; }
        public virtual User AddedByUser { get; set; } = null!;

        // Collection of quotes from the book
        public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();

        // Collection of users who have the book as a favorite
        public virtual ICollection<UserFavoriteBook> FavoriteByUsers { get; set; } = new List<UserFavoriteBook>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}