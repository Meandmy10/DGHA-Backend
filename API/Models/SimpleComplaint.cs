using ModelsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace API.Models
{
    public class ComplaintLocation
    {
        public string PlaceId { get; set; }
        public List<SimpleComplaint> Complaints { get; set; }
    }

    public class SimpleComplaint
    {
        public string UserID { get; set; }
        public string UserEmail { get; set; }
        public DateTime TimeSubmitted { get; set; }

        public string Comment { get; set; }
        public DateTime TimeLastUpdated { get; set; }
        public string Status { get; set; }
    }
}
