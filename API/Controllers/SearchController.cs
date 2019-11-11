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
            List<Results> results = await HttpReq.getPlaceByTextFromGoogle (query).ConfigureAwait (false);

            foreach (Results item in results) {
                List<Review> databaseReviews = await _context.Reviews
                    .Where (review => review.PlaceID == item.place_id)
                    .ToListAsync ()
                    .ConfigureAwait (true);

                Place place = new Place ();
                place = setPlaceDetails (place, item);

                if (databaseReviews.Count > 0) {
                    place = setPlaceRating (place, databaseReviews);
                }

                yield return place;
            }
        }

        private Place setPlaceDetails (Place place, Results apiPlace) {
            place.PlaceId = apiPlace.place_id;
            place.Name = apiPlace.name;
            place.Address = apiPlace.formatted_address;
            place.Types = apiPlace.types;

            return place;
        }

        private Place setPlaceRating (Place place, List<Review> databaseReviews) {
            place.numOfRatings = databaseReviews.Count;
            place.avgOverallRating = databaseReviews.Average (review => review.OverallRating);
            place.avgCustomerRating = databaseReviews.Average (review => review.ServiceRating);
            place.avgLocationRating = databaseReviews.Average (review => review.LocationRating);
            place.avgAmentitiesRating = databaseReviews.Average (review => review.AmentitiesRating);

            return place;
        }
    }
}