using ModelsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace API.Models
{
    public class SimpleComplaint
    {
        public SimpleComplaint(UserComplaint complaint)
        {
            UserID = complaint.UserID;
            TimeSubmitted = complaint.TimeSubmitted;
            Comment = complaint.Comment;
            TimeLastUpdated = complaint.TimeLastUpdated;
            UserEmail = complaint.UserEmail;
        }

        public string UserID { get; set; }
        public string UserEmail { get; set; }
        public DateTime TimeSubmitted { get; set; }

        public string Comment { get; set; }
        public DateTime TimeLastUpdated { get; set; }
    }

    public class UserComplaint : Complaint
    {
        public string UserEmail { get; set; }
    }
}
