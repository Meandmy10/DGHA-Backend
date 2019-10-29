using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsLibrary
{
    public class Location
    {
        public Location() { }

        public Location(string placeId)
        {
            PlaceID = placeId;
        }

        public string PlaceID { get; set; }
        //Note: if you add any more properties in here ensure you igore them in the applicaitonDbContext so they arn't added to the database

        public virtual ICollection<Review> Reviews { get; private set; }
        public virtual ICollection<Complaint> Complaints { get; private set; }
    }
}
