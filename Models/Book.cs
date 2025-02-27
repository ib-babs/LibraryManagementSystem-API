using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618 
namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public int CopiesAvailable { get; set; }

        public int BorrowCount { get; set; } = 0;

        public ICollection<BorrowedBook>? BorrowedBooks { get; set; }
        //public ICollection<ReturnedBook>? ReturnedBooks { get; set; }

    }
}
