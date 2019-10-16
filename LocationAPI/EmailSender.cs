using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationAPI
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            //this needs to be implimented at some point if we want to use email confirmation
            return Task.CompletedTask;
        }
    }
}
