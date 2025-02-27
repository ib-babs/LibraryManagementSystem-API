using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem
{
    public class SeedBook
    {
        public static async Task EnsurePopulationAsync(IApplicationBuilder app)
        {
            var context = app.ApplicationServices
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<LibraryDbContext>();

            if ((await context.Database.GetPendingMigrationsAsync()).Any())
                await context.Database.MigrateAsync();

            if (!context.Books.Any())
            {
                IEnumerable<Book> books = [
                    new() { Title = "Things Fall Apart", Author = "Chinua Achebe", ISBN = "978-0385474542", CopiesAvailable = 30},
                    new() { Title = "Half a Yellow Sun", Author = "Chimamanda Ngozi Adichie", ISBN = "978-0060798760", CopiesAvailable = 43},
                    new() { Title = "Lion and the Jewel", Author = "Wole Soyinka", ISBN = "978-0199537891", CopiesAvailable = 21},
                    new() { Title = "Americanan", Author = "Chimamanda Ngozi Adichie", ISBN = "978-0307377348", CopiesAvailable = 11},
                    new() { Title = "The Palm-Wine Drinkard", Author = "Amos Tutuola", ISBN = "978-0802135799", CopiesAvailable = 50},
                    new() { Title = "The Joys of Motherhood", Author = "Buchi Emecheta", ISBN = "978-0435905275", CopiesAvailable = 91},
                    new() { Title = "Stay with Me", Author = "Ayobami Adebayo", ISBN = "978-0708899207", CopiesAvailable = 13},
                    new() { Title = "The Fishermen", Author = "Chigozie Obioma", ISBN = "978-0708899207", CopiesAvailable = 75},
                    new() { Title = "Purple Hibiscus", Author = "Chimamanda Ngozi Adichie", ISBN = "978-1400033412", CopiesAvailable = 10},
                    new() { Title = "An Orchestra of Minorities", Author = "Chigozi Obioma", ISBN = "978-0316412393", CopiesAvailable = 16},
                ];
                await context.Books.AddRangeAsync(books);
                await context.SaveChangesAsync();
            }
        }
    }
}
