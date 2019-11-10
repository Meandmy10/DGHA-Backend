using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using API.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1031

namespace API.Controllers
{
    [ApiController]
    [Route("search")]
    public class SearchController : Controller
    {
        private string apiKey = "AIzaSyCj1t28RYFPFpa1xL_kjlIwCrH8CEryoJs";
        [HttpGet]
        public async IAsyncEnumerable<List<Place>> getSearch(string query)
        {
            string url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={query}&key={apiKey}";
            List<Place> places = new List<Place>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(url))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var data = await content.ReadAsStringAsync().ConfigureAwait(true);
                            var dataObj = JsonConvert.DeserializeObject<PlaceAPIQueryResponse>(data);
                            var results = dataObj.results;

                            foreach (var item in results)
                            {
                                Place p = new Place(placeId: item.place_id, name: item.name, address: item.formatted_address, types: item.types);
                                places.Add(p);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("and i oop- " + e);
            }

            yield return places;
        }
    }
}