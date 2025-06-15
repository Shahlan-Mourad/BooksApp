using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookApp.API.Models
{
    /// <summary>
    /// Represents a user's favorite book (many-to-many relationship).
    /// </summary>
    public class UserFavoriteBook
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public virtual Book Book { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}