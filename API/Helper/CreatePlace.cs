using System.Collections.Generic;
using System.Linq;
using API.Models;
using ModelsLibrary;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1031

public class CreatePlace {
    public static Place setPlaceFromIdDetails (Place place, Result apiPlace) {
        place.PlaceId = apiPlace.place_id;
        place.Name = apiPlace.name;
        place.Address = apiPlace.formatted_address;
        place.Types = apiPlace.types;
        place.State = apiPlace.address_components[apiPlace.address_components.Count - 3].long_name;

        return place;
    }

    public static Place setPlaceFromTextDetails (Place place, Results apiPlace) {
        place.PlaceId = apiPlace.place_id;
        place.Name = apiPlace.name;
        place.Address = apiPlace.formatted_address;
        place.Types = apiPlace.types;

        return place;
    }

    public static Place setPlaceFromIdRatings (Place place, Place databasePlace) {
        place.numOfRatings = databasePlace.numOfRatings;
        place.avgOverallRating = databasePlace.avgOverallRating;
        place.avgCustomerRating = databasePlace.avgCustomerRating;
        place.avgLocationRating = databasePlace.avgLocationRating;
        place.avgAmentitiesRating = databasePlace.avgAmentitiesRating;

        return place;
    }

    public static Place setPlaceFromTextRatings (Place place, List<Review> databaseReviews) {
        place.numOfRatings = databaseReviews.Count;
        place.avgOverallRating = databaseReviews.Average (review => review.OverallRating);
        place.avgCustomerRating = databaseReviews.Average (review => review.ServiceRating);
        place.avgLocationRating = databaseReviews.Average (review => review.LocationRating);
        place.avgAmentitiesRating = databaseReviews.Average (review => review.AmentitiesRating);

        return place;
    }
}