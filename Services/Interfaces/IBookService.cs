using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IBookService
    {
        Task AddBookAsync(Book book);
        Task<Book> GetBookAsync(string bookId);
        Task<ICollection<Book>> GetBooksAsync();
        Task<IDictionary<string, ICollection<Book>>> GetBooksGrouptedByAuthorAsync();
        Task RemoveBookAsync(string bookId);
        Task UpdateBookAsync(string bookId, Book book);
        Task<bool> BookExistsAsync(string bookId);
    }
}
