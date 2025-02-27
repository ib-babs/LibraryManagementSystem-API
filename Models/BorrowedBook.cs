using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618
namespace LibraryManagementSystem.Models
{
    public class BorrowedBook
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string BookId { get; set; }
        public string BorrowerId { get; set; }
        public bool HasBeenReturned { get; set; } = false;
        public DateTime BorrowedAt { get; set; } = DateTime.Now;

        public Book? Book { get; set; }
        public IdentityUser? Borrower { get; set; }
        public ICollection<ReturnedBook>? ReturnedBooks { get; set; }

    }
}
