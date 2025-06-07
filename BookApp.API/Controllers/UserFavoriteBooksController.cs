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

        // GET: api/UserFavoriteBooks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetFavoriteBooks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return await _context.UserFavoriteBooks
                .Where(ufb => ufb.UserId == userId)
                .Include(ufb => ufb.Book)
                    .ThenInclude(b => b.AddedByUser)
                .Include(ufb => ufb.Book)
                    .ThenInclude(b => b.Quotes)
                .Select(ufb => ufb.Book)
                .ToListAsync();
        }

        // POST: api/UserFavoriteBooks
        [HttpPost]
        public async Task<ActionResult<UserFavoriteBook>> AddFavoriteBook(int bookId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Kontrollera om boken redan är favorit
            if (await _context.UserFavoriteBooks
                .AnyAsync(ufb => ufb.UserId == userId && ufb.BookId == bookId))
            {
                return BadRequest("Boken är redan markerad som favorit");
            }

            // Kontrollera om boken existerar
            if (!await _context.Books.AnyAsync(b => b.Id == bookId))
            {
                return NotFound("Boken hittades inte");
            }

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

        // DELETE: api/UserFavoriteBooks/5
        [HttpDelete("{bookId}")]
        public async Task<IActionResult> RemoveFavoriteBook(int bookId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userFavoriteBook = await _context.UserFavoriteBooks
                .FirstOrDefaultAsync(ufb => ufb.UserId == userId && ufb.BookId == bookId);

            if (userFavoriteBook == null)
            {
                return NotFound("Favoritboken hittades inte");
            }

            _context.UserFavoriteBooks.Remove(userFavoriteBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
} 