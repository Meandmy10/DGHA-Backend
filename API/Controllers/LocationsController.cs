using System.Collections.Generic;
using System.Linq;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet ("all")]
        public async IAsyncEnumerable<ActionResult<Place>> getAllLocation () {

            List<Place> allPlaces = await _context.Reviews
                .GroupBy (x => x.PlaceID)
                .Select (g => new Place {
                    PlaceId = g.Key,
                    avgOverallRating = g.Average (p => p.OverallRating),
                    avgCustomerRating = g.Average (p => p.ServiceRating),
                    avgLocationRating = g.Average (p => p.LocationRating),
                    avgAmentitiesRating = g.Average (p => p.AmentitiesRating),
                    numOfRatings = g.Count (p => p.PlaceID != null)
                }).ToListAsync ().ConfigureAwait (false);

            foreach (var item in allPlaces) {
                var apiPlace = await HttpReq.getPlaceByIdFromGoogle (item.PlaceId).ConfigureAwait (false);
                Place place = new Place ();
                place = setPlaceDetails (place, apiPlace);
                place = setPlaceRatings (place, item);

                yield return place;
            }
        }

        // sort by state and stars between 4 and 5
        [HttpGet ("recommend")]
        public async IAsyncEnumerable<ActionResult<Place>> getRecommendedLocation (string state) {

            List<Place> placeWithHighRating = await _context.Reviews
                .GroupBy (x => x.PlaceID)
                .Where (g => g.Average (p => p.OverallRating) >= 4)
                .OrderByDescending (g => g.Count (p => p.PlaceID != null))
                .ThenByDescending (g => g.Average (p => p.OverallRating))
                .Select (g => new Place {
                    PlaceId = g.Key,
                    avgOverallRating = g.Average (p => p.OverallRating),
                    avgCustomerRating = g.Average (p => p.ServiceRating),
                    avgLocationRating = g.Average (p => p.LocationRating),
                    avgAmentitiesRating = g.Average (p => p.AmentitiesRating),
                    numOfRatings = g.Count (p => p.PlaceID != null)
                }).ToListAsync ().ConfigureAwait (false);

            foreach (Place item in placeWithHighRating) {
                var apiPlace = await HttpReq.getPlaceByIdFromGoogle (item.PlaceId).ConfigureAwait (false);
                string placeState = apiPlace.address_components[apiPlace.address_components.Count - 3].long_name.ToLower ();
                if (placeState == state.ToLower ()) {
                    Place place = new Place ();
                    place = setPlaceDetails (place, apiPlace);
                    place = setPlaceRatings (place, item);

                    yield return place;
                }
            }
        }

        private Place setPlaceDetails (Place place, Result apiPlace) {
            place.PlaceId = apiPlace.place_id;
            place.Name = apiPlace.name;
            place.Address = apiPlace.formatted_address;
            place.Types = apiPlace.types;
            place.State = apiPlace.address_components[apiPlace.address_components.Count - 3].long_name;

            return place;
        }

        private Place setPlaceRatings (Place place, Place databasePlace) {
            place.numOfRatings = databasePlace.numOfRatings;
            place.avgOverallRating = databasePlace.avgOverallRating;
            place.avgCustomerRating = databasePlace.avgCustomerRating;
            place.avgLocationRating = databasePlace.avgLocationRating;
            place.avgAmentitiesRating = databasePlace.avgAmentitiesRating;

            return place;
        }
    }

}