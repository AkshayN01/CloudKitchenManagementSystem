using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.Configuration
{
    public class Application
    {
        public Connection connection { get; set; }
    }
    public class Connection
    {
        public Redis redis { get; set; }
        public SendGridOptions sendGridOptions { get; set; }
        public RabbitMqConfiguration rabbitMqConfiguration { get; set; }
    }
    public class SendGridOptions
    {
        public String APIKey { get; set; }
        public String FromEmail { get; set; }
        public String FromName { get; set; }
    }
    public class Redis
    {
        public String server { get; set; }
        public Int32 port { get; set; }
        public bool apply { get; set; }
    }
    public class RabbitMqConfiguration
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
