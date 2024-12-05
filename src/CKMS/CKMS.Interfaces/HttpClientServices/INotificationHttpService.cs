using CKMS.Contracts.DBModels.NotificationService;
using CKMS.Contracts.DTOs.Notification.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Interfaces.HttpClientServices
{
    public interface INotificationHttpService
    {
        Task<bool> SendNotification(List<NotificationPayload> payload);
    }
}
