using FlowCytometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_FCS_Analysis.Utilities
{
    [Serializable]
    public class GatePolygon
    {
        public string gateName { get; set; }
        public string channel1 { get; set; }
        public string channel2 { get; set; }
        public List<List<SPoint>> polygons { get; set; }
    }
    [Serializable]
    public class SPoint
    {
        public int x { get; set; }
        public int y { get; set; }
    }
}
