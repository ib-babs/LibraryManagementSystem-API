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
    public class BorrowedBookController(IBorrowedBookService service) : ControllerBase
    {
        private readonly IBorrowedBookService _service = service;

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            try
            {
                return Ok(await _service.GetBorrowedBooksAsync());
            }
            catch (SqlException ex)
            {

                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBorrowedBook(string id)
        {
            try
            {
                return Ok(await _service.GetBorrowedBookAsync(id));
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
        public async Task<IActionResult> AddBorrowedBook(BorrowedBook borrowedBook, string borrowerId, string bookId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                await _service.AddBorrowedBookAsync(borrowedBook, borrowerId, bookId);
                return Ok(new { message = "You have borrowed a book." });
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
        public async Task<IActionResult> RemoveBorrowedBook(string id)
        {
            try
            {
                await _service.RemoveBorrowedBookAsync(id);
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
        public async Task<IActionResult> UpdateBorrowedBook(string id, BorrowedBook borrowedBook)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                await _service.UpdateBorrowedBookAsync(id, borrowedBook);
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

        [Authorize(Roles = "Admin, User")]
        [HttpGet("top3_most_borrowed_books")]
        public async Task<IActionResult> GetTop3MostBorrowedBook()
        {
            try
            {
                return Ok(await _service.GetTop3MostBorrowedBooks());
            }
            catch (SqlException ex)
            {

                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
