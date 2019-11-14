using System;
using System.Collections.Generic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace API.Models
{
    public class ComplaintLocation
    {
        public string PlaceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<string> Types { get; set; }
        public List<SimpleComplaint> Complaints { get; set; }
    }

    public class SimpleComplaint
    {
        public string PlaceID { get; set; }
        public string UserID { get; set; }
        public string UserEmail { get; set; }
        public DateTime TimeSubmitted { get; set; }

        public string Comment { get; set; }
        public DateTime TimeLastUpdated { get; set; }
        public string Status { get; set; }
    }
}
