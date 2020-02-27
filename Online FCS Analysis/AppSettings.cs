using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_FCS_Analysis
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public DefaultGateSetting defaultGateSetting { get; set; }
    }

    public class MysqlConnectionSettings
    {
        public string server { get; set; }
        public string database { get; set; }
        public string user { get; set; }
        public string password { get; set; }
    }

    public class DefaultGateSetting
    {
        public string path { get; set; }
        public string gate1 { get; set; }
        public string gate2 { get; set; }
        public string gate3 { get; set; }
        public string gateEos { get; set; }
        public string gateBaso { get; set; }
        public string gateBasoClusterCenter { get; set; }
    }
}
