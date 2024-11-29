using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.DBModels.AnalyticsService
{
    //Warehouse Analytics Table
    public class AggregatedOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        [Required]
        public Guid OrderId { get; set; }
        [Required]
        public String CustomerName { get; set; }
        [Required]
        public String KitchenName { get; set; }
        [Required]
        public String MenuItemName { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public Double NetAmount { get; set; }
        [Required]
        public Double GrossAmount { get; set; }

    }
}
