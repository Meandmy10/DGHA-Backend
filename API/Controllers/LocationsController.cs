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
        private readonly string[] _states = new string[] { "QLD", "NSW", "ACT", "VIC", "SA", "WA", "TAS", "NT" };
        public LocationController (ApplicationDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Get 20 places from a specified state/territory with 4 or above stars
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        /// <response code="200">Returns 20 places</response>
        /// <response code="400">If input isn't a state</response>
        [HttpGet ("recommend")]
        [ProducesResponseType (200)]
        [ProducesResponseType (400)]
        public async Task<ActionResult<List<Place>>> getRecommendedLocation (string state) {
            
            if(!_states.Contains(state)) {
                return BadRequest(); 
            }

            List<Place> filteredPlaces = new List<Place> ();                        // Places within the user's state
            List<Place> placesToSend = new List<Place> ();                          // Randomised places to send to user
            List<Task<Place>> getPlaceDetailsTaskList = new List<Task<Place>> ();   // Tasks for the google places API call

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

            // randomise 
            int maxNum = filteredPlaces.Count - 1;
            int[] randUniqueNums = getRandUniNums (0, maxNum, maxNum >= 20 ? 20 : maxNum);

            foreach (var num in randUniqueNums) {
                placesToSend.Add (filteredPlaces[num]);
            }

            return placesToSend;
        }
        
        /// <summary>
        /// Get up to 20 places that matches the user's query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="nextPageToken"></param>
        /// <returns></returns>
        /// <response code="200">Returns 20 places</response>
        /// <response code="400">If the query is null</response>
        [HttpGet ("search")]
        [ProducesResponseType (200)]
        [ProducesResponseType (400)]
        public async Task<ActionResult<SearchResponse>> getSearch (string query, string nextPageToken) {

            if(string.IsNullOrEmpty(query)) {
                return BadRequest();
            }

            SearchResponse searchResponse = new SearchResponse ();
            PlaceAPIQueryResponse paqr = await HttpReq.getPlaceByTextFromGoogle (query, nextPageToken).ConfigureAwait (true);

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

                if (place == null) {
                    place = new Place ();
                }

                setPlaceDetails (place, null, paqr.results[i]);
                searchResponse.results.Add (place);
            }

            searchResponse.nextPageToken = paqr.next_page_token;
            return searchResponse;
        }

        // NOTE: This is temporary until the other endpoint is fixed
        /// <summary>
        /// Gets specified set (of 5) of written reviews for requested Place Id
        /// </summary>
        /// <param name="placeId">Place Id to get reviews for</param>
        /// <param name="set">Set of reviews to get, starts at 0</param>
        /// <returns>Review set from specified place id</returns>
        /// <response code="200">Returns specified set of reviews from specified place</response>
        /// <response code="400">Set number invalid</response>
        /// <response code="404">No Reviews Found</response>
        [HttpGet ("reviews")]
        [ProducesResponseType (200)]
        [ProducesResponseType (400)]
        [ProducesResponseType (404)]
        public async Task<ActionResult<List<Review>>> getReviewsByPlaceId (string placeId, int set) {

            if (set < 0 || string.IsNullOrEmpty(placeId)) {
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

        // check if the place id belongs to a place that is in a specified state
        private async Task<Place> filterPlaceByState (Place databasePlace, string state) {
            var apiPlace = await HttpReq.getPlaceByIdFromGoogle (databasePlace.PlaceId).ConfigureAwait (true);

            // the place's state
            string placeState = apiPlace.address_components[apiPlace.address_components.Count - 3].long_name.ToLower ();

            // check if the place state is the name as the specified state
            if (placeState == state.ToLower ()) {
                Place place = databasePlace;
                place = setPlaceDetails (place, apiPlace, null);
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

        // IdPlaceResult and SearchPlaceResult are extremely similar
        private Place setPlaceDetails (Place place, IdPlaceResult idPlace, SearchPlaceResult searchPlace) {
            place.PlaceId = searchPlace != null ? searchPlace.place_id : idPlace.place_id;
            place.Name = searchPlace != null ? searchPlace.name : idPlace.name;
            place.Address = searchPlace != null ? formatAddress (searchPlace.formatted_address) : formatAddress (idPlace.formatted_address);
            place.Types = searchPlace != null ? searchPlace.types : idPlace.types;
            place.State = idPlace != null ? idPlace.address_components[idPlace.address_components.Count - 3].long_name : null;
            
            return place;
        }

        // Ger rid of state, postcode and country in address string
        private string formatAddress (string input) {

            for (int i = 0; i < _states.Length; i++) {
                if (input.Contains (_states[i])) {
                    int indexOfState = input.IndexOf (_states[i]);
                    input = input.Substring (0, indexOfState - 1);
                }
            }
            return input;
        }
    }
}