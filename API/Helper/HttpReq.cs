using System;
using System.Net.Http;
using System.Threading.Tasks;
using API.Models;
using Newtonsoft.Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class HttpReq
{
    private const string apiKey = "AIzaSyCj1t28RYFPFpa1xL_kjlIwCrH8CEryoJs";

    public static async Task<IdPlaceResult> GetPlaceByIdFromGoogle(string id)
    {
        Uri url = new Uri($"https://maps.googleapis.com/maps/api/place/details/json?place_id={id}&fields=place_id,types,name,address_components,formatted_address&key={apiKey}");

        try
        {
            using HttpClient client = new HttpClient();
            using HttpResponseMessage res = await client.GetAsync(url).ConfigureAwait(false);
            using HttpContent content = res.Content;

            string rawData = await content.ReadAsStringAsync().ConfigureAwait(true);
            PlaceIdQueryResponse dataObj = JsonConvert.DeserializeObject<PlaceIdQueryResponse>(rawData);
            IdPlaceResult result = dataObj.result;

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine ("and i oop- " + e);
            throw;
        }
    }

    public static async Task<PlaceAPIQueryResponse> GetPlaceByTextFromGoogle(string query, string nextPageToken)
    {
        Uri url = new Uri($"https://maps.googleapis.com/maps/api/place/textsearch/json?query={query}&fields=place_id&key={apiKey}&pagetoken={nextPageToken}");

        try
        {
            using HttpClient client = new HttpClient();
            using HttpResponseMessage res = await client.GetAsync(url).ConfigureAwait(false);
            using HttpContent content = res.Content;

            string rawData = await content.ReadAsStringAsync().ConfigureAwait(true);
            PlaceAPIQueryResponse dataObj = JsonConvert.DeserializeObject<PlaceAPIQueryResponse>(rawData);

            return dataObj;
        }
        catch (Exception e)
        {
            Console.WriteLine("and i oop- " + e);
            throw;
        }
    }
}