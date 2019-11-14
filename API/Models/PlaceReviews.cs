using ModelsLibrary;
using System.Collections.Generic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace API.Models
{
    public class PlaceReviews
    {
        public float AverageRating { get; set; }
        public float AverageLocationRating { get; set; }
        public float AverageAmentitiesRating { get; set; }
        public float AverageServiceRating { get; set; }
        public int Count { get; set; }

        public IEnumerable<Review> Reviews { get; set; }
    }
}
