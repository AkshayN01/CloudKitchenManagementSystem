using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKMS.Contracts.DBModels.AdminUserService
{
    public class Kitchen
    {
        [Key]
        public Guid KitchenId { get; set; }
        public string KitchenName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        [Required]
        public String EmailId { get; set; } = string.Empty!;
        public int IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public ICollection<AdminUser> Users { get; set; } = new List<AdminUser>();
    }
    //whenever a new kitchen is registered
    public class KitchenAudit : AuditTable
    {
        public Guid KitchenId { get; set; }
    }
}
