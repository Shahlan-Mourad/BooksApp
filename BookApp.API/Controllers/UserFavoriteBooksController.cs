using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BookApp.API.Data;
using BookApp.API.Models;
using System.Security.Claims;

namespace BookApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserFavoriteBooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserFavoriteBooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all favorite books for the current user.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetFavoriteBooks()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user" });

            var books = await _context.UserFavoriteBooks
                .Where(ufb => ufb.UserId == userId)
                .Include(ufb => ufb.Book)
                    .ThenInclude(b => b.AddedByUser)
                .Include(ufb => ufb.Book)
                    .ThenInclude(b => b.Quotes)
                .Select(ufb => ufb.Book)
                .ToListAsync();

            return books;
        }

        /// <summary>
        /// Add a book to the current user's favorites.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddFavoriteBook([FromQuery] int bookId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user" });

            if (await _context.UserFavoriteBooks.AnyAsync(ufb => ufb.UserId == userId && ufb.BookId == bookId))
                return BadRequest(new { message = "The book is already marked as a favorite." });

            if (!await _context.Books.AnyAsync(b => b.Id == bookId))
                return NotFound(new { message = "Book not found" });

            var userFavoriteBook = new UserFavoriteBook
            {
                UserId = userId,
                BookId = bookId,
                AddedAt = DateTime.UtcNow
            };

            _context.UserFavoriteBooks.Add(userFavoriteBook);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFavoriteBooks), new { id = userFavoriteBook.Id }, userFavoriteBook);
        }

        /// <summary>
        /// Remove a book from the current user's favorites.
        /// </summary>
        [HttpDelete("{bookId}")]
        public async Task<IActionResult> RemoveFavoriteBook(int bookId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user" });

            var userFavoriteBook = await _context.UserFavoriteBooks
                .FirstOrDefaultAsync(ufb => ufb.UserId == userId && ufb.BookId == bookId);

            if (userFavoriteBook == null)
                return NotFound(new { message = "Favorite book not found" });

            _context.UserFavoriteBooks.Remove(userFavoriteBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}