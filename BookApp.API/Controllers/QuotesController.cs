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
    public class QuotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Quotes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quote>>> GetQuotes()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return await _context.Quotes
                .Include(q => q.User)
                .Include(q => q.Book)
                .Where(q => q.UserId == userId)
                .ToListAsync();
        }

        // GET: api/Quotes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Quote>> GetQuote(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var quote = await _context.Quotes
                .Include(q => q.User)
                .Include(q => q.Book)
                .FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);

            if (quote == null)
            {
                return NotFound();
            }

            return quote;
        }

        // POST: api/Quotes
        [HttpPost]
        public async Task<ActionResult<Quote>> CreateQuote(Quote quote)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            quote.UserId = userId;
            quote.CreatedAt = DateTime.UtcNow;
            quote.UpdatedAt = DateTime.UtcNow;

            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, quote);
        }

        // PUT: api/Quotes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuote(int id, Quote quote)
        {
            if (id != quote.Id)
            {
                return BadRequest();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingQuote = await _context.Quotes.FindAsync(id);

            if (existingQuote == null)
            {
                return NotFound();
            }

            if (existingQuote.UserId != userId)
            {
                return Forbid();
            }

            quote.UpdatedAt = DateTime.UtcNow;
            _context.Entry(existingQuote).CurrentValues.SetValues(quote);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Quotes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
            {
                return NotFound();
            }

            if (quote.UserId != userId)
            {
                return Forbid();
            }

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Quotes/5/toggle-favorite
        [HttpPut("{id}/toggle-favorite")]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
            {
                return NotFound();
            }

            if (quote.UserId != userId)
            {
                return Forbid();
            }

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