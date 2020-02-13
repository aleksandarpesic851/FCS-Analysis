using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TestConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1) // 4
            {               
                // bool ouputExcel = false;
                //  string fcsFileName = args[0];
                //  string filePath_gates = args[1];
                //  string channelNomenclature = args[2];
                //  string sampleType = args[3];
                //int[] NLME_counts = new int[3];
                //NLME_counts = 
                 string Total_ExcelFileName = args[0];
               // string Total_ExcelFileName = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Data/Experiment List for analysis for Fixed Gating.xlsx";
                Console.WriteLine(String.Format("Starting Processing Folder Data" +
                    " according to Excel List\n{0}", Total_ExcelFileName));
                FlowCytometry.FCMeasurement.FG_folder_analysis(Total_ExcelFileName);
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
