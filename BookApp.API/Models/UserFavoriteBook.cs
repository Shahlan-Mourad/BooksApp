using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookApp.API.Models
{
    public class UserFavoriteBook
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
} 