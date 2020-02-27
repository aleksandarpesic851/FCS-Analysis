using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_FCS_Analysis.Utilities
{
    public class WBC3Cell
    {
        public List<int> wbc_n;
        public List<int> wbc_m;
        public List<int> wbc_l;

        public WBC3Cell()
        {
            wbc_n = new List<int>();
            wbc_m = new List<int>();
            wbc_l = new List<int>();
        }
    }
}
