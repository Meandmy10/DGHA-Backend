using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary;
using ModelsLibrary.Data;

// MODELS
using API.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1031
#pragma warning disable CA2007

namespace API.Controllers {
    [ApiController]
    [Route ("search")]
    public class SearchController : Controller {
        private readonly ApplicationDbContext _context;
        public SearchController (ApplicationDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async IAsyncEnumerable<Place> getSearch (string query) {
            var results = await HttpReq.getPlaceByTextFromGoogle (query).ConfigureAwait (false);

            foreach (var item in results) {
                string placeId = item.place_id;

                Place place = new Place ();
                place.PlaceId = placeId;
                place.Name = item.name;
                place.Address = item.formatted_address;
                place.Types = item.types;

                // In this case, state doesn't matter
                place.State = null;             

                // get the reviews
                List<Review> placeReviews = await _context.Reviews.Where (review => review.PlaceID == placeId).ToListAsync ().ConfigureAwait (true);

                if (placeReviews.Count > 0) {
                    place.numOfRatings = placeReviews.Count;
                    place.avgOverallRating = (float) placeReviews.Average (review => review.OverallRating);
                    place.avgCustomerRating = (float) placeReviews.Average (review => review.ServiceRating);
                    place.avgLocationRating = (float) placeReviews.Average (review => review.LocationRating);
                    place.avgAmentitiesRating = (float) placeReviews.Average (review => review.AmentitiesRating);

                }

                yield return place;
            }
        }
    }
}