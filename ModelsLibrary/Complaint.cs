using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsLibrary
{
    public class Complaint
    {
        public string UserID { get; set; }
        public string PlaceID { get; set; }
        public DateTime TimeSubmitted { get; set; }

        public string Comment { get; set; }
        public DateTime TimeLastUpdated { get; set; }
        public string Status { get; set; }
    }
}
