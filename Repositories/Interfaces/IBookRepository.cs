using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<bool> AddBookAsync(Book book);
        Task<bool> BookExistsAsync(string bookId);
        Task<Book> GetBookAsync(string id);
        Task<ICollection<Book>> GetBooksAsync();
        Task<bool> RemoveBookAsync(Book book);
        Task<bool> UpdateBookAsync(string id, Book book, Book foundBook);
        Task<bool> SaveAsync();
    }
}
