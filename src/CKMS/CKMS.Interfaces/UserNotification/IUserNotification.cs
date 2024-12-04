using CKMS.Contracts.DBModels.NotificationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Interfaces.UserNotification
{
    public interface IUserNotification
    {
        int UserNotificationType { get; }
        Task<bool> SendNotificationAsync(NotificationMessage notificationMessage);
    }
}
