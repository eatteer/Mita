using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mita.DTOs;
using Mita.Models;
using Mita.Services;
using System.Security.Claims;

namespace Mita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MitaDatabaseContext _mitaContext;

        public UsersController(MitaDatabaseContext mitaContext)
        {
            _mitaContext = mitaContext;
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<User>>> FindAll()
        {
            List<User> users = await _mitaContext.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> FindById([FromRoute] int id)
        {
            // Check if manga exists
            User? user = await _mitaContext.Users
                .Where(user => user.Id == id)
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> Create(CreateUserDTO createUserDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Check if username is already taken
            User? foundUser = await _mitaContext.Users
                .Where(user => createUserDTO.Username == user.Username)
                .FirstOrDefaultAsync();

            if (foundUser != null) return BadRequest("Username is already taken");

            // Create user and its password hash
            HashingService.CreatePasswordHash(createUserDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new User();
            user.Username = createUserDTO.Username;
            // http://codeissue.com/issues/i24dd7c2da0e61c/problem-using-encoding-utf8-getstring-and-encoding-utf8-getbytes
            user.PasswordHash = Convert.ToBase64String(passwordHash);
            user.PasswordSalt = Convert.ToBase64String(passwordSalt);
            user.Role = createUserDTO.Role;

            // Try saving user in database
            await _mitaContext.Users.AddAsync(user);
            int entriesWritten = await _mitaContext.SaveChangesAsync();
            if (entriesWritten == 0) Problem();

            return Ok(user);
        }

        [HttpPatch("{id}"), Authorize(Roles = "Admin, Reviewer")]
        public async Task<ActionResult<User>> Update([FromRoute] int id, [FromBody] UpdateUserDTO updateUserDTO)
        {
            // Check if user exists
            User? user = await _mitaContext.Users
                .Where(user => user.Id == id)
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();

            // Check if the user owns the account
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId != user.Id) return Unauthorized("You don't own this account");

            // Update user data
            user.Username = updateUserDTO.Username ?? user.Username;
            user.Role = updateUserDTO.Role ?? user.Role;

            if (updateUserDTO.Password != null)
            {
                // Create user password hash
                HashingService.CreatePasswordHash(updateUserDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = Convert.ToBase64String(passwordHash);
                user.PasswordSalt = Convert.ToBase64String(passwordSalt);
            }

            // Save user changes in database
            int entriesWritten = await _mitaContext.SaveChangesAsync();

            if (entriesWritten == 0) return Problem();

            return Ok(user);
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin, Reviewer")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            // Check if user exists
            User? user = await _mitaContext.Users
                .Where(users => users.Id == id)
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();

            // Check if the user owns the account
            if (User.FindFirstValue(ClaimTypes.Role) == "Admin, Reviewer")
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId != user.Id) return Unauthorized("You don't own this account");
            }

            // Remove user from database
            _mitaContext.Users.Remove(user);
            await _mitaContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
