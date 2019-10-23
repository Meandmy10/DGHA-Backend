using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary;
using ModelsLibrary.Data;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            return await _context.Reviews.ToListAsync()
                                         .ConfigureAwait(false);
        }

        // GET: Reviews/5
        [HttpGet("{placeId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetPlaceReviews(string placeId)
        {
            var reviews = await _context.Reviews.Where(review => review.PlaceID == placeId)
                                               .ToListAsync()
                                               .ConfigureAwait(false);

            if (reviews == null)
            {
                return NotFound();
            }

            return reviews;
        }

        // GET: Reviews/5
        [HttpGet("{placeId}/{userId}")]
        public async Task<ActionResult<Review>> GetReview(string placeId, string userId)
        {
            var review = await _context.Reviews.FirstAsync(review => review.PlaceID == placeId)
                                               .ConfigureAwait(false);

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        // PUT: Reviews/5/8
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{placeId}/{userId}")]
        public async Task<IActionResult> PutReview(string placeId, string userId, Review review)
        {
            if (review == null || placeId != review.PlaceID || userId != review.UserID)
            {
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync()
                              .ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(placeId,userId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Reviews
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Review>> PostReview(Review review)
        {
            if(review == null)
            {
                return BadRequest();
            }

            _context.Reviews.Add(review);

            try
            {
                await _context.SaveChangesAsync()
                              .ConfigureAwait(false);
            }
            catch (DbUpdateException)
            {
                if (ReviewExists(review.PlaceID, review.UserID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetReview", 
                new { 
                placeId = review.PlaceID, 
                userId = review.UserID },
                review);
        }

        // DELETE: api/Reviews/5
        [HttpDelete("{placeId}/{userId}")]
        public async Task<ActionResult<Review>> DeleteReview(string placeId, string userId)
        {
            var review = await _context.Reviews.FindAsync(userId, placeId);

            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync()
                          .ConfigureAwait(false);

            return review;
        }

        private bool ReviewExists(string placeId, string userId)
        {
            return _context.Reviews.Any(e => e.PlaceID == placeId && e.UserID == userId);
        }
    }
}
