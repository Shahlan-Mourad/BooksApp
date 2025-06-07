using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookApp.API.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime PublishedDate { get; set; }

        [StringLength(50)]
        public string ISBN { get; set; }

        public string CoverImageUrl { get; set; }

        // Relation till användare som lade till boken
        [ForeignKey("AddedByUser")]
        public int AddedByUserId { get; set; }
        public User AddedByUser { get; set; }

        // Samling av citat från boken
        public ICollection<Quote> Quotes { get; set; }

        // Samling av användare som har boken som favorit
        public ICollection<UserFavoriteBook> FavoriteByUsers { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
} 