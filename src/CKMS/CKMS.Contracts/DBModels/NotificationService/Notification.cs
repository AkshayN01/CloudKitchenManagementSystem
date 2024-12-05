using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CKMS.Contracts.DBModels.NotificationService
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }
        public int UserType { get; set; }
        public Guid UserId { get; set; }
        public String Recipient {  get; set; }
        public String Title { get; set; }
        public String Body { get; set; }
        public int NotificationType { get; set; }
        public int IsSent { get; set; }
        public DateTime DateTime {  get; set; }
    }
    public class NotificationMessage
    {
        public int UserType { get; set; }
        public string Recipient { get; set; } = String.Empty!; // Email, UserId, or other identifier
        public string Subject { get; set; } = String.Empty!; // Optional for non-email notifications
        public string Body { get; set; } = String.Empty!;
        public int NotificationType { get; set; } // e.g., "Email", "Web", "MobileApp"
    }
    public class NotificationAudit : AuditTable
    {
        public Guid NotificationId { get; set; }
    }
    public enum NotificationUserType
    {
        Admin = 1,
        Customer = 2
    }
    public enum NotificationType
    {
        Email = 1,
        Browser = 2,
        App = 3
    }
    //notification scenarios
    //user gets updated on their order
    //user gets a verification email
    //admin gets a new order
    //admin gets update on their order
    //admin gets a verification email
}
