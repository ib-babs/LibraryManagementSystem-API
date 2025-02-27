using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories.Implementatios
{
    public class ReturnedBookRepository(LibraryDbContext context) : IReturnedBookRepository
    {
        private readonly LibraryDbContext _context = context;

        public async Task<bool> AddReturnedBookAsync(ReturnedBook book)
        {
            _context.ReturnedBooks.Add(book);
            return await SaveAsync();
        }

        public async Task<bool> ReturnedBookExistsAsync(string bookId) => await _context.ReturnedBooks.AnyAsync(b => b.Id == bookId);

        public async Task<bool> RemoveReturnedBookAsync(ReturnedBook book)
        {
            _context.ReturnedBooks.Remove(book);
            return await SaveAsync();
        }

#pragma warning disable CS8603
        public async Task<ReturnedBook> GetReturnedBookAsync(string id) => await _context.ReturnedBooks.FindAsync(id);

        public async Task<ICollection<ReturnedBook>> GetReturnedBooksAsync()
            => await _context.ReturnedBooks.ToListAsync();


        public async Task<bool> SaveAsync()
        {
            var savedChange = await _context.SaveChangesAsync();
            return savedChange > 0;
        }

        public async Task<bool> UpdateReturnedBookAsync(string id, ReturnedBook book, ReturnedBook foundReturnedBook)
        {
            book.Id = foundReturnedBook.Id;
            book.ReturnerId = foundReturnedBook.ReturnerId;
            book.BorrowedBookId = foundReturnedBook.BorrowedBookId;
            _context.Entry<ReturnedBook>(foundReturnedBook).CurrentValues.SetValues(book);
            return await SaveAsync();
        }
    }
}
