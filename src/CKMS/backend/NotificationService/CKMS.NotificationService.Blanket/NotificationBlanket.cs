using CKMS.Contracts.DBModels.AdminUserService;
using CKMS.Contracts.DTOs.AdminUser.Request;
using CKMS.Contracts.DTOs.AdminUser.Response;
using CKMS.Contracts.DTOs;
using CKMS.Interfaces.Repository;
using CKMS.Library.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CKMS.Contracts.DTOs.Notification.Request;
using CKMS.Contracts.DBModels.NotificationService;
using CKMS.Library.UserNotification;

namespace CKMS.NotificationService.Blanket
{
    public class NotificationBlanket
    {
        private readonly NotificationHandler _handler;
        private readonly INotificationUnitOfWork _notificationUnitOfWork;
        public NotificationBlanket(INotificationUnitOfWork notificationUnitOfWork, NotificationHandler notificationHandler) 
        {
            _handler = notificationHandler;
            _notificationUnitOfWork = notificationUnitOfWork;
        }

        public async Task<HTTPResponse> SendNotification(List<NotificationPayload> payload)
        {
            if (payload == null)
                return APIResponse.ConstructExceptionResponse(-40, "Payload is missing");

            int retVal = -40;
            string message = string.Empty;
            Object? data = default(Object?);

            try
            {
                //check if Kitchen Id is valid or not
                if (payload == null)
                    return APIResponse.ConstructExceptionResponse(-40, "Payload is missing");

                foreach(NotificationPayload notificationPayload in payload)
                {
                    Notification notification = new Notification()
                    {
                        Id = new Guid(),
                        DateTime = DateTime.UtcNow,
                        IsSent = 0,
                        NotificationType = notificationPayload.NotificationType,
                        UserId = new Guid(notificationPayload.UserId),
                        Recipient = notificationPayload.Recipient,
                        UserType = notificationPayload.UserType,
                        Body = notificationPayload.Message,
                        Title = notificationPayload.Title,
                    };

                    NotificationMessage notificationMessage = new NotificationMessage()
                    {
                        Body = notificationPayload.Message,
                        NotificationType = notification.NotificationType,
                        Recipient = notificationPayload.Recipient,
                        Subject = notificationPayload.Title,
                        UserType = notificationPayload.UserType,
                    };
                    bool isSent = await _handler.SendNotificationAsync(notificationMessage);
                    if(isSent)
                    {
                        notification.IsSent = 1;
                    }
                    else
                    {
                        String json = await Utility.SerialiseData<Notification>(notification);
                        NotificationAudit notificationAudit = new NotificationAudit()
                        {
                            CreatedAt = DateTime.UtcNow,
                            HTTPStatus = 0,
                            NotificationId = notification.Id,
                            Payload = json
                        };
                        await _notificationUnitOfWork.NotificationAuditRepository.AddAsync(notificationAudit);
                    }
                    await _notificationUnitOfWork.NotificationRepository.AddAsync(notification);
                }
                await _notificationUnitOfWork.CompleteAsync();

                data = true;
                retVal = 1;
            }
            catch (Exception ex)
            {
                return Library.Generic.APIResponse.ConstructExceptionResponse(-40, ex.Message);
            }

            return Library.Generic.APIResponse.ConstructHTTPResponse(data, retVal, message);
        }
    }
}
