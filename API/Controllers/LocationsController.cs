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
#pragma warning disable CA1304 // Specify CultureInfo
#pragma warning disable CA1307 // Specify StringComparison

namespace API.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string[] _shortStates = new string[]
        {
            "QLD", "NSW", "ACT", "VIC", "SA", "WA", "TAS", "NT"
        };
        private readonly string[] _longStates = new string[]
        {
            "queensland", "new_south_wales", "victoria", "south_australia", "western_australia", "tasmania", "nortern_territory"
        };

        public LocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get 20 places from a specified state/territory with 4 or more stars
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        /// <response code="200">Returns 20 places</response>
        /// <response code="400">If input isn't a state</response>
        [HttpGet("recommend")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<List<Place>>> GetRecommendedLocation(string state)
        {
            if (state == null || !_longStates.Contains(state.ToLower()))
            {
                return BadRequest();
            }

            List<Place> filteredPlaces = new List<Place>();                      // Places within the user's state
            List<Place> placesToSend = new List<Place>();                        // Randomised places to send to user
            List<Task<Place>> getPlaceDetailsTaskList = new List<Task<Place>>(); // Tasks for the google places API call

            List<Place> placeWithHighRating = await _context.Reviews
                .GroupBy(x => x.PlaceID)
                .Where(g => g.Average(p => p.OverallRating) >= 4)
                .Select(g => new Place
                {
                    PlaceId = g.Key,
                    avgOverallRating = g.Average(p => p.OverallRating),
                    avgCustomerRating = g.Average(p => p.ServiceRating),
                    avgLocationRating = g.Average(p => p.LocationRating),
                    avgAmentitiesRating = g.Average(p => p.AmentitiesRating),
                    numOfAllReviews = g.Count(),
                    numOfWrittenReviews = g.Sum(p => !string.IsNullOrEmpty(p.Comment) ? 1 : 0)
                })
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var item in placeWithHighRating)
            {
                Task<Place> task = FilterPlaceByState(item, state);
                getPlaceDetailsTaskList.Add(task);
            }

            var taskData = await Task.WhenAll(getPlaceDetailsTaskList.ToArray())
                                     .ConfigureAwait(false);

            // if the place is not within the user's state, it will be returned as null 
            filteredPlaces = taskData.Where(p => p != null).ToList();

            // randomise 
            int maxNum = filteredPlaces.Count;
            int[] randUniqueNums = GetRandUniNums(0, maxNum, maxNum >= 20 ? 20 : maxNum);

            foreach (var num in randUniqueNums)
            {
                placesToSend.Add(filteredPlaces[num]);
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
        [HttpGet("search")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<SearchResponse>> GetSearch(string query, string nextPageToken)
        {
            if(string.IsNullOrEmpty(query))
            {
                return BadRequest();
            }

            SearchResponse searchResponse = new SearchResponse();
            PlaceAPIQueryResponse paqr = await HttpReq.getPlaceByTextFromGoogle(query, nextPageToken).ConfigureAwait(true);

            for (int i = 0; i < paqr.results.Count; i++)
            {
                Place place = await _context.Reviews
                    .Where(review => review.PlaceID == paqr.results[i].place_id)
                    .GroupBy(p => p.PlaceID)
                    .Select(g => new Place
                    {
                        PlaceId = g.Key,
                        avgOverallRating = g.Average(p => p.OverallRating),
                        avgCustomerRating = g.Average(p => p.ServiceRating),
                        avgLocationRating = g.Average(p => p.LocationRating),
                        avgAmentitiesRating = g.Average(p => p.AmentitiesRating),
                        numOfAllReviews = g.Count(),
                        numOfWrittenReviews = g.Sum(p => !string.IsNullOrEmpty(p.Comment) ? 1 : 0)
                    })
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);

                if (place == null)
                {
                    place = new Place();
                }

                place.PlaceId = paqr.results[i].place_id;
                place.Name = paqr.results[i].name;
                place.Address = FormatAddress(paqr.results[i].formatted_address);
                place.Types = paqr.results[i].types;

                searchResponse.results.Add(place);
            }

            searchResponse.nextPageToken = paqr.next_page_token;
            return searchResponse;
        }

        // check if the place id belongs to a place that is in a specified state
        private async Task<Place> FilterPlaceByState(Place databasePlace, string state)
        {
            var apiPlace = await HttpReq.getPlaceByIdFromGoogle(databasePlace.PlaceId).ConfigureAwait(true);

            // the place's state
            string placeState = apiPlace.address_components[apiPlace.address_components.Count - 3].long_name.ToLower();

            // check if the place state is the name as the specified state
            if (placeState == state.ToLower())
            {
                Place place = databasePlace;

                place.PlaceId = apiPlace.place_id;
                place.Name = apiPlace.name;
                place.Address = FormatAddress(apiPlace.formatted_address);
                place.Types = apiPlace.types;
                place.State = apiPlace.address_components[apiPlace.address_components.Count - 3].long_name;

                return place;
            }

            return null;
        }

        private static int[] GetRandUniNums(int minNumber, int maxNumber, int amount)
        {
            return Enumerable
                .Range(minNumber, maxNumber)
                .OrderBy(g => Guid.NewGuid())
                .Take(amount)
                .ToArray();
        }

        // Ger rid of state, postcode and country in address string
        private string FormatAddress(string input)
        {
            for (int i = 0; i < _shortStates.Length; i++)
            {
                if (input.Contains(_shortStates[i]))
                {
                    int indexOfState = input.IndexOf(_shortStates[i]);
                    input = input.Substring(0, indexOfState - 1);
                }
            }

            return input;
        }
    }
}