using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary;
using ModelsLibrary.Data;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1031
#pragma warning disable CA2007

namespace API.Controllers {
    [ApiController]
    [Route ("location")]
    public class LocationController : Controller {
        private readonly ApplicationDbContext _context;
        public LocationController (ApplicationDbContext context) {
            _context = context;
        }

        // sort by state and stars between 4 and 5
        [HttpGet ("recommend")]
        [ProducesResponseType (200)]
        public async Task<ActionResult<List<Place>>> getRecommendedLocation (string state) {
            List<Place> possiblePlacesToSend = new List<Place> ();
            List<Place> placesToSend = new List<Place> ();
            List<Place> placeWithHighRating = await _context.Reviews
                .GroupBy (x => x.PlaceID)
                .Where (g => g.Average (p => p.OverallRating) >= 4)
                .Select (g => new Place {
                    PlaceId = g.Key,
                        avgOverallRating = g.Average (p => p.OverallRating),
                        avgCustomerRating = g.Average (p => p.ServiceRating),
                        avgLocationRating = g.Average (p => p.LocationRating),
                        avgAmentitiesRating = g.Average (p => p.AmentitiesRating),
                        numOfAllReviews = g.Count (),
                        numOfWrittenReviews = g.Sum(p => p.Comment != "" ? 1 : 0)
                }).ToListAsync ().ConfigureAwait (false);

            foreach (Place item in placeWithHighRating) {
                var apiPlace = await HttpReq.getPlaceByIdFromGoogle (item.PlaceId).ConfigureAwait (false);
                string placeState = apiPlace.address_components[apiPlace.address_components.Count - 3].long_name.ToLower ();
                if (placeState == state.ToLower ()) {
                    Place place = new Place ();
                    place = CreatePlace.setPlaceFromIdDetails (place, apiPlace);
                    place = CreatePlace.setPlaceFromIdRatings (place, item);

                    possiblePlacesToSend.Add (place);
                }
            }

            int maxNum = possiblePlacesToSend.Count;
            int[] randUniqueNums = getRandUniNums (0, maxNum, maxNum >= 20 ? 20 : maxNum);

            foreach (var num in randUniqueNums) {
                placesToSend.Add (possiblePlacesToSend[num]);
            }

            return placesToSend;
        }

        [HttpGet ("search")]
        [ProducesResponseType (200)]
        [ProducesResponseType (404)]
        public async Task<ActionResult<SearchResponse>> getSearch (string query, string nextPageToken) {
            SearchResponse searchResponse = new SearchResponse ();
            PlaceAPIQueryResponse paqr = await HttpReq.getPlaceByTextFromGoogle (query, nextPageToken).ConfigureAwait (false);

            if (paqr.results.Count == 0) {
                return NotFound ();
            }

            foreach (Results item in paqr.results) {

                // Place soemthing = await _context.Reviews
                // .Where(review => review.PlaceID == item.place_id)
                // // .GroupJoin()
                // .Select(g => new Place {
                //     PlaceId = g.PlaceID,
                //     avgOverallRating = g.Average (p => p.OverallRating),
                //     avgCustomerRating = g.Average (p => p.ServiceRating),
                //     avgLocationRating = g.Average (p => p.LocationRating),
                //     avgAmentitiesRating = g.Average (p => p.AmentitiesRating),
                //     numOfAllReviews = g.Count(),
                //     numOfWrittenReviews = g.Count (p => p.Comment != "")
                // });
                List<Review> databaseReviews = await _context.Reviews
                    .Where (review => review.PlaceID == item.place_id)
                    .ToListAsync ()
                    .ConfigureAwait (true);

                Place place = new Place ();
                place = CreatePlace.setPlaceFromTextDetails (place, item);

                if (databaseReviews.Count > 0) {
                    place = CreatePlace.setPlaceFromTextRatings (place, databaseReviews);
                }
                searchResponse.results.Add (place);
            }

            searchResponse.nextPageToken = paqr.next_page_token;
            return searchResponse;
        }

        // NOTE: This is temporary until the other endpoint is fixed
        [HttpGet ("reviews")]
        [ProducesResponseType (200)]
        [ProducesResponseType (400)]
        [ProducesResponseType (404)]
        public async Task<ActionResult<List<Review>>> getReviewsByPlaceId (string placeId, int set) {

            if (set < 0) {
                return BadRequest ("Invalid Set");
            }

            List<Review> reviews = await _context.Reviews
                .Where (review => review.PlaceID == placeId && review.Comment != "")
                .OrderByDescending (reviews => reviews.TimeAdded)
                .Skip (5 * set)
                .Take (5)
                .ToListAsync ()
                .ConfigureAwait (false);

            if (reviews.Count == 0) {
                return NotFound ();
            }

            return reviews;
        }

        private int[] getRandUniNums (int minNumber, int maxNumber, int amount) {
            return Enumerable
                .Range (minNumber, maxNumber)
                .OrderBy (g => Guid.NewGuid ())
                .Take (amount)
                .ToArray ();
        }
    }

}