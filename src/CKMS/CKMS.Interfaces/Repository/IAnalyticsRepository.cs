using CKMS.Contracts.DBModels.AnalyticsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Interfaces.Repository
{
    public interface IOrderAnalyticsRepository : IGenericRepository<AggregatedOrder>
    {
        Task<List<AggregatedOrder>?> GetByOrderId(Guid OrderId);
    }
    //public interface IDiscountAnalyticsRepository : IGenericRepository<Dis>
}
