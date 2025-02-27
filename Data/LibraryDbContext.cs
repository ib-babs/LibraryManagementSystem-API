using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public class LibraryDbContext(DbContextOptions<LibraryDbContext> opts) : IdentityDbContext<IdentityUser>(opts)
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowedBook> BorrowedBooks { get; set; }
        public DbSet<ReturnedBook> ReturnedBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ReturnedBook>().HasOne(rb => rb.BorrowedBook).WithMany(bb => bb.ReturnedBooks).HasForeignKey(rb => rb.BorrowedBookId).OnDelete(DeleteBehavior.Restrict);
            base.OnModelCreating(builder);
        }
    }
}
