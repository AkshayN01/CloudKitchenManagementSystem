using CKMS.Contracts.Configuration;
using CKMS.Interfaces.Notification;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Library.Notification
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;
        public EmailService(IOptions<SendGridOptions> options) 
        {
            _apiKey = options.Value.APIKey;
            _fromEmail = options.Value.FromEmail;
            _fromName = options.Value.FromName;
        }
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

            var response = await client.SendEmailAsync(msg);

            return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }
    }
}
