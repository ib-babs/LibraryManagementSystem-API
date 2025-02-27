using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Implementations
{
    public class BookService(IBookRepository bookRepository) : IBookService
    {
        private readonly IBookRepository _bookRepository = bookRepository;

        public async Task AddBookAsync(Book book)
        {

            if (string.IsNullOrEmpty(book.ISBN) || string.IsNullOrWhiteSpace(book.ISBN))
                throw new ArgumentException("ISBN can't neither be null nor empty or whitespace.");

            if (string.IsNullOrEmpty(book.Author) || string.IsNullOrWhiteSpace(book.Author))
                throw new ArgumentException("Author can't neither be null nor empty or whitespace.");

            if (string.IsNullOrEmpty(book.Title) || string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("Title can't neither be null nor empty or whitespace.");

            var savedBook = await _bookRepository.AddBookAsync(book);
            if (!savedBook)
                throw new DbUpdateException("Something went wrong while saving the update.");

        }


        public async Task<bool> BookExistsAsync(string bookId)
        {

            if (string.IsNullOrEmpty(bookId) || string.IsNullOrWhiteSpace(bookId))
                throw new ArgumentException("bookId can't neither be null nor empty or whitespace.");
            return await _bookRepository.BookExistsAsync(bookId);
        }

        public async Task<Book> GetBookAsync(string bookId)
        {
            if (string.IsNullOrEmpty(bookId) || string.IsNullOrWhiteSpace(bookId))
                throw new ArgumentException("bookId can't neither be null nor empty or whitespace.");

            return await _bookRepository.GetBookAsync(bookId) ??
                throw new KeyNotFoundException($"Book with id [{bookId}] can't be found");
        }

        public async Task<ICollection<Book>> GetBooksAsync() => await _bookRepository.GetBooksAsync();

        public async Task<IDictionary<string, ICollection<Book>>> GetBooksGrouptedByAuthorAsync()
        {
            var groupedBooks = from book in (await _bookRepository.GetBooksAsync())
                               let author = book.Author
                               group book by author;
            var groupedByAuthor = new Dictionary<string, ICollection<Book>>();

            foreach (var book in groupedBooks)
                groupedByAuthor.Add(book.Key, [.. book]);

            return groupedByAuthor;
        }

        public async Task RemoveBookAsync(string bookId)
        {

            if (string.IsNullOrEmpty(bookId) || string.IsNullOrWhiteSpace(bookId))
                throw new ArgumentException("bookId can't neither be null nor empty or whitespace.");
            var foundBook = await GetBookAsync(bookId) ?? throw new KeyNotFoundException($"Book with id [{bookId}] can't be found");

            var savedBook = await _bookRepository.RemoveBookAsync(foundBook);

            if (!savedBook)
                throw new DbUpdateException("Something went wrong while saving the update.");
        }

        public async Task UpdateBookAsync(string bookId, Book book)
        {
            if (string.IsNullOrEmpty(book.ISBN) || string.IsNullOrWhiteSpace(book.ISBN))
                throw new ArgumentException("ISBN can't neither be null nor empty or whitespace.");

            if (string.IsNullOrEmpty(book.Author) || string.IsNullOrWhiteSpace(book.Author))
                throw new ArgumentException("Author can't neither be null nor empty or whitespace.");

            if (string.IsNullOrEmpty(book.Title) || string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("Title can't neither be null nor empty or whitespace.");

            var foundBook = await GetBookAsync(bookId) ?? throw new KeyNotFoundException($"Book with id [{bookId}] can't be found");

            var savedBook = await _bookRepository.UpdateBookAsync(bookId, book, foundBook);
            if (!savedBook)
                throw new DbUpdateException("Something went wrong while saving the update.");
        }
    }
}
