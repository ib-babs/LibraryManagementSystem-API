using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Implementations
{
    public class ReturnedBookService(IReturnedBookRepository returnedBookRepository, IBorrowedBookRepository borrowedBookRepository, UserManager<IdentityUser> userManager, IBookRepository bookRepository) : IReturnedBookService
    {
        private readonly IReturnedBookRepository _returnedBookRepository = returnedBookRepository;
        private readonly IBorrowedBookRepository _borrowedBookRepository = borrowedBookRepository;
        private readonly IBookRepository _bookRepository = bookRepository;

        public async Task AddReturnedBookAsync(ReturnedBook returnedBook, string borrowerId, string borrowedBookId)
        {

            if (string.IsNullOrEmpty(returnedBook.ReturnerId) || string.IsNullOrWhiteSpace(returnedBook.ReturnerId))
                throw new ArgumentException("ReturnerId can't neither be null nor empty or whitespace.");

            if (string.IsNullOrEmpty(returnedBook.BorrowedBookId) || string.IsNullOrWhiteSpace(returnedBook.BorrowedBookId))
                throw new ArgumentException("BorrowedBookId can't neither be null nor empty or whitespace.");

            var borrowedBook = await _borrowedBookRepository.GetBorrowedBookAsync(borrowedBookId) ??  throw new KeyNotFoundException($"Borrowed Book with id [{borrowedBookId}] can't be found.");

            if (await userManager.FindByIdAsync(borrowerId) == null)
                throw new KeyNotFoundException($"User with id [{borrowerId}] can't be found.");
            if (await userManager.FindByIdAsync(returnedBook.ReturnerId) == null)
                throw new KeyNotFoundException($"User with id [{returnedBook.ReturnerId}] can't be found.");

            if (borrowedBook.BorrowerId != borrowerId)
                throw new InvalidOperationException("The one who borrowed the book should be the one returning the book.");

            if (returnedBook.BorrowedBookId != borrowedBookId)
                throw new InvalidOperationException($"borrowedBookId [{borrowedBookId}] is not equal to returnedBook.BorrowedBookId [{returnedBook.BorrowedBookId}]");


            var book = await _bookRepository.GetBookAsync(borrowedBook.BookId) ??
                throw new KeyNotFoundException($"Book with id [{borrowedBook.BookId}] can't be found.");

            if (borrowedBook.HasBeenReturned)
                throw new InvalidOperationException("The book has already been returned");

            borrowedBook.HasBeenReturned = true;
            book.CopiesAvailable += 1;

            await _bookRepository.UpdateBookAsync(borrowedBook.BookId, book, book);
            await _borrowedBookRepository.UpdateBorrowedBookAsync(borrowedBookId, borrowedBook, borrowedBook);
            var savedReturnedBook = await _returnedBookRepository.AddReturnedBookAsync(returnedBook);

            if (!savedReturnedBook)
                throw new DbUpdateException("Something went wrong while saving the update for the returned book.");


        }


        public async Task<bool> ReturnedBookExistsAsync(string returnedBookId)
        {

            if (string.IsNullOrEmpty(returnedBookId) || string.IsNullOrWhiteSpace(returnedBookId))
                throw new ArgumentException("returnedBookId can't neither be null nor empty or whitespace.");
            return await _returnedBookRepository.ReturnedBookExistsAsync(returnedBookId);
        }

        public async Task<ReturnedBook> GetReturnedBookAsync(string returnedBookId)
        {
            if (string.IsNullOrEmpty(returnedBookId) || string.IsNullOrWhiteSpace(returnedBookId))
                throw new ArgumentException("returnedBookId can't neither be null nor empty or whitespace.");

            var returnedBook = await _returnedBookRepository.GetReturnedBookAsync(returnedBookId);
            return returnedBook ?? throw new KeyNotFoundException($"Borrowed book with id [{returnedBookId}] can't be found");
        }

        public async Task<ICollection<ReturnedBook>> GetReturnedBooksAsync() => await _returnedBookRepository.GetReturnedBooksAsync();



        public async Task RemoveReturnedBookAsync(string returnedBookId)
        {

            if (string.IsNullOrEmpty(returnedBookId) || string.IsNullOrWhiteSpace(returnedBookId))
                throw new ArgumentException("returnedBookId can't neither be null nor empty or whitespace.");
            var foundReturnedBook = await GetReturnedBookAsync(returnedBookId) ?? throw new KeyNotFoundException($"ReturnedBook with id [{returnedBookId}] can't be found");

            var savedReturnedBook = await _returnedBookRepository.RemoveReturnedBookAsync(foundReturnedBook);

            if (!savedReturnedBook)
                throw new DbUpdateException("Something went wrong while saving the update.");
        }

        public async Task UpdateReturnedBookAsync(string returnedBookId, ReturnedBook returnedBook)
        {
            if (string.IsNullOrEmpty(returnedBook.ReturnerId) || string.IsNullOrWhiteSpace(returnedBook.ReturnerId))
                throw new ArgumentException("ReturnerId can't neither be null nor empty or whitespace.");

            if (string.IsNullOrEmpty(returnedBook.BorrowedBookId) || string.IsNullOrWhiteSpace(returnedBook.BorrowedBookId))
                throw new ArgumentException("BorrowedBookId can't neither be null nor empty or whitespace.");

            if (!await _borrowedBookRepository.BorrowedBookExistsAsync(returnedBook.BorrowedBookId))
                throw new KeyNotFoundException($"Book with id [{returnedBook.BorrowedBookId}] can't be found.");
            if (await userManager.FindByIdAsync(returnedBook.ReturnerId) == null)
                throw new KeyNotFoundException($"User with id [{returnedBook.ReturnerId}] can't be found.");

            var foundReturnedBook = await GetReturnedBookAsync(returnedBookId) ?? throw new KeyNotFoundException($"ReturnedBook with id [{returnedBookId}] can't be found");

            var savedReturnedBook = await _returnedBookRepository.UpdateReturnedBookAsync(returnedBookId, returnedBook, foundReturnedBook);
            if (!savedReturnedBook)
                throw new DbUpdateException("Something went wrong while saving the update.");
        }
    }
}
