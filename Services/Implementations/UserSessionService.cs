using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Implementations;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace LibraryManagementSystem.Services.Implementatios
{
    public class UserSessionService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager) : IUserSessionService
    {
        public async Task CreateUserAsync(UserSession user, string role)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (await userManager.FindByEmailAsync(user.Email) != null)
                throw new InvalidOperationException("User with this email already exist.");

            if (await userManager.FindByNameAsync(user.UserName) != null)
                throw new InvalidOperationException("Username is already taken.");

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

        public async Task DeleteUserAsync(string id)
        {

            var foundUser = await userManager.FindByIdAsync(id) ?? throw new KeyNotFoundException($"User with id [{id}] can't be found.");
            var result = await userManager.DeleteAsync(foundUser);
            if (!result.Succeeded)
                throw new InvalidOperationException(JsonSerializer.Serialize<IEnumerable<object>>(result.Errors));
        }

        public async Task<string> UserLoginAsync(UserSession user)
        {
            ArgumentNullException.ThrowIfNull(user);
            var foundUser = await userManager.FindByEmailAsync(user.Email) ?? throw new KeyNotFoundException($"User with email [{user.Email}] can't be found.");

            user.UserName = foundUser.UserName!;
            var result = await signInManager.PasswordSignInAsync(foundUser, user.Password, true, false);

            if (!result.Succeeded)
                throw new InvalidOperationException("Password is incorrect");
            var userRole = (await userManager.GetRolesAsync(foundUser)).FirstOrDefault();
            if (string.IsNullOrEmpty(userRole))
                throw new InvalidOperationException("No role found for this user");
            return JwtService.GenerateToken(user, userRole);


        }

        public async Task UserLogOutAsync(string id)
        {
            await signInManager.SignOutAsync();
        }
        public async Task<ICollection<IdentityUser>> GetAllUsersAsync()
        {
            return await userManager.Users.ToListAsync();
        }
    }
}
