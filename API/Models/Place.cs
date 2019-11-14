using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Styles

/* NOTE: Get nearby places
*  Right now, we're getting the recommended places by state,
*  If we want be precise, we can use lat and long so we can calculate the distance
*/
namespace API.Models
{
    public class Place
    {
        public string PlaceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
        public List<string> Types { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
        public double avgOverallRating { get; set; }
        public double avgCustomerRating { get; set; }
        public double avgAmentitiesRating { get; set; }
        public double avgLocationRating { get; set; }
        public int numOfAllReviews { get; set; }
        public int numOfWrittenReviews { get; set; }
    }
}