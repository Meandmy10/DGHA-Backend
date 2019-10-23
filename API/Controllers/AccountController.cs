using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelsLibrary;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;
        //public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        // GET: Account
        [HttpGet]
        public async Task<IEnumerable<User>> GetAccounts()
        {
            return await _userManager.Users.ToListAsync()
                                           .ConfigureAwait(false);
        }

        // GET: Account/5
        [HttpGet("{id}")]
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

        // POST: Account
        [HttpPost]
        public async Task<IActionResult> PostAccount(NewUser newUser)
        {
            if (newUser == null)
            {
                return BadRequest();
            }

            var user = new User { UserName = newUser.Email, Email = newUser.Email };
            var result = await _userManager.CreateAsync(user, newUser.Password)
                                            .ConfigureAwait(false);

            if (result.Succeeded)
            {
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

                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        // PUT: Account/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(string id, User editedUser)
        {
            if(editedUser == null || editedUser.Id != id)
            {
                return BadRequest();
            }

            throw new NotImplementedException();
        }

        // DELETE: Account/5
        [HttpDelete("{id}")]
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
