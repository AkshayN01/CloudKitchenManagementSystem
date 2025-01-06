using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DTOs.Notification.Request
{
    public class NotificationPayload
    {
        public int UserType { get; set; }
        public String UserId { get; set; }
        public String Recipient { get; set; }
        public int Scenario { get; set; }
        public String Title { get; set; }
        public String Message { get; set; }
        public int NotificationType { get; set; }
    }
    public enum NotificationScenario
    {
        UserVerification = 1,
        UserOrderAccepted = 2,
        UserOrderDeclined = 3,
        UserOrderInProgress = 4,
        UserOrderOutForDelivery = 5,
        UserOrderFailed = 6,
        AdminVerification = 7,
        AdminOrderPlaced = 8,
        AdminOrderCancelled = 9,
        UserOrderDelivered  =10
    }
}
