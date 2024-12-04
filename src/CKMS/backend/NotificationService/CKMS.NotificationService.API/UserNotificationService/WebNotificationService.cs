using CKMS.Contracts.DBModels.CustomerService;
using CKMS.Contracts.DBModels.NotificationService;
using CKMS.Interfaces.UserNotification;
using CKMS.NotificationService.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CKMS.NotificationService.API.UserNotificationService
{
    public class WebNotificationService : IUserNotification
    {
        private readonly IHubContext<AdminNotificationHub> _adminHubContext;
        private readonly IHubContext<CustomerNotificationHub> _customerHubContext;
        public WebNotificationService(IHubContext<AdminNotificationHub> adminHubContext, IHubContext<CustomerNotificationHub> customerHubContext) 
        {
            _adminHubContext = adminHubContext;
            _customerHubContext = customerHubContext;
        }
        public int UserNotificationType => (int)NotificationType.Browser;

        public async Task<bool> SendNotificationAsync(NotificationMessage notificationMessage)
        {
            bool sent = false;

            if(notificationMessage == null) 
                throw new ArgumentNullException(nameof(notificationMessage));

            if(notificationMessage.UserType == (int)NotificationUserType.Admin)
            {
                await _adminHubContext.Clients.Group(notificationMessage.Recipient)
                    .SendAsync("ReceiveNotification", notificationMessage.Subject, notificationMessage.Body);
                sent = true;
            }
            else if(notificationMessage.UserType == (int) NotificationUserType.Customer)
            {
                await _customerHubContext.Clients.Group(notificationMessage.Recipient)
                    .SendAsync("ReceiveNotification", notificationMessage.Subject, notificationMessage.Body);
                sent = true;
            }

            return sent;
        }
    }
}
