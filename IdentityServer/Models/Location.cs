using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Models
{
    public class Location
    {
        public string PlaceID { get; set; }
        
        public virtual ICollection<Review> Reviews { get; private set; }
    }
}
