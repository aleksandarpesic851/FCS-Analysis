using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TestConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1) // 4
            {               
                string Total_ExcelFileName = "F:/VisualStudio/C#/MAX_WORK/FCS-Analysis/Experiment List for analysis for Fixed Gating.xlsx";
                Console.WriteLine(String.Format("Starting Processing Folder Data" +
                    " according to Excel List/n{0}", Total_ExcelFileName));

                string filePath_gates = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Fixed gating";
                string channelNomenclature = "new_names";// type of nomeclature: "old_names", "middleaged_names"

                FlowCytometry.FCMeasurement.FG_folder_analysis(Total_ExcelFileName, filePath_gates, channelNomenclature);
                Console.WriteLine("Finished Processing Folder Data according to Excel List");
                //FlowCytometry.FCMeasurement.WBC_analysis(args, ouputExcel);                       
            }
            else
            {
                throw new NotSupportedException("Insufficient or incorrect arguments provided");
            }
                
        }
    }
}
