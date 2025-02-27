using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618
namespace LibraryManagementSystem.Models
{
    public class ReturnedBook
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("BorrowedBook")]
        public string BorrowedBookId { get; set; }
        public string ReturnerId { get; set; }
        public DateTime ReturnedAt { get; set; } = DateTime.Now;

        public BorrowedBook? BorrowedBook { get; set; }
        public IdentityUser? Returner { get; set; }
    }
}
