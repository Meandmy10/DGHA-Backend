using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelsLibrary;
using System.Security.Claims;
using System.Security.Principal;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace API.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AccountsController : ControllerBase
    {

        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountsController> _logger;
        //private readonly IEmailSender _emailSender;

        public AccountsController(
            UserManager<User> userManager,
            ILogger<AccountsController> logger
            //IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _logger = logger;
            //_emailSender = emailSender;
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets All User Accounts
        /// </summary>
        /// <returns>All User Accounts</returns>
        /// <response code="200">Returns All User Accounts</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [Authorize(Roles = "Administrator")]
        public async Task<IEnumerable<BasicUser>> GetAccounts()
        {
            return await _userManager.Users.Select(user => new BasicUser(user))
                                           .ToListAsync()
                                           .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets Requested User Account
        /// </summary>
        /// <param name="userId">Account Id</param>
        /// <returns>Requested Account</returns>
        /// <response code="200">Returns Specified User Accounts</response>
        /// <response code="404">Account Not Found</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BasicUser>> GetAccount(string userId)
        {
            if (!HasOwnedDataAccess(userId))
            {
                return Forbid();
            }

            BasicUser user = await _userManager.FindByIdAsync(userId)
                                               .ConfigureAwait(false);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return user;
            }
        }

        /// <summary>
        /// Posts New User Account
        /// </summary>
        /// <param name="newUser">New User</param>
        /// <returns>Task Result</returns>
        /// <response code="201">Returns Created User Account</response>
        /// <response code="400">Returns Request Error</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        public async Task<ActionResult<BasicUser>> PostAccount(NewUser newUser)
        {
            if (newUser == null)
            {
                return BadRequest("New user not defined");
            }

            var user = new User { UserName = newUser.Email, Email = newUser.Email };
            var result = await _userManager.CreateAsync(user, newUser.Password)
                                           .ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            _logger.LogInformation("User created a new account with password.");

            //this code can be modified to use email confirmation
            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //var callbackUrl = Url.Page(
            //    "/Account/ConfirmEmail",
            //    pageHandler: null,
            //    values: new { area = "Identity", userId = user.userId, code = code },
            //    protocol: Request.Scheme);

            //await _emailSender.SendEmailAsync(newUser.Email, "Confirm your email",
            //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            //if (_userManager.Options.SignIn.RequireConfirmedAccount)
            //{
            //    return RedirectToPage("RegisterConfirmation", new { email = newUser.Email });
            //}
            //else
            //{
            //    await _signInManager.SignInAsync(user, isPersistent: false);
            //    return LocalRedirect(returnUrl);
            //}

            BasicUser createdUser = await _userManager.FindByNameAsync(user.UserName)
                                                      .ConfigureAwait(false);

            return CreatedAtAction("GetAccount", new { userId = createdUser.Id }, createdUser);
        }

        /// <summary>
        /// Puts Updated User Account
        /// NOTE: Not Impimented Yet
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="editedUser">Edited User</param>
        /// <returns></returns>
        [HttpPut("{userId}")]
        public async Task<IActionResult> PutAccount(string userId, User editedUser)
        {
            if (!HasOwnedDataAccess(userId))
            {
                return Forbid();
            }

            if (editedUser == null || editedUser.Id != userId)
            {
                return BadRequest();
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes Specified User Account
        /// NOTE: Currently does not handle user delete requests with users that have reviews on db
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Deleted user</returns>
        /// <response code="200">Returns Deleted User Account</response>
        /// <response code="404">Returns Request Error</response>
        [HttpDelete("{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BasicUser>> DeleteAccount(string userId)
        {
            if (!HasOwnedDataAccess(userId))
            {
                return Forbid();
            }

            var user = await _userManager.FindByIdAsync(userId)
                                         .ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user)
                              .ConfigureAwait(false);

            BasicUser basicUser = user;

            return basicUser;
        }

        /// <summary>
        /// Posts new role to user account
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="roleName">Role Name</param>
        /// <returns>Action Result</returns>
        /// <response code="204">Role added successfully</response>
        /// <response code="400">Returns Request Error</response>
        /// <response code="404">Returns Request Error</response>
        [HttpPost("{userId}/{roleName}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> PostUserRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId)
                                         .ConfigureAwait(false);

            if(user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName)
                                           .ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes role from user account
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="roleName">Role Name</param>
        /// <returns>Action Result</returns>
        /// <response code="204">Role removed successfully</response>
        /// <response code="400">Returns Request Error</response>
        /// <response code="404">Returns Request Error</response>
        [HttpDelete("{userId}/{roleName}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteUserRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId)
                                         .ConfigureAwait(false);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName)
                                           .ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        private bool HasOwnedDataAccess(string userId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim.Value == userId)
            {
                return true;
            }
            else
            {
                var roleClaims = claimsIdentity.FindAll(ClaimTypes.Role);
                if(roleClaims.Any(claim => claim.Value == "Administrator"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
