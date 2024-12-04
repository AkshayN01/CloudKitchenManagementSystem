using CKMS.Contracts.DBModels.NotificationService;
using CKMS.Interfaces.UserNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Library.UserNotification
{
    public class NotificationHandler
    {
        private readonly IEnumerable<IUserNotification> _userNotifications;
        public NotificationHandler(IEnumerable<IUserNotification> userNotifications)
        {
            _userNotifications = userNotifications;
        }

        public async Task SendNotificationAsync(NotificationMessage notificationMessage)
        {
            if (notificationMessage == null) 
                throw new ArgumentNullException(nameof(notificationMessage));

            var handlers = _userNotifications.Where(x => notificationMessage.NotificationTypes.Contains(x.UserNotificationType));
            if (handlers != null && handlers.Any())
            {
                foreach( var handler in handlers) 
                {
                    await handler.SendNotificationAsync(notificationMessage);
                }
            }
            else
                throw new InvalidOperationException($"No handler found for notification type: {notificationMessage.NotificationTypes}");
        }
    }
}
