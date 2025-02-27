using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Repositories.Interfaces
{
    public interface IUserSessionRepository
    {
        Task CreateUser(UserSession userSession, string role);
        Task DeleteUser(string id);
        

    }
}
