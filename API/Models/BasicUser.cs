using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModelsLibrary;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace API.Models
{
    public class BasicUser
    {
        public BasicUser(User user)
        {
            Id = user.Id;
            Email = user.Email;
        }

        public string Id { get; set; }
        public string Email { get; set; }

        public static implicit operator BasicUser(User v) => new BasicUser(v);
    }
}
