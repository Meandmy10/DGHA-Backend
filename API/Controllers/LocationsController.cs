using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary;
using ModelsLibrary.Data;

// MODELS
using API.Models;
using API.Models.APIResponse;

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

        [HttpGet ("all")]
        public async IAsyncEnumerable<ActionResult<Place>> getAllLocation () {
            List<string> allPlaceIds = await _context.Reviews.Select ((review) => review.PlaceID).Distinct ().ToListAsync ().ConfigureAwait (false);

            foreach (string id in allPlaceIds) {
                var result = await HttpReq.getPlaceByIdFromGoogle(id).ConfigureAwait (false);
                List<Review> placeReviews = await _context.Reviews.Where (review => review.PlaceID == id).ToListAsync ().ConfigureAwait (true);

                Place place = new Place ();
                place.PlaceId = id;
                place.Name = result.name;
                place.Address = result.formatted_address;
                place.Types = result.types;
                place.State = result.address_components[result.address_components.Count - 3].long_name;
                place.numOfRatings = placeReviews.Count;
                place.avgOverallRating = (float) placeReviews.Average (review => review.OverallRating);
                place.avgCustomerRating = (float) placeReviews.Average (review => review.ServiceRating);
                place.avgLocationRating = (float) placeReviews.Average (review => review.LocationRating);
                place.avgAmentitiesRating = (float) placeReviews.Average (review => review.AmentitiesRating);

                yield return place;
            }
        }

        // sort by state and stars between 4 and 5
        [HttpGet ("recommend")]
        public async IAsyncEnumerable<ActionResult<Place>> getRecommendedLocation (string state) {
            List<string> allPlaceIds = await _context.Reviews.Select ((review) => review.PlaceID).Distinct ().ToListAsync ().ConfigureAwait (false);

            foreach (string id in allPlaceIds) {
                List<Review> placeReviews = await _context.Reviews.Where (review => review.PlaceID == id).ToListAsync ().ConfigureAwait (false);
                double avgOverallRating = placeReviews.Average (r => r.OverallRating);

                if (avgOverallRating >= 4) {
                    var result = await HttpReq.getPlaceByIdFromGoogle(id).ConfigureAwait (false);
                    string placeState = result.address_components[result.address_components.Count - 3].long_name;

                    if (result.address_components[result.address_components.Count - 3].long_name.ToLower () == state.ToLower ()) {
                        Place place = new Place ();
                        place.PlaceId = id;
                        place.Name = result.name;
                        place.Address = result.formatted_address;
                        place.Types = result.types;
                        place.State = result.address_components[result.address_components.Count - 3].long_name;
                        place.numOfRatings = placeReviews.Count;
                        place.avgOverallRating = (float) placeReviews.Average (review => review.OverallRating);
                        place.avgCustomerRating = (float) placeReviews.Average (review => review.ServiceRating);
                        place.avgLocationRating = (float) placeReviews.Average (review => review.LocationRating);
                        place.avgAmentitiesRating = (float) placeReviews.Average (review => review.AmentitiesRating);

                        yield return place; 
                    }
                }
            }
        }
    }
}

