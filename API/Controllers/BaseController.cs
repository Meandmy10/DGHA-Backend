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
        protected readonly ApplicationDbContext _context;

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

        protected async Task<bool> PlaceExists(string placeId)
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
