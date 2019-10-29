using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary;
using ModelsLibrary.Data;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace API.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets All Reviews
        /// </summary>
        /// <returns>All Reviews</returns>
        /// <response code="200">Returns All Reviews</response>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            return await _context.Reviews.ToListAsync()
                                         .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets All Reviews for requested Place Id
        /// </summary>
        /// <param name="placeId">Place Id to get reviews for</param>
        /// <returns>All Reviews from specified place id</returns>
        /// <response code="200">Returns All Reviews from specified place</response>
        /// <response code="404">No Reviews Found</response>
        [HttpGet("placeId/{placeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        public async Task<ActionResult<PlaceReviews>> GetPlaceReviews(string placeId)
        {
            var reviews = await _context.Reviews.Where(review => review.PlaceID == placeId)
                                                .ToListAsync()
                                                .ConfigureAwait(false);

            if (reviews == null)
            {
                return NotFound();
            }

            var placeReviews = new PlaceReviews
            {
                AverageRating = (byte)reviews.Average(review => review.OverallRating),
                AverageLocationRating = (byte)reviews.Average(review => review.LocationRating),
                AverageAmentitiesRating = (byte)reviews.Average(review => review.AmentitiesRating),
                AverageServiceRating = (byte)reviews.Average(review => review.ServiceRating),
                Count = reviews.Count,
                Reviews = reviews
            };

            return placeReviews;
        }

        /// <summary>
        /// Gets all reviews for specified user
        /// </summary>
        /// <param name="userId">User Id to get reviews for</param>
        /// <returns>All Reviews from speficiedn user</returns>
        /// <response code="200">Returns All Reviews from speficiedn user</response>
        /// <response code="404">No Reviews Found</response>
        [HttpGet("userId/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Review>>> GetUserReviews(string userId)
        {
            var reviews = await _context.Reviews.Where(review => review.UserID == userId)
                                                .ToListAsync()
                                                .ConfigureAwait(false);

            if (reviews == null)
            {
                return NotFound();
            }

            return reviews;
        }

        /// <summary>
        /// Gets Specified Review
        /// </summary>
        /// <param name="placeId">Place Id of review's place</param>
        /// <param name="userId">User Id of review's user</param>
        /// <returns>Requested Review</returns>
        /// <response code="200">Returns Requested Review</response>
        /// <response code="404">No Review Found</response>
        [HttpGet("{placeId}/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        public async Task<ActionResult<Review>> GetReview(string placeId, string userId)
        {
            var review = await _context.Reviews.FindAsync(placeId, userId);

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        /// <summary>
        /// Posts New Review
        /// </summary>
        /// <param name="newReview">Review to add</param>
        /// <returns>Added Review</returns>
        /// <response code="201">Returns Posted Review</response>
        /// <response code="400">Review is undefined</response>
        /// <response code="409">Conflict, Review for user and place exists</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<Review>> PostReview(NewReview newReview)
        {
            if (newReview == null)
            {
                return BadRequest();
            }

            if (!HasOwnedDataAccess(newReview.UserID))
            {
                return Forbid();
            }

            //if location doesn't exist, add location to db
            if (!await _context.Locations.AnyAsync(e => e.PlaceID == newReview.PlaceID).ConfigureAwait(false))
            {
                var location = new Location(newReview.PlaceID);
                _context.Locations.Add(location);
            }

            _context.Reviews.Add(new Review
            {
                PlaceID = newReview.PlaceID,
                UserID = newReview.UserID,
                OverallRating = newReview.OverallRating,
                LocationRating = newReview.LocationRating,
                AmentitiesRating = newReview.AmentitiesRating,
                ServiceRating = newReview.ServiceRating,
                Comment = newReview.Comment
            });

            try
            {
                await _context.SaveChangesAsync()
                              .ConfigureAwait(false);
            }
            catch (DbUpdateException)
            {
                if (await ReviewExists(newReview.PlaceID, newReview.UserID).ConfigureAwait(false))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetReview",
                new
                {
                    placeId = newReview.PlaceID,
                    userId = newReview.UserID
                },
                newReview);
        }

        /// <summary>
        /// Puts Updated Review
        /// </summary>
        /// <param name="placeId">Reviews Place Id</param>
        /// <param name="userId">Reviews User Id</param>
        /// <param name="review">Updated Review</param>
        /// <returns>Action Result</returns>
        /// <response code="204">Review Successfully Updated</response>
        /// <response code="400">Invalid Input</response>
        /// <response code="404">Review Doesn't Exist</response>
        [HttpPut("{placeId}/{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutReview(string placeId, string userId, Review review)
        {
            if (review == null || placeId != review.PlaceID || userId != review.UserID)
            {
                return BadRequest();
            }

            if (!HasOwnedDataAccess(review.UserID))
            {
                return Forbid();
            }

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync()
                              .ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ReviewExists(placeId, userId).ConfigureAwait(false))
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

        /// <summary>
        /// Delete Specified Review
        /// </summary>
        /// <param name="placeId">Place Id for Review</param>
        /// <param name="userId">User Id for Review</param>
        /// <returns>Deleted Review</returns>
        /// <response code="200">Returns Deleted Review</response>
        /// <response code="404">Review Not Found</response>
        [HttpDelete("{placeId}/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Review>> DeleteReview(string placeId, string userId)
        {
            var review = await _context.Reviews.FindAsync(userId, placeId);

            if (review == null)
            {
                return NotFound();
            }

            if (!HasOwnedDataAccess(review.UserID))
            {
                return Forbid();
            }

            _context.Reviews.Remove(review);

            //If there are no reviews for location, delete location from db
            if (!await _context.Reviews.AnyAsync(review => review.PlaceID == placeId).ConfigureAwait(false))
            {
                var location = await _context.Locations.FindAsync(review.PlaceID)
                                                       .ConfigureAwait(false);
                _context.Locations.Remove(location);
            }

            await _context.SaveChangesAsync()
                          .ConfigureAwait(false);

            return review;
        }

        private async Task<bool> ReviewExists(string placeId, string userId)
        {
            return await _context.Reviews.AnyAsync(e => e.PlaceID == placeId && e.UserID == userId)
                                         .ConfigureAwait(false);
        }

        private bool HasOwnedDataAccess(string userId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim.Value == userId)
            {
                return true;
            }
            else
            {
                var roleClaims = claimsIdentity.FindAll(ClaimTypes.Role);
                if (roleClaims.Any(claim => claim.Value == "Administrator"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
