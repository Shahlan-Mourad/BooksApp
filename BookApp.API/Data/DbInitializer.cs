using BookApp.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace BookApp.API.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            // Se till att databasen är skapad
            await context.Database.EnsureCreatedAsync();

            // Kontrollera om det redan finns användare
            if (context.Users.Any())
            {
                return; // Databasen är redan seedad
            }

            // Skapa en testanvändare
            using var hmac = new HMACSHA512();
            var testUser = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Test123!")),
                PasswordSalt = hmac.Key,
                FirstName = "Test",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(testUser);
            await context.SaveChangesAsync(); // Spara användaren först

            // Skapa några exempelböcker
            var books = new List<Book>
            {
                new Book
                {
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    ISBN = "978-0743273565",
                    Description = "A story of the fabulously wealthy Jay Gatsby and his love for the beautiful Daisy Buchanan.",
                    AddedByUserId = testUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Book
                {
                    Title = "1984",
                    Author = "George Orwell",
                    ISBN = "978-0451524935",
                    Description = "A dystopian novel set in a totalitarian society where critical thought is suppressed.",
                    AddedByUserId = testUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Books.AddRange(books);

            // Skapa några exempelcitat
            var quotes = new List<Quote>
            {
                new Quote
                {
                    Text = "So we beat on, boats against the current, borne back ceaselessly into the past.",
                    BookId = books[0].Id,
                    UserId = testUser.Id,
                    IsFavorite = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Quote
                {
                    Text = "Big Brother is watching you.",
                    BookId = books[1].Id,
                    UserId = testUser.Id,
                    IsFavorite = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Quotes.AddRange(quotes);

            // Spara ändringarna
            await context.SaveChangesAsync();
        }
    }
} 