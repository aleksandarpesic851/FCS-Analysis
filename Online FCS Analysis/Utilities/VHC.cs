using FlowCytometry.Mie_Scatter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Online_FCS_Analysis.Utilities
{
    public class VHC
    {
        public static List<Record> arrVHC = ReadRecords();
        public static List<Record> ReadRecords()
        {
            string strPath = Path.Combine(Constants.wwwroot_abs_path, Constants.vhc_path);
            List<Record> arrRecords = new List<Record>();
            arrRecords.Clear();
            try
            {
                using (StreamReader file = new StreamReader(strPath))
                {
                    string line;
                    string[] vals;
                    while ((line = file.ReadLine()) != null)
                    {
                        vals = line.Split(',');
                        try
                        {
                            Record record = new Record
                            {
                                V = Convert.ToInt32(vals[0]),
                                HC = Convert.ToInt32(vals[1]),
                                S1 = Convert.ToDouble(vals[2]),
                                S2 = Convert.ToDouble(vals[3])
                            };

                            arrRecords.Add(record);
                        }
                        catch
                        {
                            return arrRecords;
                        }
                    }
                }

            }
            catch
            {
                return arrRecords;
            }

            return arrRecords;
        }
    }
}
