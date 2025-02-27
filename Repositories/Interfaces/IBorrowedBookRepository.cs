using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories.Interfaces
{
    public interface IBorrowedBookRepository
    {
        Task<bool> AddBorrowedBookAsync(BorrowedBook borrowedBook);
        Task<bool> BorrowedBookExistsAsync(string borrowedBookId);
        Task<BorrowedBook> GetBorrowedBookAsync(string id);
        Task<bool> UserHasBorrowedBefore(string userId, string bookId);
        Task<ICollection<BorrowedBook>> GetBorrowedBooksAsync();
        Task<bool> RemoveBorrowedBookAsync(BorrowedBook borrowedBook);
        Task<bool> UpdateBorrowedBookAsync(string id, BorrowedBook borrowedBook, BorrowedBook foundBorrowedBook);
        Task<bool> SaveAsync();
    }
}
