using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Implementations
{
    public class BorrowedBookService(IBorrowedBookRepository borrowedBookRepository, IBookRepository bookRepository, UserManager<IdentityUser> userManager) : IBorrowedBookService
    {
        private readonly IBorrowedBookRepository _borrowedBookRepository = borrowedBookRepository;
        private readonly IBookRepository _bookRepository = bookRepository;

        public async Task AddBorrowedBookAsync(BorrowedBook borrowedBook, string userId, string bookId)
        {
            if (string.IsNullOrEmpty(borrowedBook.BorrowerId) || string.IsNullOrWhiteSpace(borrowedBook.BorrowerId))
                throw new ArgumentException("BorrowerId can't neither be null nor empty or whitespace.");

            if (string.IsNullOrEmpty(borrowedBook.BookId) || string.IsNullOrWhiteSpace(borrowedBook.BookId))
                throw new ArgumentException("BookId can't neither be null nor empty or whitespace.");



            var book = await _bookRepository.GetBookAsync(bookId) ??  throw new KeyNotFoundException($"Book with id [{bookId}] can't be found.");

            if (await userManager.FindByIdAsync(userId) == null)
                throw new KeyNotFoundException($"User with id [{userId}] can't be found.");

            // Check if user has borrowed this book before
            if (await _borrowedBookRepository.UserHasBorrowedBefore(userId, bookId))
                throw new InvalidOperationException("You can't borrow this book again because you've already borrowed it before. For you to borrow it again, you need to return the previous one :)");

            if (book.CopiesAvailable <= 0)
                throw new InvalidOperationException("The book is unavailable.");

            book.BorrowCount += 1;
            book.CopiesAvailable -= 1;
            var savedBook = await _bookRepository.UpdateBookAsync(bookId, book, book);

            var savedBorrowedBook = await _borrowedBookRepository.AddBorrowedBookAsync(borrowedBook);
            if (!savedBorrowedBook || !savedBook)
                throw new DbUpdateException("Something went wrong while saving the update.");

        }


        public async Task<bool> BorrowedBookExistsAsync(string borrowedBookId)
        {

            if (string.IsNullOrEmpty(borrowedBookId) || string.IsNullOrWhiteSpace(borrowedBookId))
                throw new ArgumentException("borrowedBookId can't neither be null nor empty or whitespace.");
            return await _borrowedBookRepository.BorrowedBookExistsAsync(borrowedBookId);
        }

        public async Task<BorrowedBook> GetBorrowedBookAsync(string borrowedBookId)
        {
            if (string.IsNullOrEmpty(borrowedBookId) || string.IsNullOrWhiteSpace(borrowedBookId))
                throw new ArgumentException("borrowedBookId can't neither be null nor empty or whitespace.");

            var borrowedBook = await _borrowedBookRepository.GetBorrowedBookAsync(borrowedBookId);
            return borrowedBook ?? throw new KeyNotFoundException($"Borrowed book with id [{borrowedBookId}] can't be found");
        }

        public async Task<ICollection<BorrowedBook>> GetBorrowedBooksAsync() => await _borrowedBookRepository.GetBorrowedBooksAsync();


        public async Task RemoveBorrowedBookAsync(string bookId)
        {

            if (string.IsNullOrEmpty(bookId) || string.IsNullOrWhiteSpace(bookId))
                throw new ArgumentException("bookId can't neither be null nor empty or whitespace.");
            var foundBorrowedBook = await GetBorrowedBookAsync(bookId) ?? throw new KeyNotFoundException($"BorrowedBook with id [{bookId}] can't be found");

            if (!foundBorrowedBook.HasBeenReturned)
                throw new InvalidOperationException("Return this book before it can be deleted");

            var savedBorrowedBook = await _borrowedBookRepository.RemoveBorrowedBookAsync(foundBorrowedBook);

            if (!savedBorrowedBook)
                throw new DbUpdateException("Something went wrong while saving the update.");
        }

        public async Task UpdateBorrowedBookAsync(string bookId, BorrowedBook returnedBook)
        {
            if (string.IsNullOrEmpty(returnedBook.BorrowerId) || string.IsNullOrWhiteSpace(returnedBook.BorrowerId))
                throw new ArgumentException("BorrowerId can't neither be null nor empty or whitespace.");

            if (string.IsNullOrEmpty(returnedBook.BookId) || string.IsNullOrWhiteSpace(returnedBook.BookId))
                throw new ArgumentException("BookId can't neither be null nor empty or whitespace.");

            if (!await _bookRepository.BookExistsAsync(returnedBook.BookId))
                throw new KeyNotFoundException($"Book with id [{returnedBook.BookId}] can't be found.");
            if (await userManager.FindByIdAsync(returnedBook.BorrowerId) == null)
                throw new KeyNotFoundException($"User with id [{returnedBook.BorrowerId}] can't be found.");

            var foundBorrowedBook = await GetBorrowedBookAsync(bookId) ?? throw new KeyNotFoundException($"BorrowedBook with id [{bookId}] can't be found");

            var savedBorrowedBook = await _borrowedBookRepository.UpdateBorrowedBookAsync(bookId, returnedBook, foundBorrowedBook);
            if (!savedBorrowedBook)
                throw new DbUpdateException("Something went wrong while saving the update.");
        }


        public async Task<ICollection<Book>> GetTop3MostBorrowedBooks()
        {
            return (await _bookRepository.GetBooksAsync())
                .Where(b => b.BorrowCount > 0)
                .OrderByDescending(b => b.BorrowCount)
                .Take(3)
                .ToList();
        }
    }
}
