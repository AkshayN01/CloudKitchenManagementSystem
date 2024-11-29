using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Interfaces.Queue
{
    public interface IRabbitMQ
    {
        Task<IConnection?> GetConnectionAsync();
        Task<IChannel?> GetChannelAsync();
    }
}
