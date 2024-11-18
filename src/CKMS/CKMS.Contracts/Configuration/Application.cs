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
        public String environment { get; set; }
        public Redis redis { get; set; }
        public Postgre postgre { get; set; }
    }
    public class Redis
    {
        public String server { get; set; }
        public Int32 port { get; set; }
        public bool apply { get; set; }
    }
    public class Postgre
    {
        public String host { get; set; }
        public String readOnlyHost { get; set; }
        public String masterHost { get; set; }
        public String readOnlyMasterHost { get; set; }
        public bool pooling { get; set; }
        public Int32 minPoolSize { get; set; }
        public Int32 maxPoolSize { get; set; }
        public String schema { get; set; }
        public String schemaRank { get; set; }
        public String schemaAchievement { get; set; }
    }
}
