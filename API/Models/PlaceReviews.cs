using ModelsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace API.Models
{
    public class PlaceReviews
    {
        public byte AverageRating { get; set; }
        public byte AverageLocationRating { get; set; }
        public byte AverageAmentitiesRating { get; set; }
        public byte AverageServiceRating { get; set; }
        public int Count { get; set; }

        public IEnumerable<Review> Reviews { get; set; }
    }
}
