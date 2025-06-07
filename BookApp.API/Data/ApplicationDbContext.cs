using Microsoft.EntityFrameworkCore;
using BookApp.API.Models;

namespace BookApp.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<UserFavoriteBook> UserFavoriteBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurera unika index
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();

            // Konfigurera relationer för UserFavoriteBook
            modelBuilder.Entity<UserFavoriteBook>()
                .HasOne(ufb => ufb.User)
                .WithMany(u => u.FavoriteBooks)
                .HasForeignKey(ufb => ufb.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFavoriteBook>()
                .HasOne(ufb => ufb.Book)
                .WithMany(b => b.FavoriteByUsers)
                .HasForeignKey(ufb => ufb.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Konfigurera relationer för Quote
            modelBuilder.Entity<Quote>()
                .HasOne(q => q.User)
                .WithMany(u => u.Quotes)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Quote>()
                .HasOne(q => q.Book)
                .WithMany(b => b.Quotes)
                .HasForeignKey(q => q.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Konfigurera relationer för Book
            modelBuilder.Entity<Book>()
                .HasOne(b => b.AddedByUser)
                .WithMany(u => u.AddedBooks)
                .HasForeignKey(b => b.AddedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 