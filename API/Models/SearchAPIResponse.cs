using System.Collections.Generic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Styles

namespace API.Models
{
    public class SearchResponse
    {
        public List<Place> results { get; set; }
        public string nextPageToken { get; set; }

        public SearchResponse()
        {
            results = new List<Place>();
        }
    }
}
