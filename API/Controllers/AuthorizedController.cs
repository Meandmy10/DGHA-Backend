using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace API.Controllers
{
    public class AuthorizedController : ControllerBase
    {
        protected bool HasOwnedDataAccess(string userId)
        {
            //honestly not the best impimentation of this, but i could of spent a week trying to work out how to make an authentication policy.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim.Value == userId)
            {
                return true;
            }
            else
            {
                var roleClaims = claimsIdentity.FindAll(ClaimTypes.Role);
                if (roleClaims.Any(claim => claim.Value == "Administrator"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
