using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_FCS_Analysis
{
    public class AppSettings
    {
        public MysqlConnectionSettings mysqlConnectionSetting { get; set; }
    }

    public class MysqlConnectionSettings
    {
        public string server { get; set; }
        public string database { get; set; }
        public string user { get; set; }
        public string password { get; set; }
    }
}
