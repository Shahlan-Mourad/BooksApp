using BookApp.API.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookApp.API.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            // Make sure the database is created
            await context.Database.EnsureCreatedAsync();

            // Check if there are already users
            if (context.Users.Any())
            {
                return; // Database has been seeded
            }

            // Start a transaction to ensure all seeding is done atomically
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.UtcNow;

                // // Create a test user
                using var hmac = new HMACSHA512();
                var testUser = new User
                {
                    Username = "testuser",
                    Email = "test@example.com",
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Test123!")),
                    PasswordSalt = hmac.Key,
                    FirstName = "Test",
                    LastName = "User",
                    CreatedAt = now
                };

                context.Users.Add(testUser);
                await context.SaveChangesAsync(); // Save the user first

                // Create some example books
                var books = new List<Book>
                {
                    new Book
                    {
                        Title = "The Great Gatsby",
                        Author = "F. Scott Fitzgerald",
                        ISBN = "978-0743273565",
                        Description = "A story of the fabulously wealthy Jay Gatsby and his love for the beautiful Daisy Buchanan.",
                        PublishedDate = new DateTime(1925, 4, 10),
                        CoverImageUrl = "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1490528560i/4671.jpg",
                        AddedByUserId = testUser.Id,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Book
                    {
                        Title = "1984",
                        Author = "George Orwell",
                        ISBN = "978-0451524935",
                        Description = "A dystopian novel set in a totalitarian society where critical thought is suppressed.",
                        PublishedDate = new DateTime(1949, 6, 8),
                        CoverImageUrl = "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1532714506i/40961427.jpg",
                        AddedByUserId = testUser.Id,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Book
                    {
                        Title = "To Kill a Mockingbird",
                        Author = "Harper Lee",
                        ISBN = "978-0061120084",
                        Description = "The story of young Scout Finch and her father Atticus in a racially divided Alabama town.",
                        PublishedDate = new DateTime(1960, 7, 11),
                        CoverImageUrl = "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1553383690i/2657.jpg",
                        AddedByUserId = testUser.Id,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Book
                    {
                        Title = "Pride and Prejudice",
                        Author = "Jane Austen",
                        ISBN = "978-0141439518",
                        Description = "The story follows the emotional development of Elizabeth Bennet, who learns the error of making hasty judgments.",
                        PublishedDate = new DateTime(1813, 1, 28),
                        CoverImageUrl = "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1320399351i/1885.jpg",
                        AddedByUserId = testUser.Id,
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                };

                context.Books.AddRange(books);
                await context.SaveChangesAsync(); // Save the books first

                // Create some example quotes
                var quotes = new List<Quote>
                {
                    new Quote
                    {
                        Text = "So we beat on, boats against the current, borne back ceaselessly into the past.",
                        Author = "F. Scott Fitzgerald",
                        BookId = books[0].Id,
                        UserId = testUser.Id,
                        IsFavorite = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Quote
                    {
                        Text = "Big Brother is watching you.",
                        Author = "George Orwell",
                        BookId = books[1].Id,
                        UserId = testUser.Id,
                        IsFavorite = false,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Quote
                    {
                        Text = "You never really understand a person until you consider things from his point of view... Until you climb inside of his skin and walk around in it.",
                        Author = "Harper Lee",
                        BookId = books[2].Id,
                        UserId = testUser.Id,
                        IsFavorite = true,
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Quote
                    {
                        Text = "It is a truth universally acknowledged, that a single man in possession of a good fortune, must be in want of a wife.",
                        Author = "Jane Austen",
                        BookId = books[3].Id,
                        UserId = testUser.Id,
                        IsFavorite = false,
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                };

                context.Quotes.AddRange(quotes);

                // Create some favorite books for the test user
                var favoriteBooks = new List<UserFavoriteBook>
                {
                    new UserFavoriteBook
                    {
                        UserId = testUser.Id,
                        BookId = books[0].Id,
                        AddedAt = now
                    },
                    new UserFavoriteBook
                    {
                        UserId = testUser.Id,
                        BookId = books[2].Id,
                        AddedAt = now
                    }
                };

                context.UserFavoriteBooks.AddRange(favoriteBooks);

                // Save all changes
                await context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch
            {
                // Rollback the transaction if any error occurs
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}