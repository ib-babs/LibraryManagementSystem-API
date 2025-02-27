using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace LibraryManagementSystem.Repositories.Implementatios
{
    public class BookRepository(LibraryDbContext context) : IBookRepository
    {
        private readonly LibraryDbContext _context = context;

        public async Task<bool> AddBookAsync(Book book)
        {
            _context.Books.Add(book);
            return await SaveAsync();
        }

        public async Task<bool> BookExistsAsync(string bookId) => await _context.Books.AnyAsync(b => b.Id == bookId);

        public async Task<bool> RemoveBookAsync(Book book)
        {
            _context.Books.Remove(book);
            return await SaveAsync();
        }

#pragma warning disable CS8603
        public async Task<Book> GetBookAsync(string id) => await _context.Books.FindAsync(id);

        public async Task<ICollection<Book>> GetBooksAsync()
            => await _context.Books.ToListAsync();


        public async Task<bool> SaveAsync()
        {
            var savedChange = await _context.SaveChangesAsync();
            return savedChange > 0;
        }

        public async Task<bool> UpdateBookAsync(string id, Book book, Book foundBook)
        {
            book.Id = foundBook.Id;
            book.BorrowCount = foundBook.BorrowCount;
            _context.Entry<Book>(foundBook).CurrentValues.SetValues(book);
            return await SaveAsync();
        }
    }
}
