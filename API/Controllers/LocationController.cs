using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Location>> GetLocations(string searchTerm, string location = "Australia")
        {
            throw new NotImplementedException();
        }

        [HttpGet("{placeId}")]
        public async Task<Location> GetLocation(string placeId)
        {

            throw new NotImplementedException();
        }
    }
}