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
        [Required]
        public String Payload { get; set; } = String.Empty!;
        public int HTTPStatus { get; set; } //enum value
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
    public enum HTTPRequestStatus
    {
        Success =1,
        Failed = 2,
        Processing = 3,
        InQueue = 4
    }
}
