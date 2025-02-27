using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnedBookController(IReturnedBookService service) : ControllerBase
    {
        private readonly IReturnedBookService _service = service;

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<IActionResult> GetReturnedBooks()
        {
            try
            {
                return Ok(await _service.GetReturnedBooksAsync());
            }
            catch (SqlException ex)
            {

                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReturnedBook(string id)
        {
            try
            {
                return Ok(await _service.GetReturnedBookAsync(id));
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public async Task<IActionResult> AddReturnedBook(ReturnedBook returnedBook, string returnerId, string borrowedBookId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                await _service.AddReturnedBookAsync(returnedBook, returnerId, borrowedBookId);
                return Ok(new { message = "The book has been returned. Thank you!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveReturnedBook(string id)
        {
            try
            {
                await _service.RemoveReturnedBookAsync(id);
                return Ok(new { message = "Successfully removed." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReturnedBook(string id, ReturnedBook returnedBook)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                await _service.UpdateReturnedBookAsync(id, returnedBook);
                return Ok(new { message = "Updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
