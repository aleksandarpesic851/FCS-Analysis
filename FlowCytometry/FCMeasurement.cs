using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace FlowCytometry
{
    public class Channel
    {
        private int bits;
        private string name;
        private int range;
        public string beginTime; // last 3 belong in SAMPLE class -- they are the same for all channels for a given sample
        public string endTime;
        public string date;

        ///$PnN Short name for parameter n.
        public string Name
        {
            get
            {
                return name;
            }
        }

        ///$PnR Range for parameter number n. 
        public int Range
        {
            get
            {
                return range;
            }
        }

        ///$PnB Number of bits reserved for parameter number n.
        public int Bits
        {
            get
            {
                return bits;
            }
        }

        private int number;
        public int Number
        {
            get
            {
                return number;
            }
        }

        private string ampType;
        ///$PnE Amplification type for parameter n.        
        public string AmplificationType
        {
            get
            {
                return ampType;
            }
        }

        public string RunBeginTime
        {
            get
            {
                return beginTime;
            }
        }

        public string Date
        {
            get
            {
                return date;
            }
        }


        // Amplifier gain used for acquisition of parameter n. 
        private double gain;
        private bool hasGain = false;

        public bool HasGain
        {
            get
            {
                return hasGain;
            }
        }

        public double Gain
        {
            get
            {
                if (hasGain)
                    return gain;
                else
                    throw new NotSupportedException("Parameter Gain is not available");
            }
            internal set
            {
                this.gain = value;
                hasGain = true;
            }
        }

        private double[] data;

        internal Channel(int number, int bits, string name, int range, string beginTime, string endTime, string date, string ampType, int n)
        {
            this.number = number;
            this.bits = bits;
            this.name = name;
            this.range = range;
            this.beginTime = beginTime;
            this.endTime = endTime;
            this.date = date;
            this.ampType = ampType;
            data = new double[n];
        }

        internal void AddData(int index, float value)
        {
            data[index] = Convert.ToDouble(value);
        }

        internal void AddData(int index, double value)
        {
            data[index] = value;
        }

        internal void FlushData()
        {
            readOnlyData = Array.AsReadOnly<double>(data);
        }

        IReadOnlyCollection<double> readOnlyData;

        public IReadOnlyCollection<double> Data
        {
            get
            {
                return readOnlyData;
            }
        }
    }
    public class FCMeasurement
    {

        IReadOnlyDictionary<string, Channel> channels;
        public IReadOnlyDictionary<string, Channel> Channels
        {
            get
            {
                return channels;
            }
        }


        IReadOnlyDictionary<string, string> meta;
        public IReadOnlyDictionary<string, string> Meta
        {
            get
            {
                return meta;
            }
        }


        IReadOnlyDictionary<string, int> countWBC;

        public IReadOnlyDictionary<string, int> CountWBC
        {
            get
            {
                return countWBC;
            }
        }



        IReadOnlyCollection<string> channelNames;

        public IReadOnlyCollection<string> ChannelsNames
        {
            get
            {
                return channelNames;
            }
        }

        private int counts;

        public int Counts
        {
            get
            {
                return counts;
            }
        }

        public FCMeasurement(string fileName)
        {
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                InternalRead(stream);
            }
        }

        public FCMeasurement(byte[] dataArray)
        {
            using (Stream stream = new MemoryStream(dataArray))
            {
                InternalRead(stream);
            }
        }

        public FCMeasurement(Stream stream)
        {
            if (!stream.CanRead)
                throw new ArgumentException("Not readable stream");

            if (!stream.CanSeek)
                throw new ArgumentException("Not supported streams cannot seek");

            InternalRead(stream);
        }

        private long ReadLong(BinaryReader reader)
        {
            char[] header = new char[8];
            reader.Read(header, 0, 8);
            String str = new String(header, 0, 8);
            return Convert.ToInt64(str.Trim());
        }

        public static String Enquote(string str) //private
        {
            str = str.Replace("\"", "\\\"");
            if (str.Contains(","))
            {
                return String.Format("\"{0}\"", str);
            }
            else
                return str;
        }

        public void WriteMetadata(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                foreach (KeyValuePair<String, String> kvp in meta)
                {
                    writer.WriteLine(Enquote(kvp.Key) + "," + Enquote(kvp.Value));
                }
            }
        }


        public void WriteChannels(string fileName, String[] channelNames)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < channelNames.Length; i++)
                {
                    if (channels.ContainsKey(channelNames[i]))
                    {
                        if (i > 0)
                            writer.Write(",");
                        writer.Write(Enquote(channelNames[i]));
                    }
                }
                writer.WriteLine();

                for (int j = 0; j < counts; j++)
                {
                    for (int i = 0; i < channelNames.Length; i++)
                    {
                        if (channels.ContainsKey(channelNames[i]))
                        {
                            if (i > 0)
                                writer.Write(",");
                            writer.Write(channels[channelNames[i]].Data.ElementAt(j));
                        }
                    }
                    writer.WriteLine();
                }
            }
        }

        public void WriteExcelFile(String fileName)
        {
            PicoXLSX.Workbook workbook = new PicoXLSX.Workbook(fileName, "Metadata");
            workbook.AddWorksheet("Channels");

            foreach (KeyValuePair<String, String> kvp in meta)
            {
                workbook.Worksheets[0].AddNextCell(kvp.Key);
                workbook.Worksheets[0].AddNextCell(kvp.Value);
                workbook.Worksheets[0].GoToNextRow();
            }

            workbook.Worksheets[0].SetColumnWidth(0, 35f);
            workbook.Worksheets[0].SetColumnWidth(1, 70f);

            PicoXLSX.Style rightAlignStyle = new PicoXLSX.Style();
            rightAlignStyle.CurrentCellXf.HorizontalAlign = PicoXLSX.Style.CellXf.HorizontalAlignValue.right;

            foreach (String channelName in channelNames)
            {
                workbook.Worksheets[1].AddNextCell(channelName, rightAlignStyle);
            }
            workbook.Worksheets[1].GoToNextRow();

            PicoXLSX.Style timeStyle = new PicoXLSX.Style();
            timeStyle.CurrentNumberFormat.CustomFormatID = 164;
            timeStyle.CurrentNumberFormat.CustomFormatCode = "0.000";
            timeStyle.CurrentNumberFormat.Number = PicoXLSX.Style.NumberFormat.FormatNumber.custom;

            PicoXLSX.Style valueStyle = new PicoXLSX.Style();
            valueStyle.CurrentNumberFormat.CustomFormatID = 164 + 1;
            valueStyle.CurrentNumberFormat.CustomFormatCode = "0.000000";
            valueStyle.CurrentNumberFormat.Number = PicoXLSX.Style.NumberFormat.FormatNumber.custom;


            for (int i = 0; i < channelNames.Count; i++)
                workbook.Worksheets[1].SetColumnWidth(i, 18f);

            for (int j = 0; j < counts; j++)
            {
                foreach (String channelName in channelNames)
                {
                    if (channelName.Equals("Time"))
                        workbook.Worksheets[1].AddNextCell(channels[channelName].Data.ElementAt(j), timeStyle);
                    else
                        workbook.Worksheets[1].AddNextCell(channels[channelName].Data.ElementAt(j), valueStyle);
                }
                workbook.Worksheets[1].GoToNextRow();
            }

            workbook.Save();
        }

        private string ReadStringFromMetadata(BinaryReader reader, byte textSeparator)
        {
            StringBuilder sb = new StringBuilder();
            byte b;
            do
            {
                b = reader.ReadByte();
                if (b != textSeparator)
                    sb.Append((char)b);
            } while (b != textSeparator);
            return sb.ToString();
        }

        private void InternalRead(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                char[] header = new char[256];
                reader.Read(header, 0, 6);
                String str = new String(header, 0, 6);
                if (str != "FCS3.0" && str != "FCS3.1")
                    throw new InvalidDataException("Header is not valid for FCS 3.0 or 3.1 format");

                // Skip 4 spaces
                reader.Read(header, 0, 4);

                long startTextSegment = ReadLong(reader);
                long endTextSegment = ReadLong(reader);
                long startDataSegment = ReadLong(reader);
                long endDataSegment = ReadLong(reader);
                long startAnalyticsSegment = ReadLong(reader);
                long endAnalyticsSegment = ReadLong(reader);

                reader.BaseStream.Seek(startTextSegment, SeekOrigin.Begin);

                byte textSeparator = reader.ReadByte();

                Dictionary<string, string> metaDict = new Dictionary<string, string>();
                do
                {
                    string name = ReadStringFromMetadata(reader, textSeparator);
                    string value = ReadStringFromMetadata(reader, textSeparator);
                    metaDict.Add(name, value);
                } while (reader.BaseStream.Position < endTextSegment);

                meta = (IReadOnlyDictionary<string, string>)metaDict.ToDictionary(pair => pair.Key, pair => pair.Value);

                List<string> channelNames = new List<string>();
                Dictionary<string, Channel> channelDict = new Dictionary<string, Channel>();
                int paramCount = Convert.ToInt32(meta["$PAR"]);
                counts = Convert.ToInt32(meta["$TOT"].Trim());
                for (int i = 1; i <= paramCount; i++)
                {
                    string name = meta[String.Format("$P{0}N", i)].Trim();
                    int bits = Convert.ToInt32(meta[String.Format("$P{0}B", i)].Trim());
                    int range = Convert.ToInt32(meta[String.Format("$P{0}R", i)].Trim());
                    string ampType = meta[String.Format("$P{0}E", i)].Trim();

                    string beginTime = meta["$BTIM"].Trim();
                    string endTime = meta["$ETIM"].Trim();
                    string date = meta["$DATE"].Trim();

                    #region skipped meta
                    //string opticalFilter = meta[String.Format("$P{0}F", i)].Trim();
                    //string visScale = meta[String.Format("$P{0}D", i)].Trim();
                    //string wavelength = meta[String.Format("$P{0}L", i)].Trim();
                    //string power = meta[String.Format("$P{0}O", i)].Trim();
                    //string percent = meta[String.Format("$P{0}P", i)].Trim();
                    //string sname = meta[String.Format("$P{0}S", i)].Trim();
                    //string detectorType = meta[String.Format("$P{0}T", i)].Trim();
                    //string detectorVoltage = meta[String.Format("$P{0}V", i)].Trim();
                    //string gatingRegion = meta[String.Format("$R{0}I", i)].Trim();
                    //string windowSetting = meta[String.Format("$R{0}W", i)].Trim();
                    #endregion

                    channelNames.Add(name);

                    channelDict.Add(name, new Channel(i, bits, name, range, beginTime, endTime, date, ampType, counts));
                    //  channelDict.Add(name, new Channel(i, bits, name, range, ampType, counts));
                    string gainKey = String.Format("$P{0}G", i);
                    if (meta.ContainsKey(gainKey))
                        channelDict[name].Gain = Convert.ToDouble(meta[gainKey].Trim());
                }

                this.channelNames = channelNames.AsReadOnly();
                this.channels = (IReadOnlyDictionary<string, Channel>)channelDict.ToDictionary(pair => pair.Key, pair => pair.Value);

                bool reverse = meta["$BYTEORD"].Trim().Equals("4,3,2,1");
                string dataType = meta["$DATATYPE"].Trim();
                reader.BaseStream.Seek(startDataSegment, SeekOrigin.Begin);
                for (int i = 0; i < counts; i++)
                {
                    for (int j = 1; j <= paramCount; j++)
                    {
                        if (dataType == "F")
                        {
                            byte[] b1 = reader.ReadBytes(4);
                            if (reverse)
                                Array.Reverse(b1);
                            channels[channelNames[j - 1]].AddData(i, BitConverter.ToSingle(b1, 0));
                        }
                        else if (dataType == "D")
                        {
                            byte[] b1 = reader.ReadBytes(8);
                            if (reverse)
                                Array.Reverse(b1);
                            channels[channelNames[j - 1]].AddData(i, BitConverter.ToDouble(b1, 0));
                        }
                        else if (dataType == "I")
                        {
                            int width = 0;
                            if (meta[String.Format("$P{0}B", j)] == "8")
                                width = 1;
                            if (meta[String.Format("$P{0}B", j)] == "16")
                                width = 2;
                            if (meta[String.Format("$P{0}B", j)] == "24")
                                width = 3;
                            if (meta[String.Format("$P{0}B", j)] == "32")
                                width = 4;

                            if (width == 0)
                                throw new NotSupportedException("This datatype is not supported yet");

                            byte[] b1 = new byte[4];
                            Array.Copy(reader.ReadBytes(width), b1, width);

                            if (reverse)
                                Array.Reverse(b1);

                            channels[channelNames[j - 1]].AddData(i, BitConverter.ToInt32(b1, 0));
                        }
                        else
                        {
                            throw new NotSupportedException("This datatype is not supported yet");
                        }
                    }
                }

                for (int j = 1; j <= paramCount; j++)
                {
                    channels[channelNames[j - 1]].FlushData();
                }

                reader.Close();
            }
        }

        public static void ExtractInfo(string fcsFileName) //, string metadataFileName, string tableFileName)
        {
            // string metadataFileName, string tableFileName)
            string fcsFilePath = Path.GetDirectoryName(fcsFileName);
            string fcsFileOnly = Path.GetFileNameWithoutExtension(fcsFileName); //GetFileName

            // string metaFileName = String.Join(" ", fcsFileOnly, "metadata.csv");
            string excelFileName = String.Join(" ", fcsFileOnly, "Excel output.xlsx");
            //  string metadataFileName = Path.Combine(fcsFilePath, Path.GetFileName(metaFileName));
            string tableFileName = Path.Combine(fcsFilePath, Path.GetFileName(excelFileName));

            FCMeasurement sample = new FCMeasurement(fcsFileName);
            // sample.WriteMetadata(metadataFileName);
            sample.WriteExcelFile(tableFileName);
        }

        private static HashSet<double[]> GenerateHS(FCMeasurement sampleIn, string channel1, string channel2)
        {
            HashSet<double[]> hs = new HashSet<double[]>();
            for (int i = 0; i < sampleIn.Counts; i++) //sample.Counts     5000
            {
                hs.Add(new double[2] { sampleIn.Channels[channel1].Data.ElementAt(i), sampleIn.Channels[channel2].Data.ElementAt(i) });
            }
            return hs;
        }
        
        public static Tuple<int[], bool[]> diff3_Gating(string fcsFileName, string filePath_gates, string channelNomenclature) //analyzeType
        {
            // Tuple<int, bool[]> int[]
            //resultCsvFile //   channelNomenclature = "new_names";//"old_names";
            int[] NML_count = new int[3]; //output: Neutrophils, Monocytes, Lymphocytes (in order)
            string sampleType = "3-diff";
            string FCS1_H = FCMeasurement.GetChannelName("FCS1peak", channelNomenclature);
            string SSC_H = FCMeasurement.GetChannelName("SSCpeak", channelNomenclature);

            Console.WriteLine(String.Format("Starting analysis of:\n{0}", fcsFileName));

            #region generate dataset, initialize HashSets

            FCMeasurement sample = new FCMeasurement(fcsFileName);
            int TotalDataLength = sample.Counts;// GateIntactCells_array.GetLength(0);

            //string threeDiffGated = "threeDiffGated";
            //string outsideFixedGates = "OutsideGates";
            bool[,] indexGate_IntactCells;// = new bool[TotalDataLength, 2];
            bool[,] indexGate_Singlets;// = new bool[TotalDataLength, 2];
            bool[,] indexGate_CellTypes = new bool[TotalDataLength, 2];
            double x;
            double y;

            int FSC1_H_max = sample.Channels[FCS1_H].Range;
            int SSC_H_max = sample.Channels[SSC_H].Range;
            //  sample.Channels[FCS1_H].Data.ElementAt(j)
            // Console.WriteLine(String.Format("FSC1_H_max: {0}, SSC_H_max: {1}", FSC1_H_max, SSC_H_max));
            double sampleVolume_uL = getSampleVolume(sample, sampleType, channelNomenclature);

            HashSet<double[]> Gate1_hs;
            HashSet<double[]> Gate2_hs;
            //HashSet<double[]> Gate3_hs;

            bool OutputExcel = false; // true ;
            if (OutputExcel)
            {
                string outputFileName = String.Format("{0} Excel.xlsx", Path.GetFileNameWithoutExtension(fcsFileName));
                sample.WriteExcelFile(outputFileName);
            }

            #endregion

            #region get Intact (cells) gate file location, Channel names
            string fileName_gatedCells = "gating Cells.csv";
            string GateIntactCells_file = Path.Combine(filePath_gates, Path.GetFileName(fileName_gatedCells));
            Console.WriteLine("Gate path {0}", GateIntactCells_file);
              Console.WriteLine(String.Format("FCS1_H= {0}\nSSC_H = {1}", FCS1_H, SSC_H));
            Gate1_hs = GenerateHS(sample, FCS1_H, SSC_H);
            double[][] GateIntactCells_array = Gate1_hs.ToArray();

            #endregion

            // List<Polygon> polygons = loadPolygon(GateIntactCells_file);

            #region analyze "Cells" selection gate

            indexGate_IntactCells = GateArray(GateIntactCells_array, GateIntactCells_file, FSC1_H_max, SSC_H_max);
            //  Console.WriteLine(String.Format("Length of IntactCellsGatge = {0}\n Data Length = {1}", indexGate_IntactCells.Length, TotalDataLength));
            bool[] intactCol_Max = GetColumn(indexGate_IntactCells, 0); //getColumn
            bool[] intactCol_Gate = GetColumn(indexGate_IntactCells, 1);
            int count_Gate1out_Max = CountBoolTrue(intactCol_Max);
            int count_Gate1out = CountBoolTrue(intactCol_Gate);

            //indexGate_IntactCells write to file == OUTPUT
            #region Explicit gating (not used)
            /*
            for (int j = 0; j < TotalDataLength; j++)   // for (int j = 0; j < Gate1Max_Length; j++)
            {
                x = GateIntactCells_array[j][0];    //x = GateIntactCells_arrayMax[j][0];
                y = GateIntactCells_array[j][1];    //y = GateIntactCells_arrayMax[j][1];
                                          // indexGate_Cells[j] = false;
                if (x >= 16383 || y >= 16383)
                {
                    indexGate_Cells_Max[j] = false;
                    indexGate_Cells[j] = false;
                }
                else
                {
                    indexGate_Cells_Max[j] = true;
                    if (polygons != null)
                    {
                        if (polygons[0].IsInsidePoly(x, y))
                        {
                            indexGate_Cells[j] = true;
                        }
                        else
                        {
                            indexGate_Cells[j] = false;
                        }
                    }
                }
            }
            */
            #endregion

            //  indexGate_CellTypes = FlowCytometry.FCMeasurement.GateArray(GateIntactCells_array, GateIntactCells_file);

            //            int count_Gate1out_Max = CountBoolTrue2D(indexGate_CellTypes, 0);
            //           int count_Gate1out = CountBoolTrue2D(indexGate_CellTypes, 1);
            #endregion

            // GATE 2: Singlets

            #region get Singlets gate file location, generate hashset
            string fileName_gatedSinglets = "gating Singlets.csv";
            string SingletsGate_file = Path.Combine(filePath_gates, Path.GetFileName(fileName_gatedSinglets));

            string FSC1_A = GetChannelName("FCS1area", channelNomenclature);

            Gate2_hs = GenerateHS(sample, FSC1_A, FCS1_H);
            double[][] GateSinglets_array = Gate2_hs.ToArray();
            #endregion
            //  polygons = loadPolygon(SingletsGate_file);

            double[] SingletsFit = LinearRegression(GateSinglets_array);

            double[] X1Y1X2Y2 = new double[4];
            X1Y1X2Y2[0] = 0;
            X1Y1X2Y2[1] = SingletsFit[1];
            X1Y1X2Y2[2] = SingletsFit[0];
            X1Y1X2Y2[3] = 1;// SingletsFit[1];

            Console.WriteLine(String.Format("Linear regression fix Y = {0}*x+{1}", X1Y1X2Y2[3], X1Y1X2Y2[2]));

            int FSC1_A_max = sample.Channels[FSC1_A].Range;

            #region analyze Gate2 "Singlets"

            //            indexGate_CellTypes = FlowCytometry.FCMeasurement.GateArray(GateSinglets_array, SingletsGate_file);

            indexGate_Singlets = GateArray(GateSinglets_array, SingletsGate_file, FSC1_A_max, FSC1_H_max);

            bool[] Singles_Max = GetColumn(indexGate_Singlets, 0); //getColumn
            bool[] Singlets_Gate = GetColumn(indexGate_Singlets, 1);
            int count_Gate2Max = CountBoolTrue(Singles_Max);
            int count_Gate2 = CountBoolTrue(Singlets_Gate);

            //  int count_Gate2Max = CountBoolTrue2D(indexGate_Singlets, 0);
            //  int count_Gate2 = CountBoolTrue2D(indexGate_Singlets, 1);
            #endregion

            // GATE 3: Neutrophils,Lymphocytes, Monocytes
            // Channels for Gate 3 are the same as Gate 1 (FCS1 vs SSC)

            #region read Final Gate 
            string fileName_GateFinal = "gating Cell Types.csv";
            string GateFinal_file = Path.Combine(filePath_gates, Path.GetFileName(fileName_GateFinal));
            #endregion

            List<Polygon> polygons = loadPolygon(GateFinal_file);

            #region analyze GateFinal for positive selection by Cell size or Granularity (FSC vs SSC)
            /*
            bool testCombinedGate = false;  //true;//
            if (testCombinedGate)
            {
                indexGate_CellTypes = GateArray(GateIntactCells_array, GateIntactCells_file, FSC1_A_max, FSC1_H_max);
            }
            */
            /// CUT here
            bool[] NeutrophilsTF = new bool[TotalDataLength];
            if (polygons != null)
            {
                HashSet<double[]>[] cytes = new HashSet<double[]>[polygons.Count];

                for (int j = 0; j < polygons.Count; j++)
                {
                    cytes[j] = new HashSet<double[]>();
                }

                for (int j = 0; j < TotalDataLength; j++)//Gate2Max_Length 
                {
                    x = GateIntactCells_array[j][0]; //GateIntactCells_array = GateFinal_array, so using already created GateIntactCells_array
                    y = GateIntactCells_array[j][1];
                    indexGate_CellTypes[j, 0] = false;
                    indexGate_CellTypes[j, 1] = false;

                    for (int k = 0; k < polygons.Count; k++)
                    {
                        if (polygons[k].IsInsidePoly(x, y))                   //  (Polygon.IsInsidePoly(x, y))
                        {
                            cytes[k].Add(new double[2] { x, y });
                            indexGate_CellTypes[j, 1] = true;
                            if (k == 0)
                            {
                                NeutrophilsTF[j] = true;
                            }
                        }
                    }
                }
                for (int k = 0; k < polygons.Count; k++)
                {
                    NML_count[k] = cytes[k].Count;
                }
            }
            #endregion


            #region putting the gates together
            bool[] indexUnGated = new bool[TotalDataLength];
            bool[] indexGatesTotal = new bool[TotalDataLength];
            bool[] indexDataShown = new bool[TotalDataLength];
            bool[] indexDataShownNotGated = new bool[TotalDataLength];
            bool[] indexGatesG1G2 = new bool[TotalDataLength];
            bool[] indexGatesG1G2G3 = new bool[TotalDataLength];
            for (int j = 0; j < TotalDataLength; j++)
            {
                indexUnGated[j] = true;
                indexDataShown[j] = indexGate_IntactCells[j, 0] && indexGate_Singlets[j, 0];
                indexGatesTotal[j] = indexDataShown[j] && indexGate_IntactCells[j, 1] && indexGate_Singlets[j, 1] && indexGate_CellTypes[j, 1];
                indexGatesG1G2[j] = indexGate_IntactCells[j, 1] && indexGate_Singlets[j, 1];
                indexGatesG1G2G3[j] = indexDataShown[j] && indexGate_IntactCells[j, 1] && indexGate_CellTypes[j, 1];
                indexDataShownNotGated[j] = indexDataShown[j] && !indexGatesTotal[j];
            }
            int totalGatedCells = CountBoolTrue(indexGatesTotal);
            bool[] FinaCellTypeGate = GetColumn(indexGate_CellTypes, 1);
            int GateFinalCells = CountBoolTrue(FinaCellTypeGate);

            //  Console.WriteLine(stopwatch.ElapsedMilliseconds);

            bool output_Gating_Stats = true;
            if (output_Gating_Stats)
            {
                string Count_Ini = String.Format("Length of initial / raw data  = {0}", TotalDataLength);
                string Count_G1Max = String.Format("Gate1 removed Max values count = {0}", count_Gate1out_Max);
                string Count_G1 = String.Format("Gate1: FCS vs SSC (peak) Count = {0}", count_Gate1out);
                string Count_G2Max = String.Format("Gate2 removed Max values count = {0}", count_Gate2Max);
                string Count_G2 = String.Format("Gate2 Count FSC1 Peaks vs Area = {0}", count_Gate2);
                //  string Count_G3Max = String.Format("Gate3 removed Max values count = {0}", count_Gate3Max);
                // string Count_G3 = String.Format("Gate3 (FSC2 vs FL) gated cells = {0}", count_Gate3);
                string Count_GFinal = String.Format("Gate3 (FSC1 vs SSC) gated cells = {0}", GateFinalCells);
                string Count_Total = String.Format("Total gated cells (G1*G2*G3) = {0}", totalGatedCells); //G3*
                double calculatedExcludedPercent = 100 * (TotalDataLength - totalGatedCells) / TotalDataLength; //, 1 Math.Round((
                                                                                                                //  calculatedExcludedPercent = Math.Round(calculatedExcludedPercent,1);
                string excludedPercent = String.Format("Percent events gated out = {0}%", calculatedExcludedPercent);

                string[] joined_report = new string[8] {Count_Ini, Count_G1Max, Count_G1, Count_G2Max, Count_G2,
                Count_GFinal, Count_Total, excludedPercent}; //Count_G3Max, Count_G3,
                Console.WriteLine(String.Join(" \n", joined_report));
            }
            #endregion

            // return NML_count;
            // Tuple<int, bool[]> int[]
            var tuple = new Tuple<int[], bool[]>(NML_count, NeutrophilsTF);
            return tuple;
        }

        public static int[] processEOS(string fcsFileName, string filePath_gates, string channelNomenclature)
        {
            FCMeasurement sample = new FlowCytometry.FCMeasurement(fcsFileName);
            int TotalDataLength = sample.Counts;
            //string sampleType = "EOS";
            //double sampleVolume_uL = getSampleVolume(sample, sampleType, channelNomenclature);

            string FL_H = GetChannelName("FLpeak", channelNomenclature);
            string SSC_H = GetChannelName("SSCpeak", channelNomenclature);
            int FL_H_max = sample.Channels[FL_H].Range;
            int SSC_H_max = sample.Channels[SSC_H].Range;

            string fileName_GateEOS = "gating Eosinophils.csv";
            string GateEOS_file = Path.Combine(filePath_gates, Path.GetFileName(fileName_GateEOS));

            //   bool[] indexGateEOS = new bool[TotalDataLength];

            var tuple = diff3_Gating(fcsFileName, filePath_gates, channelNomenclature);

            int[] EOSreport = new int[4];
            EOSreport[0] = tuple.Item1[0];
            EOSreport[1] = tuple.Item1[1];
            EOSreport[2] = tuple.Item1[2];

            //new Tuple<int[], bool[]>(NML_count, indexNeutrophils);
            bool[] GatedNeutrophils = tuple.Item2; // indexNeutrophils
            //  Get Neutrophils
            HashSet<double[]> GateEOS_hs = GenerateHS(sample, FL_H, SSC_H);
            HashSet<double[]> Neutrophils_hs = new HashSet<double[]>();
            double[][] EOS_array = GateEOS_hs.ToArray();

            double x;
            double y;

            for (int i = 0; i < TotalDataLength; i++)
            {
                x = EOS_array[i][0];
                y = EOS_array[i][1];
                if (GatedNeutrophils[i])
                {
                    Neutrophils_hs.Add(new double[2] { x, y });
                }
            }
            double[][] Neutrophils_array = Neutrophils_hs.ToArray();
            bool[,] indexGate_EOS = GateArray(Neutrophils_array, GateEOS_file, FL_H_max, SSC_H_max);
            //            bool[,] indexGate_EOS = GateArray(EOS_array, GateEOS_file, FSC1_H_max, SSC_H_max);
            bool[] EOS_Max = GetColumn(indexGate_EOS, 0); //getColumn
            bool[] EOS_Gate = GetColumn(indexGate_EOS, 1);
            int count_EOS_Max = CountBoolTrue(EOS_Max);
            int count_GateEOS = CountBoolTrue(EOS_Gate);
            EOSreport[3] = count_GateEOS;
            Console.WriteLine(String.Format("EOS (max): {0} \nInside EOS gate: {1}", count_EOS_Max, count_GateEOS));
            return EOSreport;
        }

        public static int ProcessBaso(string fcsFileName, string filePath_gates, string channelNomenclature) //processWBC , string filetype
        {
            string sampleType = "BASO";
            FCMeasurement sample = new FlowCytometry.FCMeasurement(fcsFileName);
            string source_dir = Path.GetDirectoryName(fcsFileName);

            string FCS1_H = GetChannelName("FCS1peak", channelNomenclature);
            string SSC_H = GetChannelName("SSCpeak", channelNomenclature);

            double sampleVolume_uL = getSampleVolume(sample, sampleType, channelNomenclature);
            // getSampleVolume(FCMeasurement sample, string sampleType, string channelNomenclature)


            #region get BASO gate file location, Channel names
            string fileName_gateBasophils = "gating Basophils.csv";
            string fileName_BasophilsClusterCenters = "clusterCenters Baso2.csv";
            string GateBaso_file = Path.Combine(filePath_gates, Path.GetFileName(fileName_gateBasophils));
            string GateBaso_Clusters = Path.Combine(filePath_gates, Path.GetFileName(fileName_BasophilsClusterCenters));

            HashSet<double[]> Gate1_hs = GenerateHS(sample, FCS1_H, SSC_H);
            double[][] GateBaso_array = Gate1_hs.ToArray();

            int TotalDataLength = GateBaso_array.GetLength(0);
            int FSC1_H_max = sample.Channels[FCS1_H].Range;
            int SSC_H_max = sample.Channels[SSC_H].Range;

            #endregion

            bool[,] indexGate_Baso = GateArray(GateBaso_array, GateBaso_file, FSC1_H_max, SSC_H_max);
            bool[] intactBaso_Max = GetColumn(indexGate_Baso, 0);
            bool[] intactBaso_Gate = GetColumn(indexGate_Baso, 1);
            int count_Gate1out_Max = CountBoolTrue(intactBaso_Max);
            int count_Gate1out = CountBoolTrue(intactBaso_Gate);

            double[][] centers;// = null;
            double cluster_centerX;
            double cluster_centerY;

            List<Polygon> polygons = loadPolygon(GateBaso_file);
            bool hasClass = false;
            int number = 8;
            centers = LoadClusterCenters(GateBaso_Clusters);
            SharpCluster.Partitional.KMeans kmeans = new SharpCluster.Partitional.KMeans(Gate1_hs, hasClass);
            SharpCluster.Cluster[] clusters = kmeans.ExecuteClustering(number, centers);

            int count_inside = 0;

            foreach (SharpCluster.Cluster Cluster in clusters)
            {
                cluster_centerX = Cluster.Centroid.GetAttribute(0);
                cluster_centerY = Cluster.Centroid.GetAttribute(1);

                // classify clusters based on centroid being within fixed gates
                if (polygons[0].IsInsidePoly(cluster_centerX, cluster_centerY))
                {
                    count_inside += Cluster.GetPatterns().Length;
                    foreach (SharpCluster.Pattern pattern in Cluster.GetAllPatterns())
                    {
                        double xVal = pattern.GetAttribute(0);
                        double yVal = pattern.GetAttribute(1);
                    }
                }
            }

            string fcsFileOnly = Path.GetFileNameWithoutExtension(fcsFileName);
            string Baso_output_filename = String.Join(" ", fcsFileOnly, "Baso_count.txt");

            // sb.AppendLine(string.Join(",", fields));
            //File.WriteAllText(resultCsvFile, sb.ToString());
            // string Baso_output_filename = "Baso_output.txt"; //@
            string Baso_output_totPath = @Path.Combine(source_dir, Baso_output_filename);
            string Baso_output_content;
            Baso_output_content = String.Format("Using fixed gating \n" +
                "Number of Basophils found: {0}", count_inside);
            //= @Path.Combine(source_dir, Baso_output_filename);

            File.WriteAllText(Baso_output_totPath, Baso_output_content);
            //  File.WriteAllText(Baso_output_tot, count_Gate1out.ToString());
            //  Console.WriteLine("Wrote Baso_output.txt");

            return count_Gate1out;
        }

        public static double[][] LoadClusterCenters(string fileName)
        {
            String[] lines = File.ReadAllLines(fileName);
            double[][] centers = new double[lines.Length][];

            try
            {
                centers = new double[lines.Length][];
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] cells = lines[i].Split(new char[] { ',' });
                    centers[i] = new double[2];
                    centers[i][0] = Convert.ToDouble(cells[0].Trim());
                    centers[i][1] = Convert.ToDouble(cells[1].Trim());
                }
            }
            catch
            {
                centers = null;
            }
            return centers;

        }

        public class Polygon
        {
            public PointF[] poly;
            public Color color;

            public Polygon(PointF[] poly, Color color)
            {
                this.poly = poly;
                this.color = color;
            }

            public bool IsInsidePoly(double x, double y)
            {
                if (poly == null)
                    return false;

                double minX = poly[0].X;
                double maxX = poly[0].X;
                double minY = poly[0].Y;
                double maxY = poly[0].Y;
                for (int i = 1; i < poly.Length; i++)
                {
                    PointF q = poly[i];
                    minX = Math.Min(q.X, minX);
                    maxX = Math.Max(q.X, maxX);
                    minY = Math.Min(q.Y, minY);
                    maxY = Math.Max(q.Y, maxY);
                }

                if (x < minX || x > maxX || y < minY || y > maxY)
                {
                    return false;
                }

                // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
                bool inside = false;
                for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
                {
                    if ((poly[i].Y > y) != (poly[j].Y > y) &&
                         x < (poly[j].X - poly[i].X) * (y - poly[i].Y) / (poly[j].Y - poly[i].Y) + poly[i].X)
                    {
                        inside = !inside;
                    }
                }
                return inside;
            }
        }

        public static List<Polygon> loadPolygon(string fileName)
        {
            String[] lines = File.ReadAllLines(fileName); //   String[] lines = File.ReadAllLines(dlg.FileName);
            List<Polygon> polygons = new List<Polygon>();
            try
            {
                int i = 0;
                while (i < lines.Length)
                {
                    string[] cells = lines[i].Split(new char[] { ',' });
                    string[] colorCells = cells[2].Split(new char[] { '-' });

                    int n = Convert.ToInt32(cells[1].Trim());
                    PointF[] points = new PointF[n];
                    Color color = Color.FromArgb(Convert.ToInt32(colorCells[0].Trim()), Convert.ToInt32(colorCells[1].Trim()), Convert.ToInt32(colorCells[2].Trim()));
                    i++;
                    for (int j = 1; j <= n; j++)
                    {
                        cells = lines[i].Split(new char[] { ',' });
                        points[j - 1] = new PointF(Convert.ToSingle(cells[0].Trim()), Convert.ToSingle(cells[1].Trim()));
                        i++;
                    }
                    polygons.Add(new Polygon(points, color)); //Polygon.
                }
                //                this.polygons = polygons.ToArray();
                // this.polygons = polygons.ToArray();
            }
            catch
            {

            }
            return polygons;
        }

        public static string GetChannelName(string channelHandle, string type)
        {
            string channelName = null;
            if (type == "old_names")
            {
                if (channelHandle == "FCS1peak")
                    channelName = "FSC1LG,Peak"; //string FSC1peak = "FSC1LG,Peak"; 
                else if (channelHandle == "SSCpeak")
                    channelName = "SSCLG,Peak"; //string SSCpeak = "SSCLG,Peak"; 
                else if (channelHandle == "FCS1area")
                    channelName = "FSC1LG,Area"; //string FCS1area = "FSC1LG,Area"; 
                else if (channelHandle == "SSCarea")
                    channelName = "SSCLG,Area"; //string SSCarea = "SSCLG,Area";
                else if (channelHandle == "FSC2peak")
                    channelName = "FSC2HG,Peak";
                else if (channelHandle == "FLpeak")
                    channelName = "FLLG,Peak"; //"FLLG,Peak"
            }
            else if (type == "middleaged_names")
            {
                if (channelHandle == "FCS1peak")
                    channelName = "BS1CH1; fsc1lg-H";
                else if (channelHandle == "SSCpeak")
                    channelName = "BS1CH2; ssclg-H";
                else if (channelHandle == "FCS1area")
                    channelName = "BS1CH1; fsc1lg-A";
                else if (channelHandle == "SSCarea")
                    channelName = "BS1CH4; ssclg-A";
                else if (channelHandle == "FSC2peak")
                    channelName = "BS1CH2; fsc2lg-H";
                else if (channelHandle == "FLpeak")
                    channelName = "BS1CH3;fllg-H";//BS1CH2; 
            }
            else if (type == "new_names")// NEW names
            {
                if (channelHandle == "FCS1peak")
                    channelName = "BS1CH1; fsc1lg-H"; //string FCS1peak = "BS1CH1; fsc1lg-H";
                else if (channelHandle == "SSCpeak")
                    channelName = "BS1CH4; ssclg-H"; //string SSCpeak = "BS1CH4; ssclg-H";
                else if (channelHandle == "FCS1area")
                    channelName = "BS1CH1; fsc1lg-A"; //string FCS1area = "BS1CH1; fsc1lg-A";
                else if (channelHandle == "SSCarea")
                    channelName = "BS1CH4; ssclg-A"; //string SSCarea = "BS1CH4; ssclg-A";
                else if (channelHandle == "FSC2peak")
                    channelName = "BS1CH2; fsc2lg-H";
                else if (channelHandle == "FLpeak")
                    channelName = "BS1CH3;fllg-H";
            }

            return channelName;
        }

        public static bool[,] GateArray(double[][] Gate_array, string Gate_file, int CH1_Max, int CH2_Max) //string CH1, string CH2)
        {
            //  int FSC1_H_max = sample.Channels[CH1].Range;
            // int SSC_H_max = sample.Channels[CH2].Range;
            double x;
            double y;
            List<Polygon> polygons = loadPolygon(Gate_file);

            int TotalDataLength = Gate_array.Length;
            bool[,] indexGate_Inside_Max_ROI = new bool[TotalDataLength, 2];

            HashSet<double[]>[] PolygonRegion = new HashSet<double[]>[polygons.Count];
            for (int k = 0; k < polygons.Count; k++)
            {
                PolygonRegion[k] = new HashSet<double[]>();
            }

            //    HashSet<double[]>[] cytes = new HashSet<double[]>[polygons.Count];
            for (int j = 0; j < TotalDataLength; j++)//Gate2Max_Length 
            {
                x = Gate_array[j][0];
                y = Gate_array[j][1];
                indexGate_Inside_Max_ROI[j, 1] = false; //defaut assignment outside ROI
                // indexGate_Cells[j] = false;
                if (x >= CH1_Max - 1 || y >= CH2_Max - 1)
                {
                    indexGate_Inside_Max_ROI[j, 0] = false; //Max
                    indexGate_Inside_Max_ROI[j, 1] = false;
                }
                else
                {
                    indexGate_Inside_Max_ROI[j, 0] = true; //Max
                    if (polygons != null)
                    {
                        if (polygons[0].IsInsidePoly(x, y))
                        {
                            // PolygonRegion[0].Add(new double[2] { x, y }); //System.NullReferenceException: 'Object reference not set to an instance of an object.'
                            indexGate_Inside_Max_ROI[j, 1] = true;
                        }
                    }
                    else
                    {
                        //     ThrowException("Gate not found");
                    }
                }
            }
            return indexGate_Inside_Max_ROI;
        }

        public static double[] LinearRegression(double[][] xyVals) //double[] xVals, double[] yVals,
        {
            // out double rSquared,
            //out double yIntercept,
            //out double slope)
            double[] regCoeff = new double[3];
            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double sumCodeviates = 0;
            double rSquared;
            double yIntercept;
            double slope;

            for (var i = 0; i < xyVals.Length; i++)
            {
                var x = xyVals[i][0];
                var y = xyVals[i][1];

                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }

            var count = xyVals.Length;
            var ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            var ssY = sumOfYSq - ((sumOfY * sumOfY) / count);

            var rNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            var rDenom = (count * sumOfXSq - (sumOfX * sumOfX)) * (count * sumOfYSq - (sumOfY * sumOfY));
            var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            var meanX = sumOfX / count;
            var meanY = sumOfY / count;

            var dblR = rNumerator / Math.Sqrt(rDenom);

            rSquared = dblR * dblR;
            yIntercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
            regCoeff[0] = slope;
            regCoeff[1] = yIntercept;
            regCoeff[2] = rSquared;

            return regCoeff;
        }

        public static int CountBoolTrue(bool[] testArray)
        {
            bool val = true;
            return testArray.Count(c => c == val);
        }

        public static int CountBoolTrue2D(bool[,] testArray2D, int dimension)
        {
            bool val = true;
            int Ndata = testArray2D.Length;
            bool[] testArray1D = new bool[Ndata];
            for (int i = 0; i < Ndata; i++)
            {
                testArray1D[i] = testArray2D[i, dimension];
               
            }

            return testArray1D.Count(c => c == val);
        }

        public static bool[] GetColumn(bool[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        public static double getFlowRate(string fileType) //getSampleVolume FlowRate
        {
            double flow_rate = 37.6;
            if (fileType == "RBC")
            {
                flow_rate = 27; // microliter per min RBC, platelets
            }
            else if (fileType == "BASO")
            {
                flow_rate = 39.5; // microliter per min for Basophils
            }
            else // if (fileType == "WBC" || fileType == "3-diff" || fileType == "4-diff" || fileType == "EOS")
                flow_rate = 37.6; // microleter per min for WBC (3-diff)
            return flow_rate;
        }

        public static double getSampleVolume(FCMeasurement sample, string sampleType, string channelNomenclature)
        {
            double sampleVolume = new double();
            string FCS1_H = GetChannelName("FCS1peak", channelNomenclature);

            string sampleDate = sample.Channels[FCS1_H].date;
            string startTime = sample.Channels[FCS1_H].beginTime;
            string endTime = sample.Channels[FCS1_H].endTime;

            string beginDateTime = Enquote(string.Join(" ", sampleDate, startTime));
            string endDateTime = Enquote(string.Join(" ", sampleDate, endTime));
            Console.WriteLine(String.Format("Begin time ={0}\nEnd time ={1}", beginDateTime, endDateTime));

            DateTime dateTimeStart = DateTime.Parse(beginDateTime,
                    System.Globalization.CultureInfo.InvariantCulture);

            double sampleTimeSec = EpapsedTimeSec(sample, channelNomenclature);
            double flowRate = getFlowRate(sampleType);
            double sampleVolume_uL = Math.Round(sampleTimeSec * flowRate / 60, 0);

            Console.WriteLine(String.Format("Time of sampling: {0}s\n" +
                "Flow Rate: {1}uL/min\nSampleVolume: {2}uL\n", sampleTimeSec, flowRate, sampleVolume_uL));

            return sampleVolume;
        }

        public static double EpapsedTimeSec(FCMeasurement sample, string channelNomenclature)
        {
            string FCS1_H = FCMeasurement.GetChannelName("FCS1peak", channelNomenclature);
            string sampleDate = sample.Channels[FCS1_H].date;
            string startTime = sample.Channels[FCS1_H].beginTime;
            string endTime = sample.Channels[FCS1_H].endTime;

            string beginDateTime = Enquote(string.Join(" ", sampleDate, startTime));
            string endDateTime = Enquote(string.Join(" ", sampleDate, endTime));

            DateTime dateTimeStart = DateTime.Parse(beginDateTime,
                    System.Globalization.CultureInfo.InvariantCulture);
            DateTime dateTimeEnd = DateTime.Parse(endDateTime,
                                System.Globalization.CultureInfo.InvariantCulture);
            TimeSpan diff = dateTimeEnd - dateTimeStart; // Math.Abs(
            double diffSec = diff.TotalSeconds;
            //  Console.WriteLine(String.Format("Datetime A: {0}\nDatetime B: {1}\nTime difference: {2}s", dateTimeStart, dateTimeEnd, Math.Round(diffSec, 0)));

            return diffSec;
        }
        public static void FG_folder_analysis(string Total_ExcelFileName) //(string excelPathName, bool ouputExcel) //void Main(string[] args)
        {
            #region Read Excel file Generate list FileName_FlowRate
             
            string filePath_gates = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Fixed gating";
            // int SheetNum = 1;        
            /*   string Total_ExcelFileName = args[0];
            string RowStart_String = args[1];
            string RowCount_String = args[2];
            int RowStart = Convert.ToInt32(RowStart_String);
            int RowCount = Convert.ToInt32(RowCount_String);
            */
            int RowStart = 69; // 69 for 2014 DataFile
            int RowCount = 14;          // number of files in the list to be analyzed   

            //   string Total_ExcelFileName = Path.Combine(excelPath, Path.GetFileName(excelFileName));
            var FN_AT_FR_List = FlowCytometry.FCMeasurement.ReadExcelList(Total_ExcelFileName, RowStart, RowCount);
            //   Console.WriteLine(String.Format("String in cell A69:\n{0}", ExcelCell_String));
            #endregion
            
            int[] NMLE = new int[4];
            bool ouputExcel = false;
            string[] arguments = new string[4];
            arguments[1] = filePath_gates; // file path to folder with fixed gates
            string channelNomenclature = "new_names";// type of nomeclature: "old_names", "middleaged_names"
            arguments[2] = channelNomenclature;         

            double FlowRate;
            double[] NMLE_1k_Per_mL = new double[4];
            string[] NMLE_report = new string[4]; // in thousands of cells per mL as String
            StringBuilder sb = new StringBuilder();
            int FileNumber = 0;
            string FlowRateString;
            string FileName_WO_Ext;
            sb.AppendLine(string.Join(",", "FileName", "Analysis Type", "Volume(mL)",
                                     "Neutrophils", "Monocytes", "Lymphocytes", "Eosinophils",
                                     "Neut(K/mL)", "Mono(K/mL)", "Lymph(K/mL)", "Eos(K/mL)"));
            foreach (var listItem in FN_AT_FR_List) //Iterating over rows ... RowCount
            {
                FileNumber++; //RowCount 
                Console.Write(String.Format("Processing file #{0}", FileNumber));
                
                arguments[0] = listItem[0]; // full FCS filename with path an extension
                arguments[3] = listItem[1]; // type of analysis: "3-diff", "EOS", "BASO"
                FlowRateString = listItem[2];
                Console.Write(String.Format("Sample Volume associated with file = {0}mL", FlowRateString));
                FileName_WO_Ext = Path.GetFileNameWithoutExtension(arguments[0]);

                FlowRate = Convert.ToDouble(FlowRateString);
                NMLE = WBC_analysis(arguments, ouputExcel);

                for (int i = 0; i < NMLE.Length; i++)
                {
                    NMLE_1k_Per_mL[i] = Math.Round(NMLE[i] / FlowRate, 2);
                    NMLE_report[i] = Math.Round(NMLE_1k_Per_mL[i]/1000,2).ToString();
                    Console.Write(String.Format("NMLE{0} TC:{1}\n",i, NMLE[i])); //NMLE_report
                }
                Console.Write(String.Format("NMLE_Report: {0},{1},{2},{3}\n", NMLE_report));
                sb.AppendLine(string.Join(",", FileName_WO_Ext, arguments[3], FlowRateString, NMLE[0], NMLE[1], NMLE[2], NMLE[3],
                    NMLE_report[0], NMLE_report[1], NMLE_report[2], NMLE_report[3]));
            }
            File.WriteAllText("Fixed Gating output.csv", sb.ToString());
            Console.Write("Wrote Fixed Gating output\n");
            Console.Write("Press a button ...");
            Console.ReadKey();
        }
        public void Excel(string ExcelPath, int SheetNum) // string SheetName,
        {
            _Application excel = new _Excel.Application();
            Workbook wb = excel.Workbooks.Open(@ExcelPath);
            _Excel.Worksheet ws = (_Excel.Worksheet)wb.Worksheets[SheetNum];
            // ws = wb.Worksheets.get_Item(1); //[SheetNum];            
            //  ws = (_Excel.Worksheet)wb.Workbooks.get_Item(1); //[SheetNum];
            // ws = (_Excel.Worksheet)wb.ActiveSheet;

            wb.Close();
            excel.Quit();
        }

        public static List<string[]> ReadExcelList(string ExcelPath, int Row, int rowCount)
        {
            _Application excel = new _Excel.Application();
            int SheetNum = 1;
            Workbook wb = excel.Workbooks.Open(@ExcelPath);
            Worksheet ws = (wb.Worksheets[SheetNum] as Worksheet);
            var FN_AT_FR_List = new List<string[]>();

            Range xlRange = ws.UsedRange;
            int totalRowCount = xlRange.Rows.Count;
            int totalColCount = xlRange.Columns.Count;
            int i, k;
            List<string> colNames = new List<string>() 
            {
                "Filename",
                "FileLocation",
                "Analysis Type",
                "Volume",
            };

            int[] colIdxs = new int[4];
            int idx;
            for (i = 1; i <= totalColCount; i ++)
            {
                var tmpRange = (ws.Cells[1, i] as Range);
                if (tmpRange == null || tmpRange.Value == null)
                    continue;

                idx = colNames.IndexOf(tmpRange.Value.ToString());
                if ( idx > -1)
                {
                    colIdxs[idx] = i;
                }
            }

            Console.WriteLine("Column Idxs:{0}, {1}, {2}, {3}", colIdxs[0], colIdxs[1], colIdxs[2], colIdxs[3] );
            Console.WriteLine("Press a button...");
            Console.ReadKey();

            try
            {
                for (i = Row; i <= Row + rowCount; i++)
                {
                    if (i > totalRowCount)
                        break;
                    string[] newData = new string[4];

                    for (k = 0; k < colNames.Count; k++)
                    {
                        var tmpRange = (ws.Cells[i, colIdxs[k]] as Range);
                        if (tmpRange == null || tmpRange.Value == null)
                            break;

                        newData[k] = tmpRange.Value.ToString();
                    }

                    if (newData[0] == null)
                        continue;

                    FN_AT_FR_List.Add(new string[3] 
                        {
                            Path.Combine(newData[1], newData[0]),
                            newData[2],
                            newData[3]
                        });
                }
                wb.Close();
                excel.Quit();
            }          
            catch
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
                wb.Close();
                excel.Quit();
            }
            return FN_AT_FR_List;
        }

        public static int[] WBC_analysis(string[] args, bool ouputExcel) //void Main(string[] args)
        {
            int[] counts = new int[4]; // does not work for EOS, which is 4-diff (EOS + 3-diff)
            if (args.Length >= 3) //>= 3)
            {
                /* ERROR HANDLING EXCEPTIONS
                  FCS file not found
                  key (channel) names not found
                  type of analysis is not found
                */
                string fcsFileName = args[0]; // full FCS filename with path an extension
                string filePath_gates = args[1]; // file path to folder with fixed gates
                string channelNomenclature = args[2]; //  type of nomeclature: "old_names", "new_names", "middleaged_names"
                string sampleType = args[3]; // type of analysis: "EOS", "BASO", "3-diff"
                                             //  bool ouputExcel = args[4];
                                             //  ouputExcel = false;
                double pctNeutrophils; // percent Neutrophils
                double pctMonocytes; // percent Monocytes
                double pctLympocytes; // percent Lymphocytes
                double pctEosinophils; // percent Eosinophils
                int totalWBC;

                int[] NML_count = null;
                bool[] indexGatesG1G2 = null;
                var tuple = new Tuple<int[], bool[]>(NML_count, indexGatesG1G2);

                if (ouputExcel)
                {
                    FlowCytometry.FCMeasurement.ExtractInfo(args[0]); //, args[1], args[2]);
                }

                if (sampleType == "BASO") //"BASO" "EOS" "3-diff"
                {
                    counts[0] = ProcessBaso(fcsFileName, filePath_gates, channelNomenclature);
                    Console.WriteLine(String.Format("Basophil analysis for {0}: \n" +
                                                    "Basophil count: {1}", fcsFileName, counts[0]));
                }
                else if (sampleType == "EOS")
                {
                    counts = processEOS(fcsFileName, filePath_gates, channelNomenclature); //tuple.Item2, 
                    totalWBC = counts[0] + counts[1] + counts[2] + counts[3];

                    pctNeutrophils = 100 * counts[0] / totalWBC; // percent Neutrophils
                    pctLympocytes = 100 * counts[1] / totalWBC; // percent Lymphocytes
                    pctMonocytes = 100 * counts[2] / totalWBC;  // percent Monocytes
                    pctEosinophils = 100 * counts[3] / totalWBC;  // percent Eosinophils

                    pctNeutrophils = Math.Round(pctNeutrophils, 1);
                    pctLympocytes = Math.Round(pctLympocytes, 1);
                    pctMonocytes = Math.Round(pctMonocytes, 1);
                    pctEosinophils = Math.Round(pctEosinophils, 1);

                    Console.WriteLine(String.Format("4-diff analysis for {0}: \n" +
                                   "Total WBC: {1}\n" +
                                   "Neutrophils: {2}, {3}%\n" +
                                   "Monocytes: {4}, {5}%\n" +
                                   "Lymphocytes: {6}, {7}%\n" +
                                   "Eosinophils: {8}, {9}%\n", fcsFileName, totalWBC, counts[0], pctNeutrophils,
                                    counts[1], pctLympocytes, counts[2], pctMonocytes, counts[3], pctEosinophils));
                }
                else if (sampleType == "3-diff")
                {
                    tuple = diff3_Gating(fcsFileName, filePath_gates, channelNomenclature);
                    totalWBC = tuple.Item1[0] + tuple.Item1[1] + tuple.Item1[2];
                    pctNeutrophils = 100 * tuple.Item1[0] / totalWBC; // percent Neutrophils
                    pctLympocytes = 100 * tuple.Item1[1] / totalWBC; // percent Lymphocytes
                    pctMonocytes = 100 * tuple.Item1[2] / totalWBC;  // percent Monocytes

                    for (int i = 0; i <= 2; i++)
                        counts[i] = tuple.Item1[i];
                    counts[3] = 0; // No Eosinophils

                    pctNeutrophils = Math.Round(pctNeutrophils, 1);
                    pctLympocytes = Math.Round(pctLympocytes, 1);
                    pctMonocytes = Math.Round(pctMonocytes, 1);
                    Console.WriteLine(String.Format("3-diff analysis for {0}: \n" +
                                   "Total WBC: {1}\n" +
                                   "Neutrophils: {2}, {3}%\n" +
                                   "Monocytes: {4}, {5}%\n" +
                                   "Lymphocytes: {6}, {7}%\n", fcsFileName, totalWBC,
                    tuple.Item1[0], pctNeutrophils, tuple.Item1[1], pctLympocytes, tuple.Item1[2], pctMonocytes));
                }

                // Console.WriteLine("Finished processing. Press a button ...");
                // Console.ReadKey();
            }
            else
            {
                throw new NotSupportedException("Insufficient or incorrect arguments provided");
            }
            return counts;
        }

        public static void outputFCSasExcel(FCMeasurement sample)
        {           
                sample.WriteMetadata("meta.csv");
                string outputFileName = "Data Excel format.xlsx";
                sample.WriteExcelFile(outputFileName);     
        }
        public static bool[] findMaxValues(FCMeasurement sample, double[][] arrayInGate, string Channel1, string Channel2)
        {
            double x, y;
            int Channel1_Max = sample.Channels[Channel1].Range;
            int Channel2_Max = sample.Channels[Channel2].Range;

            int TotalDataLength = arrayInGate.Length;
            bool[] indexGate_Max = new bool[TotalDataLength];
            for (int j = 0; j < TotalDataLength; j++)
            {
                x = arrayInGate[j][0];
                y = arrayInGate[j][1];

                if (x >= Channel1_Max || y >= Channel2_Max)
                    indexGate_Max[j] = false;
                else
                    indexGate_Max[j] = true;
            }
            return indexGate_Max;
        }
        
        public static bool[] onePolygonGate(List<Polygon> polygon, double[][] arrayInGate)
        {
            double x, y;
           
            int TotalDataLength = arrayInGate.Length;
            bool[] indexGate = new bool[TotalDataLength];

            for (int j = 0; j < TotalDataLength; j++)
            {
                x = arrayInGate[j][0]; 
                y = arrayInGate[j][1];  
                indexGate[j] = false;
                if (polygon != null)
                {
                    if (polygon[0].IsInsidePoly(x, y))
                        indexGate[j] = true;
                    else
                        indexGate[j] = false;
                }
            }
            return indexGate;
        }

        public static void calculateDynamicGates(List<Polygon> polygons, double[][] arrData)
        {
            int i = 0;
            List<double[]> totalData = new List<double[]>();
            for (i = 0; i < arrData.Length; i ++)
            {
                totalData.Add(arrData[i]);
            }

            for (i = 0; i < 3; i++)
            {
                CustomCluster.Global.CELL_CENTER[i] = CustomCluster.Global.GetCentroid(polygons[i].poly);
            }



        }
    }
}
