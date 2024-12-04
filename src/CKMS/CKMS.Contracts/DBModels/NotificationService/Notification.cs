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
        public String Title { get; set; }
        public String Description { get; set; }
        public int NotificationType { get; set; }
        public int IsSent { get; set; }
        public DateTime DateTime {  get; set; }
    }
    public enum NotificationType
    {
        Email = 1,
        Browser = 2,
        App = 3
    }
}
