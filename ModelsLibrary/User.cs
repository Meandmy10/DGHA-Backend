using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ModelsLibrary
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class User : IdentityUser
    {
        public virtual ICollection<Review> Reviews { get; private set; }
    }
}
