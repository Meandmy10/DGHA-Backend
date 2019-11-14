using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ComplaintsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ComplaintsController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets all unresolved complaints devided in complaintLocation objects
        /// </summary>
        /// <returns>All unresolved complaints devided in complaintLocation objects</returns>
        /// <response code="200">Returns All unresolved complaints devided in complaintLocation objects</response>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<ComplaintLocation>>> GetComplaints()
        {
            var complaints = await _context.Complaints
                .Join(_context.Users,
                c => c.UserID,
                u => u.Id,
                (c, u) => new { Complaint = c, User = u })
                .Where(cu => cu.Complaint.UserID == cu.User.Id)
                .Where(cu => cu.Complaint.Status != "Resolved")
                .OrderBy(cu => cu.Complaint.PlaceID)
                .ThenByDescending(cu => cu.Complaint.TimeLastUpdated)
                .ThenByDescending(cu => cu.Complaint.TimeSubmitted)
                //.ThenBy(cu => cu.Complaint.Status)
                .ToListAsync()
                .ConfigureAwait(false);

            string location = complaints.First().Complaint.PlaceID;
            int locationNo = 0;

            List<ComplaintLocation> complaintLocations = new List<ComplaintLocation>()
            {
                {
                    new ComplaintLocation()
                    {
                        PlaceId = location,
                        Complaints = new List<SimpleComplaint>()
                        {
                            new SimpleComplaint()
                            {
                                UserID = complaints.First().User.Id,
                                UserEmail = complaints.First().User.Email,
                                TimeSubmitted = complaints.First().Complaint.TimeSubmitted,

                                Comment = complaints.First().Complaint.Comment,
                                TimeLastUpdated = complaints.First().Complaint.TimeLastUpdated,
                                Status = complaints.First().Complaint.Status
                            }
                        }
                    }
                }
            };

            for (int i = 1; i < complaints.Count; i++)
            {
                if (complaints[i].Complaint.PlaceID == location)
                {
                    //add to existing list
                    complaintLocations[locationNo].Complaints.Add(new SimpleComplaint()
                    {
                        UserID = complaints[i].User.Id,
                        UserEmail = complaints[i].User.Email,
                        TimeSubmitted = complaints[i].Complaint.TimeSubmitted,

                        Comment = complaints[i].Complaint.Comment,
                        TimeLastUpdated = complaints[i].Complaint.TimeLastUpdated,
                        Status = complaints[i].Complaint.Status
                    });
                }
                else
                {
                    //create new location
                    complaintLocations.Add(new ComplaintLocation()
                    {
                        PlaceId = complaints[i].Complaint.PlaceID,
                        Complaints = new List<SimpleComplaint>()
                        {
                            new SimpleComplaint()
                            {
                                UserID = complaints[i].User.Id,
                                UserEmail = complaints[i].User.Email,
                                TimeSubmitted = complaints[i].Complaint.TimeSubmitted,

                                Comment = complaints[i].Complaint.Comment,
                                TimeLastUpdated = complaints[i].Complaint.TimeLastUpdated,
                                Status = complaints[i].Complaint.Status
                            }
                        }
                    });

                    location = complaints[i].Complaint.PlaceID;
                    locationNo++;
                }
            }

            return complaintLocations;
        }

        /// <summary>
        /// Gets all resolved complaints
        /// </summary>
        /// <returns>All resolved complaints</returns>
        /// <response code="200">Returns All resolved complaints in ComplaintsLocaiton Objects</response>
        [HttpGet("Resolved")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Complaint>>> GetResolvedComplaints()
        {
            return await _context.Complaints.Where(complaint => complaint.Status == "Resolved")
                                            .OrderByDescending(complaint => complaint.TimeLastUpdated)
                                            .ToListAsync()
                                            .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets Specified Complaint
        /// </summary>
        /// <param name="placeId">Place Id of complaint's place</param>
        /// <param name="userId">User Id of complaint's user</param>
        /// <param name="timeSubmitted">Time when compaint submitted</param>
        /// <returns>Requested Complaint</returns>
        /// <response code="200">Returns requested complaint</response>
        /// <response code="404">No complaint Found</response>
        [HttpGet("{placeId}/{userId}/{timeSubmitted}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Complaint>> GetComplaint(string placeId, string userId, DateTime timeSubmitted)
        {
            Complaint complaint = await _context.Complaints.FindAsync(placeId, userId, timeSubmitted);

            if (complaint == null)
            {
                return NotFound();
            }

            return complaint;
        }

        /// <summary>
        /// Posts new complaint
        /// </summary>
        /// <param name="newComplaint">Complaint to add</param>
        /// <returns>Added Complaint</returns>
        /// <response code="201">Returns Posted Review</response>
        /// <response code="400">Bad Request</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Complaint>> PostComplaint(NewComplaint newComplaint)
        {
            if (newComplaint == null || newComplaint.PlaceID == null || newComplaint.UserID == null)
            {
                return BadRequest("Invalid Input");
            }

            //if (!await PlaceExists(newComplaint.PlaceID).ConfigureAwait(false))
            //{
            //    return BadRequest("Place doesn't exist");
            //}

            if (!HasOwnedDataAccess(newComplaint.UserID))
            {
                return Forbid();
            }

            //if location doesn't exist, add location to db
            if (!await _context.Locations.AnyAsync(e => e.PlaceID == newComplaint.PlaceID).ConfigureAwait(false))
            {
                var location = new Location(newComplaint.PlaceID);
                _context.Locations.Add(location);
            }

            _context.Complaints.Add(new Complaint 
            { 
                PlaceID = newComplaint.PlaceID, 
                UserID = newComplaint.UserID, 
                Comment = newComplaint.Comment 
            });

            DateTime timeBeforeSave = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync()
                              .ConfigureAwait(false);
            }
            catch (DbUpdateException e)
            {
                if (!await UserExists(newComplaint.UserID).ConfigureAwait(false))
                {
                    return BadRequest(e.InnerException.Message);
                }
                else
                {
                    throw;
                }
            }

            Complaint createdComplaint = await _context.Complaints.Where(c =>
                                                                   c.UserID == newComplaint.UserID &&
                                                                   c.PlaceID == newComplaint.PlaceID)
                                                                  .OrderByDescending(c => c.TimeSubmitted)
                                                                  .FirstAsync()
                                                                  .ConfigureAwait(false);

            return CreatedAtAction("GetComplaint", new { 
                placeId = createdComplaint.PlaceID, 
                userId = createdComplaint.UserID, 
                timeSubmitted = createdComplaint.TimeSubmitted 
            }, 
            createdComplaint);
        }

        /// <summary>
        /// Puts updated complaint
        /// </summary>
        /// <param name="placeId">Complaints place id</param>
        /// <param name="userId">Complaints user id</param>
        /// <param name="timeSubmitted">Time complaint was submitted</param>
        /// <param name="complaint">Updated complaint</param>
        /// <returns>Action result</returns>
        /// <response code="204">Complaint Successfully Updated</response>
        /// <response code="400">Invalid Input</response>
        /// <response code="404">Complaint Doesn't Exist</response>
        [HttpPut("{placeId}/{userId}/{timeSubmitted}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutComplaint(string placeId, string userId, DateTime timeSubmitted, Complaint complaint)
        {
            if (complaint == null || placeId != complaint.PlaceID || userId != complaint.UserID || timeSubmitted != complaint.TimeSubmitted)
            {
                return BadRequest("Invalid Input");
            }

            _context.Entry(complaint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync()
                              .ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ComplaintExists(placeId, userId, timeSubmitted).ConfigureAwait(false))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Delete specified Complaint
        /// </summary>
        /// <param name="placeId">Place Id for Complaint</param>
        /// <param name="userId">User Id for Complaint</param>
        /// <param name="timeSubmitted">Time Complaint Submitted</param>
        /// <returns>Deleted Complaint</returns>
        /// <response code="200">Returns Deleted Complaint</response>
        /// <response code="404">Complaint Not Found</response>
        [HttpDelete("{placeId}/{userId}/{timeSubmitted}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Complaint>> DeleteComplaint(string placeId, string userId, DateTime timeSubmitted)
        {
            var complaint = await _context.Complaints.FindAsync(placeId, userId, timeSubmitted);

            if (complaint == null)
            {
                return NotFound();
            }

            _context.Complaints.Remove(complaint);

            await _context.SaveChangesAsync()
                          .ConfigureAwait(false);

            await UpdateLocationStatus(placeId).ConfigureAwait(false);

            return complaint;
        }

        private async Task<bool> ComplaintExists(string placeId, string userId, DateTime timeSubmitted)
        {
            return await _context.Complaints.AnyAsync(e => e.PlaceID == placeId && e.UserID == userId && e.TimeSubmitted == timeSubmitted)
                                            .ConfigureAwait(false);
        }
    }
}
