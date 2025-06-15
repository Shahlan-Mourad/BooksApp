using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BookApp.API.Data;
using BookApp.API.Models;
using BookApp.API.DTOs;
using System.Security.Claims;

namespace BookApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all quotes for the current user.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quote>>> GetQuotes()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user" });

            var quotes = await _context.Quotes
                .Include(q => q.User)
                .Include(q => q.Book)
                .Where(q => q.UserId == userId)
                .ToListAsync();

            return quotes;
        }

        /// <summary>
        /// Get a quote by id for the current user.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Quote>> GetQuote(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user" });

            var quote = await _context.Quotes
                .Include(q => q.User)
                .Include(q => q.Book)
                .FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);

            if (quote == null)
                return NotFound(new { message = "Quote not found" });

            return quote;
        }

        /// <summary>
        /// Create a new quote for the current user.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Quote>> CreateQuote([FromBody] Quote quote)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user" });

            quote.UserId = userId;
            quote.CreatedAt = DateTime.UtcNow;
            quote.UpdatedAt = DateTime.UtcNow;

            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, quote);
        }

        /// <summary>
        /// Update an existing quote for the current user.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuote(int id, [FromBody] Quote quote)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != quote.Id)
                return BadRequest(new { message = "Incorrect ID" });

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user" });

            var existingQuote = await _context.Quotes.FindAsync(id);

            if (existingQuote == null)
                return NotFound(new { message = "Quote not found" });

            if (existingQuote.UserId != userId)
                return Forbid();

            // Uppdatera endast tillåtna fält
            existingQuote.Text = quote.Text;
            existingQuote.Author = quote.Author;
            existingQuote.BookId = quote.BookId;
            existingQuote.IsFavorite = quote.IsFavorite;
            existingQuote.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteExists(id))
                    return NotFound(new { message = "Quote not found" });
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a quote for the current user.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user" });

            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
                return NotFound(new { message = "Quote not found" });

            if (quote.UserId != userId)
                return Forbid();

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Toggle favorite status for a quote.
        /// </summary>
        [HttpPut("{id}/toggle-favorite")]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user" });

            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
                return NotFound(new { message = "Quote not found" });

            if (quote.UserId != userId)
                return Forbid();

            quote.IsFavorite = !quote.IsFavorite;
            quote.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool QuoteExists(int id)
        {
            return _context.Quotes.Any(e => e.Id == id);
        }
    }
}