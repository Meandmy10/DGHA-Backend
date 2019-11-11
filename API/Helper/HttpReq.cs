using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using API.Models;
using Newtonsoft.Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1031

public class HttpReq {
    private static string apiKey = "AIzaSyCj1t28RYFPFpa1xL_kjlIwCrH8CEryoJs";
    public static async Task<Result> getPlaceByIdFromGoogle (string id) {
        string url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={id}&fields=place_id,types,name,address_components,formatted_address&key={HttpReq.apiKey}";

        try {
            using (HttpClient client = new HttpClient ()) {
                using (HttpResponseMessage res = await client.GetAsync (url)) {
                    using (HttpContent content = res.Content) {
                        var rawData = await content.ReadAsStringAsync ().ConfigureAwait (true);
                        var dataObj = JsonConvert.DeserializeObject<PlaceIdQueryResponse> (rawData);
                        var result = dataObj.result;

                        return result;
                    }
                }
            }
        } catch (Exception e) {
            Console.WriteLine ("and i oop- " + e);
            return null;
        }
    }

    public static async Task<List<Results>> getPlaceByTextFromGoogle(string query) {
        string url =$"https://maps.googleapis.com/maps/api/place/textsearch/json?query={query}&fields=place_id&key={HttpReq.apiKey}";

        try {
            using (HttpClient client = new HttpClient ()) {
                using (HttpResponseMessage res = await client.GetAsync (url)) {
                    using (HttpContent content = res.Content) {
                        var rawData = await content.ReadAsStringAsync ().ConfigureAwait (true);
                        var dataObj = JsonConvert.DeserializeObject<PlaceAPIQueryResponse> (rawData);
                        var result = dataObj.results;

                        return result;
                    }
                }
            }
        } catch (Exception e) {
            Console.WriteLine ("and i oop- " + e);
            return null;
        }
    }
}