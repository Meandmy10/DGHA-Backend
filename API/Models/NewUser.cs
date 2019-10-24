using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace API.Models
{
    public class NewUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
