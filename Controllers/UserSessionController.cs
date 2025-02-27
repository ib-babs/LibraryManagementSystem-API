using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Implementations;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSessionController(IUserSessionService userSessionService) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserSession user, string role)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                await userSessionService.CreateUserAsync(user, role);
                return Ok(new { message = "New account has been created." });
            }
            catch (InvalidOperationException ex)
            {
                
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentNullException ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserSession userSession)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                return Ok(await userSessionService.UserLoginAsync(userSession));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                return Ok(await userSessionService.GetAllUsersAsync());
            }
            catch (SqlException ex)
            {

                return StatusCode(500, new { error = ex.Message });
            }
        }

        //public IActionResult LogOut()
        //{

        //}

        //public IActionResult ValidateToken(string token)
        //{

        //}
    }
}
