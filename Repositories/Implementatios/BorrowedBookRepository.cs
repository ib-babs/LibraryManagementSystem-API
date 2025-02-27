using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories.Implementatios
{
    public class BorrowedBookRepository(LibraryDbContext context) : IBorrowedBookRepository
    {
        private readonly LibraryDbContext _context = context;

        public async Task<bool> AddBorrowedBookAsync(BorrowedBook borrowedBook)
        {
            _context.BorrowedBooks.Add(borrowedBook);
            return await SaveAsync();
        }

        public async Task<bool> BorrowedBookExistsAsync(string borrowedBookId) => await _context.BorrowedBooks.AnyAsync(b => b.Id == borrowedBookId);

        public async Task<bool> RemoveBorrowedBookAsync(BorrowedBook borrowedBook)
        {
            _context.BorrowedBooks.Remove(borrowedBook);
            return await SaveAsync();
        }

#pragma warning disable CS8603
        public async Task<BorrowedBook> GetBorrowedBookAsync(string id) => await _context.BorrowedBooks.FindAsync(id);

        public async Task<ICollection<BorrowedBook>> GetBorrowedBooksAsync()
            => await _context.BorrowedBooks.ToListAsync();


        public async Task<bool> SaveAsync()
        {
            var savedChange = await _context.SaveChangesAsync();
            return savedChange > 0;
        }

        public async Task<bool> UpdateBorrowedBookAsync(string id, BorrowedBook borrowedBook, BorrowedBook foundBorrowedBook)
        {
            borrowedBook.Id = foundBorrowedBook.Id;
            borrowedBook.BorrowerId = foundBorrowedBook.BorrowerId;
            borrowedBook.BookId = foundBorrowedBook.BookId;
            _context.Entry<BorrowedBook>(foundBorrowedBook).CurrentValues.SetValues(borrowedBook);
            return await SaveAsync();
        }

        public async Task<bool> UserHasBorrowedBefore(string userId, string bookId)
        {
            var borrowedBook = await _context.BorrowedBooks.Where(bb => bb.BorrowerId == userId && bb.BookId == bookId).AnyAsync(bb => bb.HasBeenReturned == false);
            return borrowedBook;
        }
    }
}
