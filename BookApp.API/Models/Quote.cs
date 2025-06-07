using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookApp.API.Models
{
    public class Quote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public bool IsFavorite { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
} 