using System.Collections.Generic;
using System.Linq;
using API.Models;
using ModelsLibrary;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1031

public class CreatePlace {
    public static Place setPlaceDetails (Place place, IdPlaceResult idPlace, SearchPlaceResult searchPlace) {
        place.PlaceId = searchPlace != null ? searchPlace.place_id : idPlace.place_id; 
        place.Name = searchPlace != null ? searchPlace.name : idPlace.name; 
        place.Address = searchPlace != null ? formatAddress(searchPlace.formatted_address) : formatAddress(idPlace.formatted_address);
        place.Types = searchPlace != null ? searchPlace.types : idPlace.types;
        place.State = idPlace != null ? idPlace.address_components[idPlace.address_components.Count - 3].long_name : null;
        return place;
    }

    private static string formatAddress(string input) {
        string[] states = new string[] {"QLD", "NSW", "ACT", "VIC", "SA", "WA", "TAS", "NT"};

        for(int i = 0; i < states.Length; i++) {
            if(input.Contains(states[i])) {
                int indexOfState = input.IndexOf(states[i]);
                input = input.Substring(0, indexOfState - 1); 
            }
        }

        return input; 
    }
}