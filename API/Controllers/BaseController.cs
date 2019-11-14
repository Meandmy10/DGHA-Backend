using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace API.Controllers
{
    public abstract class BaseController : AuthorizedController
    {
        private readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        protected async Task UpdateLocationStatus(string placeId)
        {
            if (!await _context.Reviews.AnyAsync(review => review.PlaceID == placeId).ConfigureAwait(false) &&
                !await _context.Complaints.AnyAsync(complaint => complaint.PlaceID == placeId).ConfigureAwait(false))
            {
                var location = await _context.Locations.FindAsync(placeId)
                                                       .ConfigureAwait(false);

                _context.Locations.Remove(location);

                await _context.SaveChangesAsync()
                              .ConfigureAwait(false);
            }
        }

        //unsurpress these warnings when impimented
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable CA1822 // Mark members as static
        protected async Task<bool> PlaceExists(string placeId)
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (placeId == null) return false;
            throw new NotImplementedException();
            //uncomment instances when impimented (in reviews controller & complaints controller, start of post requests)
        }

        protected async Task<bool> UserExists(string userId)
        {
            if (userId == null) return false;
            return await _context.Users.AnyAsync(user => user.Id == userId).ConfigureAwait(false);
        }
    }
}
