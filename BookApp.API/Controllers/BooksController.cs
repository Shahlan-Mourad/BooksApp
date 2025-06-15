using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BookApp.API.Data;
using BookApp.API.Models;
using BookApp.API.DTOs;
using System.Security.Claims;
using System.Text.Json;

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

        /// <summary>
        /// Get all books.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            var books = await _context.Books
                .Include(b => b.AddedByUser)
                .Select(b => new BookDTO(
                    b.Id,
                    b.Title ?? string.Empty,
                    b.Author ?? string.Empty,
                    b.Description ?? string.Empty,
                    b.PublishedDate,
                    b.ISBN ?? string.Empty,
                    b.CoverImageUrl,
                    (b.AddedByUser != null ? b.AddedByUser.Username : null) ?? string.Empty,
                    b.CreatedAt,
                    b.UpdatedAt
                ))
                .ToListAsync();

            return books;
        }

        /// <summary>
        /// Get a book by id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.AddedByUser)
                .Where(b => b.Id == id)
                .Select(b => new BookDTO(
                    b.Id,
                    b.Title ?? string.Empty,
                    b.Author ?? string.Empty,
                    b.Description ?? string.Empty,
                    b.PublishedDate,
                    b.ISBN ?? string.Empty,
                    b.CoverImageUrl,
                    (b.AddedByUser != null ? b.AddedByUser.Username : null) ?? string.Empty,
                    b.CreatedAt,
                    b.UpdatedAt
                ))
                .FirstOrDefaultAsync();

            if (book == null)
                return NotFound(new { message = "Book not found" });

            return book;
        }

        /// <summary>
        /// Create a new book.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BookDTO>> CreateBook([FromBody] CreateBookDTO createBookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user" });

            var book = new Book
            {
                Title = createBookDto.Title,
                Author = createBookDto.Author,
                Description = createBookDto.Description,
                PublishedDate = createBookDto.PublishedDate,
                ISBN = createBookDto.ISBN,
                CoverImageUrl = createBookDto.CoverImageUrl,
                AddedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var createdBook = await _context.Books
                .Include(b => b.AddedByUser)
                .Where(b => b.Id == book.Id)
                .Select(b => new BookDTO(
                    b.Id,
                    b.Title ?? string.Empty,
                    b.Author ?? string.Empty,
                    b.Description ?? string.Empty,
                    b.PublishedDate,
                    b.ISBN ?? string.Empty,
                    b.CoverImageUrl,
                    (b.AddedByUser != null ? b.AddedByUser.Username : null) ?? string.Empty,
                    b.CreatedAt,
                    b.UpdatedAt
                ))
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, createdBook);
        }

        /// <summary>
        /// Update an existing book.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<BookDTO>> UpdateBook(int id, [FromBody] UpdateBookDTO updateBookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user" });

            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
                return NotFound(new { message = "Book not found" });

            // If the book doesn't belong to the current user, transfer ownership
            if (existingBook.AddedByUserId != userId)
                existingBook.AddedByUserId = userId;

            existingBook.Title = updateBookDto.Title;
            existingBook.Author = updateBookDto.Author;
            existingBook.Description = updateBookDto.Description;
            existingBook.PublishedDate = updateBookDto.PublishedDate;
            existingBook.ISBN = updateBookDto.ISBN;
            existingBook.CoverImageUrl = updateBookDto.CoverImageUrl;
            existingBook.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                    return NotFound(new { message = "Book not found" });
                else
                    throw;
            }
            catch (DbUpdateException ex)
            {
                // Check if it's a unique constraint violation
                if (ex.InnerException?.Message?.Contains("UNIQUE constraint failed") == true)
                {
                    return BadRequest(new { message = "A book with this ISBN already exists. Please use a different ISBN." });
                }
                throw;
            }
            catch (Exception)
            {
                throw;
            }

            var updatedBook = await _context.Books
                .Include(b => b.AddedByUser)
                .Where(b => b.Id == id)
                .Select(b => new BookDTO(
                    b.Id,
                    b.Title ?? string.Empty,
                    b.Author ?? string.Empty,
                    b.Description ?? string.Empty,
                    b.PublishedDate,
                    b.ISBN ?? string.Empty,
                    b.CoverImageUrl,
                    (b.AddedByUser != null ? b.AddedByUser.Username : null) ?? string.Empty,
                    b.CreatedAt,
                    b.UpdatedAt
                ))
                .FirstOrDefaultAsync();

            return Ok(updatedBook);
        }

        /// <summary>
        /// Delete a book.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user" });

            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return NotFound(new { message = "Book not found" });

            // If the book doesn't belong to the current user, transfer ownership first
            if (book.AddedByUserId != userId)
            {
                book.AddedByUserId = userId;
                await _context.SaveChangesAsync();
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