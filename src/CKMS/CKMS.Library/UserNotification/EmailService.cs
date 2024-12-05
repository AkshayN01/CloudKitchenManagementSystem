using CKMS.Contracts.Configuration;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CKMS.Contracts.DBModels.NotificationService;
using CKMS.Interfaces.UserNotification;

namespace CKMS.Library.UserNotification
{
    public class EmailService : IUserNotification
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public int UserNotificationType => (int)NotificationType.Email;

        public EmailService(IOptions<SendGridOptions> options) 
        {
            _apiKey = options.Value.APIKey;
            _fromEmail = options.Value.FromEmail;
            _fromName = options.Value.FromName;
        }
        public async Task<bool> SendNotificationAsync(NotificationMessage notificationMessage)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(notificationMessage.Recipient);
            var msg = MailHelper.CreateSingleEmail(from, to, notificationMessage.Subject, notificationMessage.Body, notificationMessage.Body);

            var response = await client.SendEmailAsync(msg);

            return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }
        public Contracts.DBModels.NotificationService.Notification GetNotificationPayload(int userType, int notificationScenario, int notificationType, Guid UserId)
        {
            Contracts.DBModels.NotificationService.Notification notification = new Contracts.DBModels.NotificationService.Notification();

            return notification;
        }

        //private (String, String) GetNotifcationBody(int notificationScenario)
        //{
        //    return notificationScenario switch
        //    {
        //        (int)NotificationScenario.AdminVerification => ("Verify your account", "")
        //    };
        //}

    }
}
