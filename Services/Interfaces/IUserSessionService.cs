using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IUserSessionService
    {
        Task CreateUserAsync(UserSession userSession, string role);
        Task DeleteUserAsync(string id);
        Task<string> UserLoginAsync(UserSession userSession);
        Task UserLogOutAsync(string id);
        Task<ICollection<IdentityUser>> GetAllUsersAsync();

    }
}
