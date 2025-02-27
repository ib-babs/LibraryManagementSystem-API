using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IReturnedBookService
    {
        Task AddReturnedBookAsync(ReturnedBook returnedBook, string userId, string borrowedBookId);
        Task<ReturnedBook> GetReturnedBookAsync(string returnedBookId);
        Task<ICollection<ReturnedBook>> GetReturnedBooksAsync();
        Task RemoveReturnedBookAsync(string returnedBookId);
        Task UpdateReturnedBookAsync(string returnedBookId, ReturnedBook returnedBook);
        Task<bool> ReturnedBookExistsAsync(string returnedBookId);
    }
}
