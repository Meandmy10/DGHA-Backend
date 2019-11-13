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
            List<Place> filteredPlaces = new List<Place> (); // Places within the user's state
            List<Place> placesToSend = new List<Place> (); // Randomised places to send to user
            List<Task<Place>> getPlaceDetailsTaskList = new List<Task<Place>> (); // Tasks for the google places API call

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
                        numOfWrittenReviews = g.Sum (p => p.Comment != "" ? 1 : 0)
                }).ToListAsync ().ConfigureAwait (false);

            foreach (var item in placeWithHighRating) {
                Task<Place> task = filterPlaceByState (item, state);
                getPlaceDetailsTaskList.Add (task);
            }

            var taskData = await Task.WhenAll (getPlaceDetailsTaskList.ToArray ());

            // if the place is not within the user's state, it will be returned as null 
            filteredPlaces = taskData.Where (p => p != null).ToList ();

            int maxNum = filteredPlaces.Count - 1;
            int[] randUniqueNums = getRandUniNums (0, maxNum, maxNum >= 20 ? 20 : maxNum);

            foreach (var num in randUniqueNums) {
                placesToSend.Add (filteredPlaces[num]);
            }

            return placesToSend;
        }

        [HttpGet ("search")]
        [ProducesResponseType (200)]
        [ProducesResponseType (404)]
        public async Task<ActionResult<SearchResponse>> getSearch (string query, string nextPageToken) {
            SearchResponse searchResponse = new SearchResponse ();
            PlaceAPIQueryResponse paqr = await HttpReq.getPlaceByTextFromGoogle (query, nextPageToken).ConfigureAwait (true);

            if (paqr.results.Count == 0) {
                return NotFound ();
            }

            for (int i = 0; i < paqr.results.Count; i++) {
                Place place = await _context.Reviews
                    .Where (review => review.PlaceID == paqr.results[i].place_id)
                    .GroupBy (p => p.PlaceID)
                    .Select (g => new Place {
                        PlaceId = g.Key,
                            avgOverallRating = g.Average (p => p.OverallRating),
                            avgCustomerRating = g.Average (p => p.ServiceRating),
                            avgLocationRating = g.Average (p => p.LocationRating),
                            avgAmentitiesRating = g.Average (p => p.AmentitiesRating),
                            numOfAllReviews = g.Count (),
                            numOfWrittenReviews = g.Sum (p => p.Comment != "" ? 1 : 0)
                    })
                    .FirstOrDefaultAsync ().ConfigureAwait (true);

                if(place == null) {
                    place = new Place();
                }

                CreatePlace.setPlaceDetails (place, null, paqr.results[i]);
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

        private async Task<Place> filterPlaceByState (Place databasePlace, string state) {
            var apiPlace = await HttpReq.getPlaceByIdFromGoogle (databasePlace.PlaceId).ConfigureAwait (true);
            string placeState = apiPlace.address_components[apiPlace.address_components.Count - 3].long_name.ToLower ();

            if (placeState == state.ToLower ()) {
                Place place = databasePlace;
                place = CreatePlace.setPlaceDetails (place, apiPlace, null);
                return place;
            }

            return null;
        }

        private int[] getRandUniNums (int minNumber, int maxNumber, int amount) {
            Random random = new Random ();
            int[] randUniNums = new int[20];

            for (int i = 0; i < randUniNums.Length; i++) {
                randUniNums[i] = random.Next (minNumber, maxNumber);
            }

            return randUniNums;
        }
    }
}