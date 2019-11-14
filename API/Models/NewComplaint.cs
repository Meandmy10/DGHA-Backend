using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModelsLibrary;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace API.Models
{
    public class NewComplaint
    {
        public NewComplaint()
        {
        }

        public NewComplaint(Complaint complaint)
        {
            if (complaint == null) throw new ArgumentNullException(nameof(complaint));
            UserID = complaint.UserID;
            PlaceID = complaint.PlaceID;
            Comment = complaint.Comment;
        }

        public string UserID { get; set; }
        public string PlaceID { get; set; }
        public string Comment { get; set; }

        public static implicit operator NewComplaint(Complaint v)
        {
            return new NewComplaint(v);
        }
    }
}
