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
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books
                .Include(b => b.AddedByUser)
                .Include(b => b.Quotes)
                .ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.AddedByUser)
                .Include(b => b.Quotes)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            book.AddedByUserId = userId;
            book.CreatedAt = DateTime.UtcNow;
            book.UpdatedAt = DateTime.UtcNow;

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingBook = await _context.Books.FindAsync(id);

            if (existingBook == null)
            {
                return NotFound();
            }

            if (existingBook.AddedByUserId != userId)
            {
                return Forbid();
            }

            book.UpdatedAt = DateTime.UtcNow;
            _context.Entry(existingBook).CurrentValues.SetValues(book);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            if (book.AddedByUserId != userId)
            {
                return Forbid();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
} 