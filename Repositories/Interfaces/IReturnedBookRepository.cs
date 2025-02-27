using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories.Interfaces
{
    public interface IReturnedBookRepository
    {
        Task<bool> AddReturnedBookAsync(ReturnedBook book);
        Task<bool> ReturnedBookExistsAsync(string bookId);
        Task<ReturnedBook> GetReturnedBookAsync(string id);
        Task<ICollection<ReturnedBook>> GetReturnedBooksAsync();
        Task<bool> RemoveReturnedBookAsync(ReturnedBook book);
        Task<bool> UpdateReturnedBookAsync(string id, ReturnedBook book, ReturnedBook foundReturnedBook);
        Task<bool> SaveAsync();
    }
}
