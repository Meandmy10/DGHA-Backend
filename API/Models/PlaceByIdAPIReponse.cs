using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CA1707 // Identifiers should not contain underscores

namespace API.Models
{
    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; }
    }

    public class IdPlaceResult
    {
        public List<AddressComponent> address_components { get; }
        public string formatted_address { get; set; }
        public string name { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; }
    }

    public class PlaceIdQueryResponse
    {
        public List<object> html_attributions { get; }
        public IdPlaceResult result { get; set; }
        public string status { get; set; }
    }
}