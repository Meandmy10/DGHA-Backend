using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary;
using ModelsLibrary.Data;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace API.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets Locations for specified search term and location (optional)
        /// NOTE: Not Implimented Yet
        /// </summary>
        /// <param name="searchTerm">Search Term</param>
        /// <param name="location">Location (Optional)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Location>> GetLocations(string searchTerm, string location = "Australia")
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets Requested Location
        /// NOTE: Not Impimented Yet
        /// </summary>
        /// <param name="placeId">Place Id</param>
        /// <returns></returns>
        [HttpGet("{placeId}")]
        public async Task<Location> GetLocation(string placeId)
        {
            throw new NotImplementedException();
        }

        //public async Task<ActionResult<Review>> PostLocation(Location location)
        //{
        //    if (location == null)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Locations.Add(location);

        //    try
        //    {
        //        await _context.SaveChangesAsync()
        //                      .ConfigureAwait(false);
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (LocationExists(location.PlaceID))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetLocation",
        //        new
        //        {
        //            placeId = location.PlaceID
        //        },
        //        location);
        //}

        //public async Task<ActionResult<Location>> DeleteLocation(string placeId)
        //{
        //    var location = await _context.Locations.FindAsync(placeId);

        //    if (location == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Locations.Remove(location);
        //    await _context.SaveChangesAsync()
        //                  .ConfigureAwait(false);

        //    return location;
        //}

        //private bool LocationExists(string placeId)
        //{
        //    return _context.Locations.Any(e => e.PlaceID == placeId);
        //}
    }
}