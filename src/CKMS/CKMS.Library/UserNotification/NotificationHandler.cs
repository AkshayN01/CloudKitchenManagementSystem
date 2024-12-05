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

        public async Task<bool> SendNotificationAsync(NotificationMessage notificationMessage)
        {
            if (notificationMessage == null) 
                throw new ArgumentNullException(nameof(notificationMessage));

            var handler = _userNotifications.FirstOrDefault(x => notificationMessage.NotificationType == x.UserNotificationType);
            if (handler != null)
            {
                return await handler.SendNotificationAsync(notificationMessage);
            }
            else
                throw new InvalidOperationException($"No handler found for notification type: {notificationMessage.NotificationType}");
        }
    }
}
