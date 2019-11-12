using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA2227 // Collection properties should be read only

namespace API.Models
{
    public class SearchResponse
    {
        public List<Place> results { get; set; }
        public string nextPageToken {get; set;}

        public SearchResponse() {
            this.results = new List<Place>();
        }
    }
}
