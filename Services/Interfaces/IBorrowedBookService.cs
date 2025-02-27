using LibraryManagementSystem.Models;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IBorrowedBookService
    {
        Task AddBorrowedBookAsync(BorrowedBook borrowedBook, string userId, string bookId);
        Task<BorrowedBook> GetBorrowedBookAsync(string borrowedBookId);
        Task<ICollection<BorrowedBook>> GetBorrowedBooksAsync();
        Task RemoveBorrowedBookAsync(string borrowedBookId);
        Task UpdateBorrowedBookAsync(string borrowedBookId, BorrowedBook borrowedBook);
        Task<bool> BorrowedBookExistsAsync(string borrowedBookId);
        Task<ICollection<Book>> GetTop3MostBorrowedBooks();
    }
}
