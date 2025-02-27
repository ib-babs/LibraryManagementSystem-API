using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace LibraryManagementSystem.Repositories.Implementatios
{
    public class UserSessionRepository(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : IUserSessionRepository
    {
        public async Task CreateUser(UserSession user, string role)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (await userManager.FindByEmailAsync(user.Email) != null)
                throw new InvalidOperationException("User with this email already exist.");

            var newUser = new IdentityUser() { UserName = user.UserName, Email = user.Email, EmailConfirmed = true };

            var userResult = await userManager.CreateAsync(newUser, user.Password);
            if (!userResult.Succeeded)
            {
                throw new InvalidOperationException(JsonSerializer.Serialize<IEnumerable<object>>(userResult.Errors));
            }

            var userRole = new IdentityRole(role);
            if (!await roleManager.RoleExistsAsync(role))
            {

                var roleResult = await roleManager.CreateAsync(userRole);
                if (!roleResult.Succeeded)
                    throw new InvalidOperationException(JsonSerializer.Serialize<IEnumerable<object>>(roleResult.Errors));
            }

            var addToRoleResult = await userManager.AddToRoleAsync(newUser, role);
            if (!addToRoleResult.Succeeded)
                throw new InvalidOperationException(JsonSerializer.Serialize<IEnumerable<object>>(addToRoleResult.Errors));


        }

        public async Task DeleteUser(string id)
        {
            var foundUser = await userManager.FindByIdAsync(id) ?? throw new KeyNotFoundException($"User with id [{id}] can't be found.");

            var result = await userManager.DeleteAsync(foundUser);
            if (!result.Succeeded)
                throw new InvalidOperationException(JsonSerializer.Serialize<IEnumerable<object>>(result.Errors));
        }


    }
}
