
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace API.Models
{
    public class NewReview
    {
        public string UserID { get; set; }
        public string PlaceID { get; set; }
        public byte OverallRating { get; set; }
        public byte LocationRating { get; set; }
        public byte AmentitiesRating { get; set; }
        public byte ServiceRating { get; set; }
        public string Comment { get; set; }
    }
}
