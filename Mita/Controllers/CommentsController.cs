using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mita.DTOs;
using Mita.Models;

namespace Mita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly MitaDatabaseContext _mitaContext;

        public CommentsController(MitaDatabaseContext mitaContext)
        {
            _mitaContext = mitaContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Comment>>> FindAll()
        {
            List<Comment> comments = await _mitaContext.Comments
                .Include(comment => comment.Review)
                .Include(comment => comment.Review.Manga)
                .Include(comment => comment.User)
                .ToListAsync();

            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> FindById([FromRoute] int id)
        {
            // Check if comment exists
            Comment? comment = await _mitaContext.Comments
                .Include(comment => comment.Review)
                .Include(comment => comment.Review.Manga)
                .Include(comment => comment.User)
                .Where(comment => comment.Id == id)
                .FirstOrDefaultAsync();

            if (comment == null) return NotFound();

            return Ok(comment);
        }

        [HttpPost, Authorize(Roles = "Reviewer")]
        public async Task<ActionResult<Comment>> Create(CreateCommentDTO createCommentDTO)
        {
            // Validate if the request body is valid
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Create comment
            Comment newComment = new Comment();
            newComment.Title = createCommentDTO.Title;
            newComment.Body = createCommentDTO.Body;
            newComment.UserId = userId;
            newComment.ReviewId = createCommentDTO.ReviewId;

            // Save comment in database
            await _mitaContext.Comments.AddAsync(newComment);
            int entriesWritten = await _mitaContext.SaveChangesAsync();

            if (entriesWritten == 0) Problem();

            // Find the comment that was just created
            Comment? savedComment = await _mitaContext.Comments
                .Include(comment => comment.Review)
                .Include(comment => comment.Review.Manga)
                .Include(comment => comment.User)
                .Where(comment => comment.Id == newComment.Id)
                .FirstOrDefaultAsync();

            return Ok(savedComment);
        }

        [HttpPatch("{id}"), Authorize(Roles = "Reviewer")]
        public async Task<ActionResult<Comment>> Update([FromRoute] int id, [FromBody] UpdateCommentDTO updateCommentDTO)
        {
            // Check if review exists
            Comment? comments = await _mitaContext.Comments.
                Where(comment => comment.Id == id)
                .Include(comment => comment.Review)
                .Include(comment => comment.Review.Manga)
                .Include(comment => comment.User)
                .FirstOrDefaultAsync();

            if (comments == null) return NotFound();

            // Update comment data
            comments.Title = updateCommentDTO.Title ?? comments.Title;
            comments.Body = updateCommentDTO.Body ?? comments.Body;

            // Save comment changes in database
            int entriesWritten = await _mitaContext.SaveChangesAsync();

            if (entriesWritten == 0) return Problem();

            return Ok(comments);
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin, Reviewer")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            // Check if comment exists
            Comment? comment = await _mitaContext.Comments
                .Where(comment => comment.Id == id)
                .FirstOrDefaultAsync();

            if (comment == null) return NotFound();

            // Check if the user owns the comment
            if (User.FindFirstValue(ClaimTypes.Role) == "Reviewer")
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId != comment.UserId) return Unauthorized("You don't own this comment");
            }

            // Remove comment from database
            _mitaContext.Comments.Remove(comment);
            await _mitaContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
