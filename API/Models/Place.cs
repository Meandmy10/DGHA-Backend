using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA2227 // Collection properties should be read only


namespace API.Models
{
    public class Place
    {
        public string PlaceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<string> Types { get; set; }

        public Place(string placeId, string name, string address, List<string> types)
        {
            this.PlaceId = placeId;
            this.Name = name;
            this.Address = address;
            this.Types = types;
        }
    }
}
