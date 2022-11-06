using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mita.DTOs;
using Mita.Models;

namespace Mita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly MitaContext _mitaContext;

        public ReviewsController(MitaContext mitaContext)
        {
            _mitaContext = mitaContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Review>>> FindAll()
        {
            List<Review> reviews = await _mitaContext.Reviews
                .Include(review => review.User)
                .Include(review => review.Manga)
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> FindById([FromRoute] int id)
        {
            // Check if review exists
            Review? review = await _mitaContext.Reviews
                .Include(review => review.User)
                .Include(review => review.Manga)
                .Where(review => review.Id == id)
                .FirstOrDefaultAsync();

            if (review == null) return NotFound();

            return Ok(review);
        }

        [HttpPost, Authorize(Roles = "Reviewer")]
        public async Task<ActionResult<Review>> Create(CreateReviewDTO createReviewDTO)
        {
            // Validate if the request body is valid
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Create review
            Review newReview = new Review();
            newReview.Title = createReviewDTO.Title;
            newReview.Body = createReviewDTO.Body;
            newReview.UserId = userId;
            newReview.MangaId = createReviewDTO.MangaId;

            // Save review in database
            await _mitaContext.Reviews.AddAsync(newReview);
            int entriesWritten = await _mitaContext.SaveChangesAsync();

            if (entriesWritten == 0) Problem();

            // Find the review that was just created
            Review? savedReview = await _mitaContext.Reviews
                .Include(review => review.User)
                .Include(review => review.Manga)
                .Where(review => review.Id == newReview.Id)
                .FirstOrDefaultAsync();

            return Ok(savedReview);
        }

        [HttpPatch("{id}"), Authorize(Roles = "Reviewer")]
        public async Task<ActionResult<Review>> Update([FromRoute] int id, [FromBody] UpdateReviewDTO updateReviewDTO)
        {
            // Check if review exists
            Review? review = await _mitaContext.Reviews.
                Where(review => review.Id == id)
                .Include(review => review.User)
                .Include(review => review.Manga)
                .FirstOrDefaultAsync();

            if (review == null) return NotFound();

            // Check if the user owns the review
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId != review.UserId) return Unauthorized("You don't own this review");

            // Update review data
            review.Title = updateReviewDTO.Title ?? review.Title;
            review.Body = updateReviewDTO.Body ?? review.Body;

            // Save review changes in database
            int entriesWritten = await _mitaContext.SaveChangesAsync();

            if (entriesWritten == 0) return Problem();

            return Ok(review);
        }

        [HttpDelete("{id}"), Authorize(Roles = "Reviewer")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            // Check if review exists
            Review? review = await _mitaContext.Reviews
                .Where(review => review.Id == id)
                .FirstOrDefaultAsync();

            if (review == null) return NotFound();

            // Check if the user owns the review
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId != review.UserId) return Unauthorized("You don't own this review");

            // Remove review from database
            _mitaContext.Reviews.Remove(review);
            await _mitaContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
