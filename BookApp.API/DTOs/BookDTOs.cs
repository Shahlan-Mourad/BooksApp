namespace BookApp.API.DTOs
{
    /// <summary>
    /// Data Transfer Object for Book entity.
    /// </summary>
    public class BookDTO
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Author { get; init; }
        public string Description { get; init; }
        public DateTime PublishedDate { get; init; }
        public string ISBN { get; init; }
        public string? CoverImageUrl { get; init; }
        public string AddedByUsername { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }

        public BookDTO(int id, string title, string author, string description, DateTime publishedDate, string isbn, string? coverImageUrl, string addedByUsername, DateTime createdAt, DateTime? updatedAt)
        {
            Id = id;
            Title = title;
            Author = author;
            Description = description;
            PublishedDate = publishedDate;
            ISBN = isbn;
            CoverImageUrl = coverImageUrl;
            AddedByUsername = addedByUsername;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    /// <summary>
    /// Base class for creating or updating a book.
    /// </summary>
    public abstract class BookBaseDTO
    {
        public string Title { get; init; }
        public string Author { get; init; }
        public string Description { get; init; }
        public DateTime PublishedDate { get; init; }
        public string ISBN { get; init; }
        public string? CoverImageUrl { get; init; }

        protected BookBaseDTO(string title, string author, string description, DateTime publishedDate, string isbn, string? coverImageUrl)
        {
            Title = title;
            Author = author;
            Description = description;
            PublishedDate = publishedDate;
            ISBN = isbn;
            CoverImageUrl = coverImageUrl;
        }
    }

    /// <summary>
    /// DTO for creating a new book.
    /// </summary>
    public class CreateBookDTO : BookBaseDTO
    {
        public CreateBookDTO(string title, string author, string description, DateTime publishedDate, string isbn, string? coverImageUrl)
            : base(title, author, description, publishedDate, isbn, coverImageUrl) { }
    }

    /// <summary>
    /// DTO for updating an existing book.
    /// </summary>
    public class UpdateBookDTO : BookBaseDTO
    {
        public UpdateBookDTO(string title, string author, string description, DateTime publishedDate, string isbn, string? coverImageUrl)
            : base(title, author, description, publishedDate, isbn, coverImageUrl) { }
    }
}