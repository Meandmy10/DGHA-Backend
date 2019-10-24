using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelsLibrary;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace API.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountsController> _logger;
        private readonly IEmailSender _emailSender;
        //public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public AccountsController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountsController> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets All User Accounts
        /// </summary>
        /// <returns>All User Accounts</returns>
        /// <response code="200">Returns All User Accounts</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IEnumerable<User>> GetAccounts()
        {
            return await _userManager.Users.ToListAsync()
                                           .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets Requested User Account
        /// </summary>
        /// <param name="id">Account Id</param>
        /// <returns>Requested Account</returns>
        /// <response code="200">Returns Specified User Accounts</response>
        /// <response code="404">Account Not Found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<User>> GetAccount(string id)
        {
            User user = await _userManager.FindByIdAsync(id)
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
        public async Task<ActionResult<User>> PostAccount(NewUser newUser)
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
            //    values: new { area = "Identity", userId = user.Id, code = code },
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

            var createdUser = await _userManager.FindByNameAsync(user.UserName)
                                        .ConfigureAwait(false);

            return CreatedAtAction("GetAccount", new { createdUser.Id }, createdUser);
        }

        /// <summary>
        /// Puts Updated User Account
        /// NOTE: Not Impimented Yet
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="editedUser">Edited User</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(string id, User editedUser)
        {
            if (editedUser == null || editedUser.Id != id)
            {
                return BadRequest();
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes Specified User Account
        /// NOTE: Currently does not handle user delete requests with users that have reviews on db
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Deleted user</returns>
        /// <response code="200">Returns Deleted User Account</response>
        /// <response code="404">Returns Request Error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<User>> DeleteAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id)
                                         .ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user)
                              .ConfigureAwait(false);

            return user;
        }
    }
}
