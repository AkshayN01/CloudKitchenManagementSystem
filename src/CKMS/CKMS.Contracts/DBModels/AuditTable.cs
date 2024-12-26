using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DBModels
{
    public class AuditTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        public String EntityId { get; set; }
        public int EntityType {  get; set; }
        [Required]
        public String Payload { get; set; } = String.Empty!;
        public int HTTPStatus { get; set; } //enum value
        public int IsSent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
    public enum EntityType
    {
        AdminUser = 1,
        Kitchen = 2,
        Inventory = 3,
        InventoryMovement = 4,
        MenuItem = 5,
        Category = 6,
        Discount = 7,
        DiscountUsage = 8,
        Order = 9,
        Payment = 10
    }
    public enum HTTPRequestStatus
    {
        Success =1,
        Failed = 2,
        Processing = 3,
        InQueue = 4
    }
}
