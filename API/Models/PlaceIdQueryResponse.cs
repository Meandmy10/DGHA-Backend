using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable

namespace API.Models.APIResponse {
    public class AddressComponent {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class Result {
        public List<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public string name { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
    }

    public class PlaceIdQueryResponse {
        public List<object> html_attributions { get; set; }
        public Result result { get; set; }
        public string status { get; set; }
    }

    public enum PlaceQueryType {
        SearchById = 0,
        SearchByText = 1
    }
}