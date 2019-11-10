using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA2227 // Collection properties should be read only

namespace API.Models
{
    public class SearchResponse
    {
        public bool next { get; set; }
        public List<Place> places { get; set; }
    }
}
