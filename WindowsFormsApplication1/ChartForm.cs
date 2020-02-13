using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
//using ChartDirector;
using System.Windows.Forms.DataVisualization.Charting;
using ClusterAPI;


namespace WindowsFormsApplication1
{
    public partial class ChartForm : Form //partial
    {
        double[][] centers = null;
        Polygon[] polygons = null;
 
        string channelNomenclature = "old_names";// "middleaged_names"; //"new_names";//
        //PaintEventArgs pEventArg;

        string FSC1_H, SSC_H;

        string Loaded_TotalFileName;
        string WBC_file_type;
        //string Gating_Type;

        //string Loaded_filePath;
        //string Loaded_fileName;
        //bool[] indexGatesTotal;// = new bool[TotalDataLength];

        // string filePath_gates_default = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Fixed gating";
        // string FixedGatesLocationName = "Fixed Gating folder location.txt";
        // string gates_filePath = "Fixed Gating folder location.txt";
        // File.WriteAllText(gates_filePath, filePath_gates);


        string filePath_gates = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Fixed gating";//null;// null;//
        string starting_filePath = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Data/MGH";
        bool drawPolygons = false;
       // string GateEOS_file = Path.Combine(filePath_gates, Path.GetFileName(fileName_GateEOS)); 
        // List<Polygon> polygons = new List<Polygon>();

        public ChartForm()
        {
            InitializeComponent();
            chartData.PostPaint += chartData_PostPaint;
                       
            try
            {
                string FixedGatesLocationName = File.ReadAllText("Fixed Gating folder location.txt");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(String.Format("Select a folder with fixed Gates"));
                string filePath_gates = Set_FixedGatingFolder();
            }
                                   

            //if (filePath_gates == null)
            //{
            //   Set_FixedGatingFolder(); // object,EventArgs
            //}
        }

        void chartData_PostPaint(object sender, ChartPaintEventArgs e)
        {
            if (chartData.ChartAreas.Count > 0)
            {
                ChartArea ca = chartData.ChartAreas[0];
                if (polygons != null && drawPolygons)
                {
                    foreach (Polygon p in polygons)
                    {
                        p.DrawInChart(e.ChartGraphics.Graphics, ca.AxisX, ca.AxisY);
                    }
                }
            }

            //throw new NotImplementedException();
        }
 
        FlowCytometry.FCMeasurement sample;
         
        private void ChartForm_Load(object sender, EventArgs e) //private
        {
            // Create a new Mean-Shift algorithm for 3 dimensional samples

            string filePath_Test = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Csharp/Build4/WindowsFormsApplication1/bin/Debug/";
            string fileName_Test = "HL60_007.fcs";
            string fileTest = Path.Combine(filePath_Test, Path.GetFileName(fileName_Test));

            string filePath_RBC = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Data/MGH/RBC/141203_RBC/";
            string fileName_RBC = "t94491_01.fcs";
            string fileRBC = Path.Combine(filePath_RBC, Path.GetFileName(fileName_RBC));

            //  string filePath_WBC = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Data/MGH/WBC/140725/";
            //  string fileName_WBC = "H43069_CD16APC_20mW_01.fcs";

            /*            string filePath_WBC = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Data/MGH/CBC/160122_CBC";
                        string fileName_WBC = "wbc4_1.fcs";
            */

            string filePath_WBC = "F:\\VisualStudio\\C#\\Clustering\\TestData\\FCS";
            string fileName_WBC = "WBC_sample.fcs";

            string fileWBC = Path.Combine(filePath_WBC, Path.GetFileName(fileName_WBC));

            // may want to disable loading of a file on startup 
            //Loaded_TotalFileName
            // if (Loaded_TotalFileName != null)
            //  {
            sample = new FlowCytometry.FCMeasurement(fileWBC); //fileWBC

            foreach (String name in sample.ChannelsNames)
            {
                comboBox1.Items.Add(name);
                comboBox2.Items.Add(name);
            }
            // }

            /*            chartData.Dock = DockStyle.Fill;
                        ChartPanel.Controls.Add(chartData);
            */
        }


        private HashSet<double[]> GenerateDataSet(string channel1, string channel2)
        {
            HashSet<double[]> hs = new HashSet<double[]>();

            for (int i = 0; i < sample.Counts; i++) //sample.Counts    sample.Counts  10000
            {
                hs.Add(new double[2] { sample.Channels[channel1].Data.ElementAt(i), sample.Channels[channel2].Data.ElementAt(i) });
                //System.ArgumentNullException: 'Value cannot be null.  Parameter name: key'
                //System.Collections.Generic.KeyNotFoundException: 'The given key was not present in the dictionary.'
            }
            return hs;
        }

        private Color[] colors = new Color[11] { Color.Maroon, Color.Green, Color.Navy, Color.Magenta, Color.MediumTurquoise, Color.Crimson,
        Color.Gold, Color.Gray, Color.Indigo, Color.LawnGreen, Color.Cyan}; //MediumSlateBlue
        private void RebuildButton_Click(object sender, EventArgs e)
        {
            string channel1 = comboBox1.Text;
            string channel2 = comboBox2.Text;
            int number = 5; //4

            if (centers != null)
                number = centers.Length;

            if (!channel1.Equals(channel2))
            {
                // CLUSTERING
                bool hasClass = false;
                HashSet<double[]> _dataSet = GenerateDataSet(channel1, channel2);
                //SharpCluster.Agnes agnes = new SharpCluster.Agnes(_dataSet, hasClass);
                //SharpCluster.Clusters rawClusters = agnes.ExecuteClustering(SharpCluster.ClusterDistance.Strategy.CompleteLinkage, number);
                //SharpCluster.Cluster[] clusters = agnes.BuildFlatClustersFromHierarchicalClustering(rawClusters, rawClusters.GetClusters().Count());
                //  MessageBox.Show("Starting clustering");
                //MeanShift meanShift = new MeanShift()
                SharpCluster.Partitional.KMeans kmeans = new SharpCluster.Partitional.KMeans(_dataSet, hasClass);
                SharpCluster.Cluster[] clusters = kmeans.ExecuteClustering(number, centers);
                // MessageBox.Show("Done with clustering");
                //Converte clusters (hierarquico) para clusters (particional) como o k-means, para utilizar Dunn e Davies-Bouldin

                #region charts initialization
//                chartData.Annotations.Clear();
//                chartData.ChartAreas.Clear();
//                chartData.Legends.Clear();
                chartData.Series.Clear();
                //ChartArea chartArea1 = new ChartArea();
                //Legend legend1 = new Legend();
                //legend1.Name = "Legend1";
                //chartData.ChartAreas.Add(chartArea1);
                //chartData.Legends.Add(legend1);
                // this is the error
                chartData.Series.Clear();
                #endregion
                int index = 0;
                foreach (SharpCluster.Cluster cluster in clusters)
                {
                    string clusterNameInside = "InsideCluster" + cluster.Id;
                    chartData.Series.Add(clusterNameInside);
                    chartData.Series[clusterNameInside].ChartType = SeriesChartType.Point;
                    chartData.Series[clusterNameInside].MarkerSize = 2;
                    chartData.Series[clusterNameInside].MarkerStyle = MarkerStyle.Diamond;
                    chartData.Series[clusterNameInside].MarkerColor = colors[index];

                    string clusterNameOutside = "OutsideCluster" + cluster.Id;
                    chartData.Series.Add(clusterNameOutside);
                    chartData.Series[clusterNameOutside].ChartType = SeriesChartType.Point;
                    chartData.Series[clusterNameOutside].MarkerSize = 1;
                    chartData.Series[clusterNameOutside].MarkerStyle = MarkerStyle.Circle;
                    chartData.Series[clusterNameOutside].MarkerColor = colors[index];

                    foreach (SharpCluster.Pattern pattern in cluster.GetAllPatterns())
                    {
                        double x = pattern.GetAttribute(0);
                        double y = pattern.GetAttribute(1);
                        bool added = false;

                        if (polygons != null)
                        {
                            if (polygons.Length > 2)
                            {
                                if (polygons[1].IsInsidePoly(x, y))
                                {
                                    chartData.Series[clusterNameInside].Points.AddXY(x, y);
                                    added = true;
                                }
                            }
                        }
                        if (!added)
                            chartData.Series[clusterNameOutside].Points.AddXY(x, y);

                    }
                    index++;
                }

                #region unused coloring
                /*
                chartData.Series.Add("Vf2");
                chartData.Series["Vf2"].ChartType = SeriesChartType.Point;

                chartData.Series.Add("Vf3");
                chartData.Series["Vf3"].ChartType = SeriesChartType.Point;

                for (int i = 0; i < sample.Counts; i++)
                {
                    if (IsInsidePoly(sample.Channels[channel1].Data.ElementAt(i), sample.Channels[channel2].Data.ElementAt(i)))
                        chartData.Series["Vf3"].Points.AddXY(sample.Channels[channel1].Data.ElementAt(i), sample.Channels[channel2].Data.ElementAt(i));
                    else
                        chartData.Series["Vf2"].Points.AddXY(sample.Channels[channel1].Data.ElementAt(i), sample.Channels[channel2].Data.ElementAt(i));
                }
                chartData.Series["Vf2"].MarkerSize = 2;
                chartData.Series["Vf2"].MarkerStyle = MarkerStyle.Circle;
                chartData.Series["Vf2"].MarkerColor = Color.Maroon;                
                
                chartData.Series["Vf3"].MarkerSize = 2;
                chartData.Series["Vf3"].MarkerStyle = MarkerStyle.Circle;
                chartData.Series["Vf3"].MarkerColor = Color.Green;*/
                #endregion

                #region charts finish
                chartData.ChartAreas[0].AxisX.Title = channel1;
                chartData.ChartAreas[0].AxisY.Title = channel2;

                chartData.ChartAreas[0].AxisX.RoundAxisValues();
                chartData.ChartAreas[0].AxisY.RoundAxisValues();

                chartData.ChartAreas[0].AxisX.IsStartedFromZero = true;
                chartData.ChartAreas[0].AxisY.IsStartedFromZero = true;
                #endregion

                chartData.Invalidate();
                sample.WriteMetadata("meta.csv");
                string outputFileName = "Data Excel format.xlsx";
                sample.WriteExcelFile(outputFileName);
                //CalculateWBC();
                //WriteWBC_Counts("WBC_counts_test.csv");
            }
            else
            {
                MessageBox.Show("Channels are the same");
            }
        }

        private void btnLoadPolygons_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Comma separated values|*.csv";
            List<Polygon> polygons = new List<Polygon>();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                String[] lines = File.ReadAllLines(dlg.FileName);
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
                        polygons.Add(new Polygon(points, color));
                    }
                    this.polygons = polygons.ToArray();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnLoadClusters_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Comma separated values|*.csv";
            string clusterLineMessage;
            List<string> clusterMsg = new List<string>();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                centers = null;
                String[] lines = File.ReadAllLines(dlg.FileName);
                try
                {
                    centers = new double[lines.Length][];
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] cells = lines[i].Split(new char[] { ',' });
                        centers[i] = new double[2];
                        centers[i][0] = Convert.ToDouble(cells[0].Trim());
                        centers[i][1] = Convert.ToDouble(cells[1].Trim());
                        clusterLineMessage = String.Format("Loading cluster centers, c = ({0},{1})", centers[i][0], centers[i][1]);
                        clusterMsg.Add(clusterLineMessage);
                        //clusterLineMessages[i] = String.Format("Loading cluster centers, c = ({0},{1})", centers[i][0], centers[i][1]);
                        //clusterMsg.Add(clusterLineMessages[i]);
                    }
                    string[] joined_CC_report = clusterMsg.ToArray();
                    MessageBox.Show(String.Join(" \n", joined_CC_report));
                }
                catch (Exception ex)
                {
                    centers = null;
                    MessageBox.Show(ex.Message, "No loaded cluster centers");
                }
            }
        }

        public int[] CalculateWBC(string filePath, string fileName) // private void CalculateWBC() private void CalculateWBC() //static double[]
        {
            // load data: FCS, SSC channels
            // identify 3 cell types using polygons in file
            // Read Excel file, convert to array?

            int[] cellsLMG = new int[3];
            FSC1_H = FlowCytometry.FCMeasurement.GetChannelName("FCS1peak", channelNomenclature);
            SSC_H = FlowCytometry.FCMeasurement.GetChannelName("SSCpeak", channelNomenclature);
            string fileTot = Path.Combine(filePath, Path.GetFileName(fileName));
            FlowCytometry.FCMeasurement sampleFC;
            sampleFC = new FlowCytometry.FCMeasurement(fileTot);
            //var FileNameSplit = fileTot.Split('/', '.')[1];
            string FileNameSplit = fileName.Split('.')[0];


            HashSet<double[]> firstGate = GenerateDataSet(FSC1_H, SSC_H);
            double[][] arrayG1 = firstGate.ToArray();
            var firstGateLength = arrayG1.GetLength(0);

            //    string clusterNameLymphocytes = "Lymphocytes";

            HashSet<double[]>[] cytes = new HashSet<double[]>[polygons.Length];
            for (int k = 0; k < polygons.Length; k++)
            {
                cytes[k] = new HashSet<double[]>();
            }

            double x;
            double y;
            int Neutrophils;
            int LymphNum;
            int Monocytes;
            double[][] arrayCytes;
            for (int j = 0; j < firstGateLength; j++)//FCS
            {
                x = arrayG1[j][0];
                y = arrayG1[j][1];

                if (polygons != null)
                {
                    //if (polygons.Length > 2)
                    for (int k = 0; k < polygons.Length; k++)
                    {
                        if (polygons[k].IsInsidePoly(x, y))                   //  (Polygon.IsInsidePoly(x, y))
                        {
                            cytes[k].Add(new double[2] { x, y }); //[x][y]
                        }
                    }
                }
                arrayCytes = cytes[0].ToArray();
                Neutrophils = cytes[0].Count; //Granulocytes
                LymphNum = cytes[1].Count;
                Monocytes = cytes[2].Count;
                cellsLMG[0] = Neutrophils;
                cellsLMG[1] = LymphNum;
                cellsLMG[2] = Monocytes;
            }

            string[] nameList = new string[3] { "Neutrophils", "Lymphocytes", "Monocytes" };

            Dictionary<string, int> WBC_Dict = new Dictionary<string, int>();
            for (int i = 0; i < 3; i++)
            {
                string name = nameList[i];
                int value = cellsLMG[i];
                WBC_Dict.Add(name, value);
            }

            IReadOnlyDictionary<string, int> countWBC = (IReadOnlyDictionary<string, int>)WBC_Dict.ToDictionary(pair => pair.Key, pair => pair.Value);
            string writeFileName1 = string.Join("_", "WBC_counts", FileNameSplit);
            string writeFileName = string.Join(".", writeFileName1, "csv");

            WriteWBC_Counts(writeFileName, fileTot, countWBC); //

            #region some junk
            /*
            // LINQ =>                //meanValue = Math.Max(arrayLymph)
            //double[,] arrayLymph2 = Lymphocytes.ToArray();
            //            SharpCluster.Partitional.KMeans kmeans = new SharpCluster.Partitional.KMeans(_dataSet, hasClass);
            //     SharpCluster.Cluster[] clusters = kmeans.ExecuteClustering(number, centers);
            string writeFileName = string.Join("_", "WBC_counts",FileNameSplit,".csv");
            string writeFileName = string.Join("", FileNameSplit, "_WBC_counts.csv");
            var csv = new StringBuilder();
            string first = reader[0].ToString();
            string second = image.ToString();
            var newLine = string.Format("{0},{1}{2}", first, second, Environment.NewLine);
            csv.Append(newLine);
            WriteFilePath = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Csharp/Build9/WindowsFormsApplication1/bin/Debug/";
            File.WriteAllText(WriteFilePath, csv.ToString());
            */
            #endregion
            return cellsLMG;
        }

        private String Enquote(string str)
        {
            str = str.Replace("\"", "\\\"");
            if (str.Contains(","))
            {
                return String.Format("\"{0}\"", str);
            }
            else
                return str;
        }


        public void WriteWBC_Counts(string WriteFileName, string FCSfileName, IReadOnlyDictionary<string, int> countWBC)
        {
            using (StreamWriter writer = new StreamWriter(WriteFileName))
            {
                writer.WriteLine("Filename: {0}", FCSfileName);
                foreach (KeyValuePair<String, int> kvp in countWBC)
                {
                    writer.WriteLine(Enquote(kvp.Key) + "," + kvp.Value);
                }
            }
        }

        private void btnGating_Click(object sender, EventArgs e) // old btnFixedGating
        {
            // Create options for 5-diff
            // 3-diff
            // EOS (4-diff)
            // BASO
            // int[] WBC_analysis(string[] args, bool ouputExcel) 
            // Add options: Visualize before all = Gate0, Gate1, Gate2, Gate3.
            // Options Radio button?

            try
            {
                filePath_gates = File.ReadAllText("Fixed Gating folder location.txt");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(String.Format("Select a folder with fixed Gates"));
                filePath_gates = Set_FixedGatingFolder();
            }
          //  MessageBox.Show(String.Format("Btn Gating: filePath_gates = {0}", filePath_gates));

            string FSC1_H = FlowCytometry.FCMeasurement.GetChannelName("FCS1peak", channelNomenclature);
            string SSC_H = FlowCytometry.FCMeasurement.GetChannelName("SSCpeak", channelNomenclature);

            string filePath = null;
            string fileName = null;

            //string Output_appendix;
            //string Joined_output_filename;
            //string Total_output_filename;

            string fileTot;

            #region get file from dialog box
            if (Loaded_TotalFileName != null)
            {
                fileTot = Loaded_TotalFileName;
                filePath = Path.GetDirectoryName(fileTot);
                fileName = Path.GetFileName(fileTot);
            }
            else
            {
                // MessageBox.Show("Select a file");
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.InitialDirectory = starting_filePath; //GetDataPath
                    dlg.Filter = "FCS files|*.fcs";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        Loaded_TotalFileName = dlg.FileName;
                    }
                }
                FileNameBox.Text = Loaded_TotalFileName;
                fileTot = Loaded_TotalFileName;
            }

            #endregion


            #region generate dataset, initialize HashSets
            sample = new FlowCytometry.FCMeasurement(fileTot);

            bool OutputExcel = false; // true ;
            if (OutputExcel)
            {
                FlowCytometry.FCMeasurement.outputFCSasExcel(sample);
            }


            #endregion

            #region chart initialization
            //ChartArea chartArea1 = new ChartArea();
            //chartData.ChartAreas.Add(chartArea1); 
  
            //chartData.Annotations.Clear();
            //chartData.ChartAreas.Clear();
            //chartData.Legends.Clear();
            chartData.Series.Clear();

            string threeDiffGated = "threeDiffGated";
            string outsideFixedGates = "OutsideGates";

            chartData.Series.Add(threeDiffGated);
            chartData.Series[threeDiffGated].ChartType = SeriesChartType.Point;
            chartData.Series[threeDiffGated].MarkerSize = 1;
            chartData.Series[threeDiffGated].MarkerStyle = MarkerStyle.Circle; //Diamond
            chartData.Series[threeDiffGated].MarkerColor = Color.Blue;// colors[index];

            chartData.Series.Add(outsideFixedGates);
            chartData.Series[outsideFixedGates].ChartType = SeriesChartType.Point;
            chartData.Series[outsideFixedGates].MarkerSize = 1;
            chartData.Series[outsideFixedGates].MarkerStyle = MarkerStyle.Circle;
            chartData.Series[outsideFixedGates].MarkerColor = Color.Red; //colors[index];

            string Channel_X = FSC1_H; // default channels FSC vs SSC
            string Channel_Y = SSC_H;
            //MessageBox.Show(String.Format("Channels X,Y \n{0},{1}", FSC1_H, SSC_H));

            //chartData.ChartAreas[0].AxisX.Title = FSC1_H;
            //chartData.ChartAreas[0].AxisY.Title = SSC_H;

            //chartData.BringToFront();
            

            //  int index = 0; //loop over index

            /*
            string clusterNameInside = "InsideCluster" + cluster.Id;
            chartData.Series.Add(clusterNameInside);
            chartData.Series[clusterNameInside].ChartType = SeriesChartType.Point;
            chartData.Series[clusterNameInside].MarkerSize = 2;
            chartData.Series[clusterNameInside].MarkerStyle = MarkerStyle.Diamond;
            chartData.Series[clusterNameInside].MarkerColor = colors[index];
                        
            chartData.Series.Add(gatedCells);
            chartData.Series[gatedCells].ChartType = SeriesChartType.Point;
            chartData.Series[gatedCells].MarkerSize = 2;
            chartData.Series[gatedCells].MarkerStyle = MarkerStyle.Circle;
            chartData.Series[gatedCells].MarkerColor = Color.Red;

            chartData.Series.Add(gatedSinglets);
            chartData.Series[gatedSinglets].ChartType = SeriesChartType.Point;
            chartData.Series[gatedSinglets].MarkerSize = 2;
            chartData.Series[gatedSinglets].MarkerStyle = MarkerStyle.Circle;
            chartData.Series[gatedSinglets].MarkerColor = Color.Red;
            */
            #endregion

            //bool ProcessEOS = false;
            //string Gate1_file;
            //double[][] Gate1_array;
            //int count_Gate1out_Max;
            //List<Polygon> polygons;

            double x;
            double y;
            //int FSC1_H_max = sample.Channels[FSC1_H].Range;
            int SSC_H_max = sample.Channels[SSC_H].Range;

            sample = new FlowCytometry.FCMeasurement(Loaded_TotalFileName);
            string source_dir = Path.GetDirectoryName(Loaded_TotalFileName);
            HashSet<double[]> Gate1_hs = GenerateDataSet(FSC1_H, SSC_H);
            double[][] sampleGate_array = Gate1_hs.ToArray();
            int TotalDataLength = sampleGate_array.GetLength(0);

           // indexGate1_Max = new bool[TotalDataLength];
           // indexGate1 = new bool[TotalDataLength];
           // string Channel_X = FSC1_H; // default channels FSC vs SSC
           // string Channel_Y = SSC_H;            

            if (WBC_file_type == "BASO")
            {              
                gate_Basophils(sample);
            }
            else if (WBC_file_type == "3-diff")
            {
                gate_3diff(sample);
                /*
                HashSet<double[]> gatedData_hs = new HashSet<double[]>();
                HashSet<double[]> Gate2_hs = new HashSet<double[]>();
                HashSet<double[]> Gate3_hs = new HashSet<double[]>();

                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();

                int eps;
                int minNumPoints;
                eps = 160; // radius  
                minNumPoints = 200; // minimum number of points in the cluster
                            

                #region get Cells gate file location, Channel names
                // string filePath_gates = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Csharp/Build9/WindowsFormsApplication1/bin/Debug";

                string fileName_gatedCells = "gating Cells.csv";
                Gate1_file = Path.Combine(filePath_gates, Path.GetFileName(fileName_gatedCells));

                string diff3_ClusterCentersFile = "clusterCenters 3diff.csv";
                string diff3_ClusterCenters = Path.Combine(filePath_gates, Path.GetFileName(diff3_ClusterCentersFile));

                string FCS1_H = FlowCytometry.FCMeasurement.GetChannelName("FCS1peak", channelNomenclature);
                SSC_H = FlowCytometry.FCMeasurement.GetChannelName("SSCpeak", channelNomenclature);

                Gate1_array = sampleGate_array;
                //   MessageBox.Show(String.Format("Total data length = {0}", TotalDataLength));
                double[,] Array_XY = MeansClustering.MeansCluster.To2D(Gate1_array);
                double[] Array_X = new double[TotalDataLength];
                double[] Array_Y = new double[TotalDataLength];                      

                #endregion

                #region Kernel calculation

                #region add chart type
                string KernelPlot = "KDE plot";
                chartData.Series.Add(KernelPlot);
                chartData.Series[KernelPlot].ChartType = SeriesChartType.Point;
                chartData.Series[KernelPlot].MarkerSize = 2;
                chartData.Series[KernelPlot].MarkerStyle = MarkerStyle.Circle; //Diamond
                                                                            // chartData.Series[KernelPlot].MarkerColor = Color.Red;// colors[index];
                #endregion

                double sigma = 0.5; // RELATED TO bandwidth
                double sigma_2D = 3;
                double sigma_localDensity = 0.125; // RELATED TO bandwidth
                int nbins = 100;

                double[,] result1D = null;
                double[,] result2D = null;
                //double densityExpFactor = 

                bool solveKDE1D = false;
                bool solveKDE2D = false; //true; //
                bool localDensity = false;
                int nbinsX = nbins;
                int nbinsY = nbins;
                double NormalizationFactor = 0;
                double[] localDens;
                double DensityMax = 0;
                double denseMax;
                double denseMin;
                Color[] localColor;
                double[] array_1D = new double[nbinsX];

                if (solveKDE1D)
                {
                    result1D = MeansClustering.MeansCluster.KDE1d(Array_Y, sigma, nbins);            //double[] data, double sigma, int nbins)
                                                                                                     // return double[,]
                    for (int j = 0; j < nbins; j++)
                    {
                        x = result1D[j, 0]; // should equal result[j,0]  //Gate1_array = GateFinal_array, so using already created Gate1_array
                        y = result1D[j, 1];
                        chartData.Series[KernelPlot].Points.AddXY(x, y);                     //  y = result[j, 1];
                    }
                }
                else if (solveKDE2D)
                {
                    // array_1D = MeansClustering.MeansCluster.KDE2d(Array_XY, sigma, nbins);            //double[] data, double sigma, int nbins)

                    result2D = MeansClustering.MeansCluster.KDE2d(Array_XY, sigma_2D, nbins);
                    StringBuilder sb = new StringBuilder();
                    // b = reader.ReadByte();
                    // if (b != textSeparator)
                    // sb.Append((char)b);

                    double minX = Array_X.Min();
                    double maxX = Array_X.Max();
                    double minY = Array_Y.Min();
                    double maxY = Array_Y.Max();
                    // To get index use:       int indMax = Array.IndexOf(data, Max);

                    // Like MATLAB linspace(MIN, MAX, nbins);
                    double result2D_min = 1000000000;
                    double result2D_max = 0;
                    double[] fields = new double[nbinsX];

                    for (int j = 0; j < nbinsX; j++)
                    {
                        //x = minX + j * (maxX - minX) / nbinsX;
                        for (int k = 0; k < nbinsY; k++)
                        {
                            if (result2D_min > result2D[j, k])
                            {
                                result2D_min = result2D[j, k];
                            }
                            if (result2D_max < result2D[j, k])
                            {
                                result2D_max = result2D[j, k];
                            }
                            //   fields[k] = result2D[j, k];
                        }
                        //  sb.AppendLine(string.Join(",", fields));
                    }


                    for (int j = 0; j < nbinsX; j++)
                    {
                        x = minX + j * (maxX - minX) / nbinsX;
                        for (int k = 0; k < nbinsY; k++)
                        {
                            fields[k] = (result2D[j, k] - result2D_min) / (result2D_max - result2D_min);
                        }
                        sb.AppendLine(string.Join(",", fields));
                    }

                    File.WriteAllText("2D_N50xN50.csv", sb.ToString());
                    MessageBox.Show(String.Format("Min density = {0}, Max density = {1}", result2D_min, result2D_max));
                    // string readLine;
                    //   readLine = string.Join(", ", ScaledCrossSection1.ToString(), ScaledCrossSection2.ToString(), radius.ToString(), HC.ToString()); //String.Format(
                    // sb.AppendLine(readLine); //string.Join(",", fields));
                    //   Console.WriteLine(String.Format("For x={0}, cross-section1 = {1}, cross-section2 = {2}\n ", Math.Round(x, 3), Math.Round(CrossSection1, 3), Math.Round(CrossSection2, 3)));


                    //  int Ndata = Array_XY.GetLength(0);
                    int point_index = 0;
                    for (int j = 0; j < nbinsX; j++)
                    {
                        x = minX + j * (maxX - minX) / nbinsX;
                        for (int k = 0; k < nbinsY; k++)
                        {
                            y = minY + k * (maxY - minY) / nbinsY;
                            //     chartData.Series[KernelPlot].Points.AddXY(x, y);                     //  y = result[j, 1];

                            //   Color colorVal = HeatColorMap(result2D[j,k], result2D_min, result2D_max);
                            Color colorVal = RedColorMap(result2D[j, k], result2D_min, result2D_max);
                            //  Color colorVal = RedColorMap(KDE2d_Result[j, k], result_KDE_2D_min, result_KDE_2D_max);


                            chartData.Series[KernelPlot].Points.AddXY(x, y);

                            chartData.Series[KernelPlot].Points[point_index].Color = colorVal; //j*nbinsX+k
                            point_index++;

                        }
                    }

                    #region commented out
                    //   MessageBox.Show(String.Format("Length of data = {0}", Ndata));
                    //   MessageBox.Show("Starting calculation of local density");
                    // double calcTest = 1.0 / (Math.Sqrt(2 * Math.PI * sigma * sigma));
                    //  MessageBox.Show(String.Format("Test of norm factor = {0}", calcTest));

                    // int countsInLoop = MeansClustering.MeansCluster.localDensity(Array_XY, sigma);

                    //  localDens = MeansClustering.MeansCluster.localDensity(Array_XY, sigma_localDensity);

                    //   MessageBox.Show(String.Format("MaxDensity = {0}", denseMax));
                    // MessageBox.Show(String.Format("Counts in loop = {0}", countsInLoop));
                    // int arraySize = MeansClustering.MeansCluster.localDensity(Array_XY, sigma);
                    //MessageBox.Show(String.Format("arraySize = {0}", arraySize));

                    //double NormalizationFactor = result2D.Sum();
                    //  MessageBox.Show(String.Format("denseMin denseMax = {0},{1}", denseMin, denseMax));
                    #endregion
                }
                else if (localDensity)
                {
                    double[] localDensRescaled = new double[TotalDataLength];
                    localDens = MeansClustering.MeansCluster.localDensity(Array_XY, sigma_localDensity);//XY_norm

                    denseMax = localDens.Max();
                    denseMin = localDens.Min(); ///Ndata

                    for (int j = 0; j < TotalDataLength; j++)
                    {
                        x = Array_X[j];
                        y = Array_Y[j];

                        localDensRescaled[j] = (localDens[j] - denseMin) / (denseMax - denseMin);
                        // Color colorVal = HeatColorMap(localDens[j], denseMin, denseMax);
                        Color colorVal = RedColorMap(localDens[j], denseMin, denseMax);
                        //Color colorVal = HeatColorMap(localDensRescaled[j], 0, 1);

                        //  R = Convert.ToByte(255 * localDensRescaled[j]);
                        //  MessageBox.Show(String.Format("Color R = {0}", R));
                        chartData.Series[KernelPlot].Points.AddXY(x, y);
                        // chartData.Series[KernelPlot].Points.AddXY(j, localDensRescaled[j]);
                        chartData.Series[KernelPlot].Points[j].Color = colorVal;
                        //                    chartData.Series[KernelPlot].Points[j].Color = colorVal;
                    }


                    MessageBox.Show("Plotted local density, scaled");

                    
                    for (int j = 0; j < TotalDataLength; j++)
                    {
                        //  Color colorVal = HeatColorMap(localDens[j], denseMin, denseMax);
                        Color colorVal = HeatColorMap(localDensRescaled[j], 0, 1);
                        //MessageBox.Show(String.Format("Normalization Factor = {0}", colorVal));
                        //  Color colorVal = HeatColorMap(localDens[j], 0, 1);
                        x = Array_X[j]; // should equal result[j,0]  //Gate1_array = GateFinal_array, so using already created Gate1_array
                        y = Array_Y[j];

                        chartData.Series[KernelPlot].Points.AddXY(x, y);
                        chartData.Series[KernelPlot].Points[j].Color = colorVal;
                        //chartData.Series[KernelPlot].Points[0].Color = HeatColorMap(localDens[j], 0, 1);
                        NormalizationFactor += localDens[j]; // TotalDataLength
                    }

                    
                    //chartData.Invalidate();
                }
               

                 #endregion
             
                #region junk
                /*
                 *  chartData.Series.ResumeUpdates()
                  chartData.Series[1].Points.Item[0].YValues = Double(1) {MyNewValue, 0}
                   chartData.Series[1].Points[0].Color = Color.Red
                  chartData.DataBind()
                     chartData.Series.Invalidate()
                     chartData.Series.SuspendUpdates()


                            // = Z                                        // return double[,]
                for (int j = 0; j < nbinsY; j++) // row designation -> Y
                {
                    for (int k = 0; k < nbinsX; k++) // column designation -> X
                    {
                        localDensity = result2D[j, k];

                        NormalizationFactor += localDensity;
                        x = result2D[j, 0]; // should equal result[j,0]  //Gate1_array = GateFinal_array, so using already created Gate1_array
                        y = result2D[j, 1];
                        if (localDensity > DensityMax)
                        {
                            DensityMax = localDensity;
                        } //ContourLayer layer = c.addContourLayer(dataX, dataY, dataZ);
                        //  ContourLayer layer = chartData.addContourLayer(Array_X, Array_X, Array_X);

                        chartData.Series[KernelPlot].Points.AddXY(x, y);
                        chartData.Series[KernelPlot].Points[0].Color = HeatColorMap(localDensity, 0, 1);  //Color.Red
                                                                                                       // chartData.Series[KernelPlot].MarkerColor = HeatColorMap(localDensity, 0, 1);
                                                                                                       // colors[index];
                                                                                                       // HeatColorMap
                                                                                                       // color points according to density
                    }
                }

                

                // double MaxResult = result2D.Max();
                //  IEnumerable<int> allValues = result2D.Cast<int>();
                //   int MinResult = allValues.Min();
                //   int MaxResult = allValues.Max();

                //  MessageBox.Show(String.Format("Normalization Factor = {0}", NormalizationFactor));

                //            MessageBox.Show("Press a key...");
                //  Console.ReadKey(true); //Console.Read


                
                for (int j = 0; j < TotalDataLength; j++)   // for (int j = 0; j < Gate1Max_Length; j++)
                {
                    x = Gate1_array[j][0]; //Gate1_array = GateFinal_array, so using already created Gate1_array
                    y = Gate1_array[j][1];
                    //                chartData.Series[KernelPlot].Points.AddXY(x, y);
                }
    


                // Create a new Mean-Shift algorithm for 3 dimensional samples
                // MeanShift meanShift = new MeanShift()
                // {
                //     // Use a uniform kernel density
                //     Kernel = new UniformKernel(),
                //     Bandwidth = 2.0
                // };


                // MessageBox.Show(String.Format("Shift X = {0}", shiftX)); //
                // shiftXY = MeansClustering.MeansCluster.mShift(test_X, test_Y, test2D);

                //  MessageBox.Show(String.Format("Kernel density = {0}", shiftXY)); //shiftX
                //            MessageBox.Show(String.Format("Shifts X,Y = {0},{1}", shiftXY[0], shiftXY[1])); //shiftX
                //System.FormatException: 'Index (zero based) must be greater than or equal to zero and less than the size of the argument list.'
                
                #endregion

                polygons = loadPolygon(Gate1_file);

                #region analyze "Cells" selection gate
                bool[] indexGate1_Max = new bool[TotalDataLength];
                bool[] indexGate1 = new bool[TotalDataLength];
                bool[] indexGate2_Max = new bool[TotalDataLength];
                bool[] indexGate2 = new bool[TotalDataLength];
                bool[] indexGate3_Max = new bool[TotalDataLength];
                bool[] indexGate3 = new bool[TotalDataLength];
                bool[,] indexGateFinal = new bool[TotalDataLength, 2];



                indexGate1_Max = FlowCytometry.FCMeasurement.findMaxValues(sample, Gate1_array, FSC1_H, SSC_H);
                count_Gate1out_Max = CountBoolTrue(indexGate1_Max);

                for (int j = 0; j < TotalDataLength; j++)   // for (int j = 0; j < Gate1Max_Length; j++)
                {
                    x = Gate1_array[j][0];    //x = Gate1_arrayMax[j][0];
                    y = Gate1_array[j][1];    //y = Gate1_arrayMax[j][1];
                                              // indexGate1[j] = false;
                    if (polygons != null)
                    {
                        if (polygons[0].IsInsidePoly(x, y))
                        {
                            indexGate1[j] = true;
                            chartData.Series[threeDiffGated].Points.AddXY(x, y);//PLOT results of CELLS GATE 
                                                                             //chartData.Series[cluster_id].Points.AddXY(point.X, point.Y);
                        }
                        else
                        {
                            indexGate1[j] = false;
                            chartData.Series[outsideFixedGates].Points.AddXY(x, y);
                        }
                    }
                }
                //  indexGateFinal = FlowCytometry.FCMeasurement.GateArray(Gate1_array, Gate1_file);
                MessageBox.Show("Should have plotted Gate1");
                chartData.BringToFront();
                int count_Gate1out = CountBoolTrue(indexGate1);
                if (checkBoxGate1.Checked)
                {
                    chartData.BringToFront();
                    MessageBox.Show(String.Format("Total data length = {0} \n Inside Cells gate = {1}", TotalDataLength, count_Gate1out));
                }

                #endregion

                // GATE 2: Singlets

                #region get Singlets gate file location
                string filePath_gate2 = filePath_gates; // "C:/Users/begem/OneDrive/Desktop/General Fluidics/Csharp/Build9/WindowsFormsApplication1/bin/Debug";
                string fileName_gatedSinglets = "gating Singlets.csv";
                string Gate2_file = Path.Combine(filePath_gate2, Path.GetFileName(fileName_gatedSinglets));

                string FSC1_A = FlowCytometry.FCMeasurement.GetChannelName("FCS1area", channelNomenclature);

                Gate2_hs = GenerateDataSet(FSC1_A, FCS1_H);
                double[][] Gate2_array = Gate2_hs.ToArray();
                #endregion

                polygons = loadPolygon(Gate2_file);

                double[] SingletsFit = new double[3];
                SingletsFit = FlowCytometry.FCMeasurement.LinearRegression(Gate2_array);
                MessageBox.Show(String.Format("Slope = {0}, Y-intercept = {1}, R^2 = {2}", SingletsFit[0], SingletsFit[1], SingletsFit[2]));

                double[] X1Y1X2Y2 = new double[4];
                X1Y1X2Y2[0] = 0;
                X1Y1X2Y2[1] = 0;
                X1Y1X2Y2[2] = SingletsFit[0];
                X1Y1X2Y2[3] = SingletsFit[1];

                // DrawLineInt(PaintEventArgs e, X1Y1X2Y2); //System.Windows.Forms.
                // DrawLineInt(ChartPaintEventArgs e, X1Y1X2Y2);
                // ChartPaintEventArgs e
                // Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
                // e.Graphics.DrawLine(pen, X1Y1X2Y2[0],X1Y1X2Y2[1],X1Y1X2Y2[2],X1Y1X2Y2[3]);

                indexGate2_Max = FlowCytometry.FCMeasurement.findMaxValues(sample, Gate1_array, FSC1_A, FSC1_H);
                int count_Gate2Max = CountBoolTrue(indexGate2_Max);

                #region analyze Gate2 "Singlets"

                for (int j = 0; j < TotalDataLength; j++)
                {
                    x = Gate2_array[j][0];    //x = Gate1_arrayMax[j][0];
                    y = Gate2_array[j][1];    //y = Gate1_arrayMax[j][1];

                    if (polygons != null)
                    {
                        // indexGate2[j] = false; //setting default value (outside "singlets")
                        if (polygons[0].IsInsidePoly(x, y))
                        {
                            indexGate2[j] = true;
                            chartData.Series[threeDiffGated].Points.AddXY(x, y);
                        }
                        else
                        {
                            indexGate2[j] = false;
                            chartData.Series[outsideFixedGates].Points.AddXY(x, y);
                        }
                    }
                    else
                    {
                        throw new InvalidDataException("Singlets Gate not found");
                    }
                }

                int count_Gate2 = CountBoolTrue(indexGate2);
                if (checkBoxGate2.Checked)
                {
                    foreach (var series in chartData.Series)
                    {
                        series.Points.Clear();
                    }

                    chartData.BringToFront();
                    MessageBox.Show(String.Format("Total data length = {0} \n Inside Singlets gate = {1}", TotalDataLength, count_Gate2));
                }

                #endregion

                // GATE 4: Neutrophils,Lymphocytes, Monocytes
                // Channels for Gate 3 are the same as Gate 1 (FCS1 vs SSC)

                #region read Final Gate 
                //  string filePath_gates = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Csharp/Build9/WindowsFormsApplication1/bin/Debug";
                string fileName_GateFinal = "gating Cell Types.csv"; // gating2.csv"; //4
                string GateFinal_file = Path.Combine(filePath_gates, Path.GetFileName(fileName_GateFinal));
                #endregion

                polygons = loadPolygon(GateFinal_file);

                #region analyze GateFinal for positive selection by Cell size or Granularity (FSC vs SSC)

                FSC1_H_max = sample.Channels[FCS1_H].Range;
                SSC_H_max = sample.Channels[FCS1_H].Range;

                bool testCombinedGate = true;
                if (testCombinedGate)
                {
                    indexGateFinal = FlowCytometry.FCMeasurement.GateArray(Gate1_array, Gate1_file, FSC1_H_max, SSC_H_max);
                }
                else
                {
                    HashSet<double[]>[] cytes = new HashSet<double[]>[polygons.Count];
                    //
                    for (int j = 0; j < polygons.Count; j++)
                    {
                        cytes[j] = new HashSet<double[]>();
                    }

                    for (int j = 0; j < TotalDataLength; j++)//Gate2Max_Length 
                    {
                        x = Gate1_array[j][0]; //Gate1_array = GateFinal_array, so using already created Gate1_array
                        y = Gate1_array[j][1];
                        indexGateFinal[j, 1] = false;
                        if (polygons != null)
                        {
                            for (int k = 0; k < polygons.Count; k++)
                            {
                                if (polygons[k].IsInsidePoly(x, y))                   //  (Polygon.IsInsidePoly(x, y))
                                {
                                    cytes[k].Add(new double[2] { x, y }); //[x][y]
                                                                          // chartData.Series[threeDiffGated].Points.AddXY(x, y);
                                    indexGateFinal[j, 1] = true;
                                }
                                else
                                {
                                    //  chartData.Series[outsideFixedGates].Points.AddXY(x, y);
                                    //indexGateFinal[j] = false;
                                }
                            }
                        }
                    }
                }

                #endregion

                #region putting the gates together
                bool[] indexUnGated = new bool[TotalDataLength];
                indexGatesTotal = new bool[TotalDataLength];
                bool[] indexDataShown = new bool[TotalDataLength];
                bool[] indexDataShownNotGated = new bool[TotalDataLength];
                bool[] indexGatesG1G2 = new bool[TotalDataLength];
                bool[] indexGatesG1G2G3 = new bool[TotalDataLength];
                bool[] indexGatesG1G2Gfin = new bool[TotalDataLength];
                for (int j = 0; j < TotalDataLength; j++)
                {
                    indexUnGated[j] = true;
                    indexDataShown[j] = indexGate1_Max[j] && indexGate2_Max[j] && indexGate3_Max[j];
                    indexGatesTotal[j] = indexDataShown[j] && indexGate1[j] && indexGate2[j] && indexGate3[j] && indexGateFinal[j, 1];
                    indexGatesG1G2[j] = indexGate1_Max[j] && indexGate1[j] && indexGate2_Max[j] && indexGate2[j];
                    indexGatesG1G2G3[j] = indexDataShown[j] && indexGate1[j] && indexGate2[j] && indexGate3[j];
                    indexGatesG1G2Gfin[j] = indexDataShown[j] && indexGate1[j] && indexGate2[j] && indexGateFinal[j, 1];
                    indexDataShownNotGated[j] = indexDataShown[j] && !indexGatesTotal[j];
                }
                int totalGatedCells = CountBoolTrue(indexGatesTotal);
                //int GateFinalCells = FlowCytometry.FCMeasurement.CountBoolTrue2D(indexGateFinal,1);

                bool[] Final_Gate = FlowCytometry.FCMeasurement.GetColumn(indexGateFinal, 1);

                //  Console.WriteLine(stopwatch.ElapsedMilliseconds);


                bool output_Gating_Stats = false;
                if (output_Gating_Stats)
                {
                    int GateFinalCells = CountBoolTrue(Final_Gate);
                    stopwatch.Stop();
                    // MessageBox.Show(String.Format("Time for fixed gating = {0}ms", stopwatch.ElapsedMilliseconds));
                    string Count_Ini = String.Format("Length of initial / raw data  = {0}", TotalDataLength);
                    string Count_G1Max = String.Format("Gate1 removed Max values count = {0}", count_Gate1out_Max);
                    string Count_G1 = String.Format("Gate1: FCS vs SSC (peak) Count = {0}", count_Gate1out);
                    string Count_G2Max = String.Format("Gate2 removed Max values count = {0}", count_Gate2Max);
                    string Count_G2 = String.Format("Gate2 Count FSC1 Peaks vs Area = {0}", count_Gate2);
                    string Count_GFinal = String.Format("Gate3 (FSC1 vs SSC) gated cells = {0}", GateFinalCells);
                    string Count_Total = String.Format("Total gated cells (G1*G2*G3) = {0}", totalGatedCells); //G3*
                    double calculatedExcludedPercent = 100 * (TotalDataLength - totalGatedCells) / TotalDataLength; //, 1 Math.Round((
                                                                                                                    //  calculatedExcludedPercent = Math.Round(calculatedExcludedPercent,1);
                    string excludedPercent = String.Format("Percent events gated out: {0}%\n" +
                        "Time for fixed gating: {1}ms", calculatedExcludedPercent, stopwatch.ElapsedMilliseconds);

                    string[] joined_report = new string[8] {Count_Ini, Count_G1Max, Count_G1, Count_G2Max, Count_G2,
                    Count_GFinal, Count_Total, excludedPercent}; //Count_G3Max, Count_G3,
                    MessageBox.Show(String.Join(" \n", joined_report));
                }

                #endregion

                #region plot Final Gate
                bool plotFixedGates = true;//  false; ///
                bool plotKmeansGated = false;
                //   bool plotClustering = !plotFixedGates;
                int countInside = 0;
                int countOutside = 0;

                if (plotFixedGates)
                {
                    stopwatch.Start();
                    for (int j = 0; j < TotalDataLength; j++)//Gate2Max_Length 
                    {
                        if (indexDataShown[j])
                        {
                            x = Gate1_array[j][0]; //Gate1_array = GateFinal_array, so using already created Gate1_array
                            y = Gate1_array[j][1];

                            if (indexGatesG1G2[j])
                            {
                                chartData.Series[threeDiffGated].Points.AddXY(x, y);
                                countInside++;
                            }
                            else
                            {
                                chartData.Series[outsideFixedGates].Points.AddXY(x, y);
                                countOutside++;
                            }
                        }
                    }
                    stopwatch.Stop();

                    MessageBox.Show(String.Format("Time for plotting = {0}", stopwatch.ElapsedMilliseconds));
                    MessageBox.Show(String.Format("Overall Gating: In = {0}, , out = {1}", countInside, countOutside));
                }
                else if (plotKmeansGated)
                {
                    int number = 5; //4
                    if (centers != null)
                    {
                        number = centers.Length;
                        MessageBox.Show("Loaded centers");
                    }
                    else
                    {
                        MessageBox.Show("Centers not loaded");
                    }

                    HashSet<double[]> firstGate = GenerateDataSet(FSC1_H, SSC_H);                  

                    double[][] arrayG1 = firstGate.ToArray();
                    // CLUSTERING
                    bool hasClass = false;

                    // HashSet<double[]> dataSet_hs = GenerateDataSet(channel1, channel2);

                    bool[] indexGatesUsed;
                    //= G1G2G3
                    if (checkBox_PrefilterData.Checked)
                    {
                        indexGatesUsed = indexGatesG1G2;
                    }
                    else
                    {
                        indexGatesUsed = indexUnGated;
                    }

                    for (int j = 0; j < TotalDataLength; j++)
                    {
                        x = arrayG1[j][0];
                        y = arrayG1[j][1];
                        if (indexGatesG1G2[j]) //G3
                        {
                            gatedData_hs.Add(new double[2] { x, y });
                        }
                    }

                    //  MessageBox.Show("Starting clustering inside loop");
                    SharpCluster.Partitional.KMeans kmeans = new SharpCluster.Partitional.KMeans(gatedData_hs, hasClass);
                    SharpCluster.Cluster[] clusters = kmeans.ExecuteClustering(number, centers);


                    //   MessageBox.Show("Done with clustering inside loop");
                    //Converte clusters (hierarquico) para clusters (particional) como o k-means, para utilizar Dunn e Davies-Bouldin

                    #region charts implementation
                    chartData.Annotations.Clear();
                    chartData.ChartAreas.Clear();
                    chartData.Legends.Clear();

                    // ChartArea chartArea1 = new ChartArea();
                    //Legend legend1 = new Legend();
                    //legend1.Name = "Legend1";
                    chartData.ChartAreas.Add(chartArea1);
                    //chartData.Legends.Add(legend1);

                    chartData.Series.Clear();

                    int index = 0;
                    foreach (SharpCluster.Cluster cluster in clusters)
                    {
                        string clusterNameInside = "InsideCluster" + cluster.Id;
                        chartData.Series.Add(clusterNameInside);
                        chartData.Series[clusterNameInside].ChartType = SeriesChartType.Point;
                        chartData.Series[clusterNameInside].MarkerSize = 1;
                        chartData.Series[clusterNameInside].MarkerStyle = MarkerStyle.Diamond;
                        chartData.Series[clusterNameInside].MarkerColor = colors[index];

                        string clusterNameOutside = "OutsideCluster" + cluster.Id;
                        //System.ArgumentException: 'A chart element with the name 'OutsideCluster' already exists in the 'SeriesCollection'.'
                        chartData.Series.Add(clusterNameOutside);
                        chartData.Series[clusterNameOutside].ChartType = SeriesChartType.Point;
                        chartData.Series[clusterNameOutside].MarkerSize = 1;
                        chartData.Series[clusterNameOutside].MarkerStyle = MarkerStyle.Circle;
                        chartData.Series[clusterNameOutside].MarkerColor = colors[index]; //Color.Red;//

                        foreach (SharpCluster.Pattern pattern in cluster.GetAllPatterns())
                        {
                            x = pattern.GetAttribute(0);
                            y = pattern.GetAttribute(1);
                            bool added = false;

                            if (polygons != null)
                            {
                                if (polygons.Count > 2)
                                {
                                    if (polygons[1].IsInsidePoly(x, y))
                                    {
                                        chartData.Series[clusterNameInside].Points.AddXY(x, y);
                                        added = true;
                                    }
                                }
                            }
                            if (!added)
                                chartData.Series[clusterNameOutside].Points.AddXY(x, y);

                        }
                        index++;
                    }

                    chartData.ChartAreas[0].AxisX.Title = FCS1_H;
                    chartData.ChartAreas[0].AxisY.Title = SSC_H;

                    chartData.ChartAreas[0].AxisX.RoundAxisValues();
                    chartData.ChartAreas[0].AxisY.RoundAxisValues();

                    chartData.ChartAreas[0].AxisX.IsStartedFromZero = true;
                    chartData.ChartAreas[0].AxisY.IsStartedFromZero = true;
                    #endregion

                }
                else
                {
                    double[][] arrayInit;
                    //   FlowCytometry.FCMeasurement sample = new FlowCytometry.FCMeasurement(@DataFile.FileName);
                    double[][] arrayG1;

                    //  FCS1_H = FlowCytometry.FCMeasurement.GetChannelName("FCS1peak", channelNomenclature);

                    // MessageBox.Show(String.Format("FSC1_H = {0}\n SSC_H = {1}", FSC1_H, SSC_H));
                    HashSet<double[]> filteredData_hs = GenerateDataSet(FSC1_H, SSC_H); //firstGate
                    List<double[]> filteredData = filteredData_hs.ToList();

                    // List<double[]> filteredData = filteredData_hs.ToList();
                    HashSet<double[]> filteredData2_hs = new HashSet<double[]>();
                    arrayInit = filteredData.ToArray();//arrayG1;// 
                                                       // HashSet<double[]> filteredData;

                    for (int i = 0; i < arrayInit.Length; i++)
                    {
                        if (indexGatesG1G2[i])
                        {
                            double xVal = arrayInit[i][0];//.GetAttribute(0);
                            double yVal = arrayInit[i][1];// filteredData2_hs[i].GetAttribute(1);
                            filteredData2_hs.Add(new double[2] { xVal, yVal });
                        }
                    }

                    arrayG1 = filteredData2_hs.ToArray();

                    Array_XY = MeansClustering.MeansCluster.To2D(arrayG1);
                    string[] XY_strArr = new string[arrayG1.Length];

                    for (int j = 0; j < arrayG1.Length; j++)
                    {
                        XY_strArr[j] = string.Join(",", Array_XY[j, 0], Array_XY[j, 1]);
                    }

                    //  File.WriteAllText("2D_N50xN50.csv", sb.ToString());

                    MeansShift ms = new MeansShift(XY_strArr); //sb.ToString()
                    MeansShift.ClearSeries(chartData);
                    MeansShift.visualizeRealData(chartData);

                    //-------------------------------
                    MyCustomDatasetItem[] featureData = { };

                    // Setting DataPoints
                    List<MyCustomDatasetItem> testPoints = new List<MyCustomDatasetItem>();
                    for (int i = 0; i < MeansShift.DataSet.Count; i++)
                    {
                        testPoints.Add(new MyCustomDatasetItem(
                            MeansShift.DataSet[i].X,
                            MeansShift.DataSet[i].Y
                            ));
                    }
                    featureData = testPoints.ToArray();
                    HashSet<MyCustomDatasetItem[]> clusters;
                    // Setting the distance method (euclidean)
                    var dbs = new DbscanAlgorithm<MyCustomDatasetItem>((x1, y1) => Math.Sqrt(((x1.X - y1.X) * (x1.X - y1.X)) + ((x1.Y - y1.Y) * (x1.Y - y1.Y))));
                    // Computing the clusters (params must be tested)

                    //------------------------- CHANGE PARAMS HERE -------------------------

                    dbs.ComputeClusterDbscan(allPoints: featureData,
                        epsilon: eps, minPts: minNumPoints, clusters: out clusters); // epsilon: 175, minPts: 25, clusters: out clusters); FOR WBC03

                    //-----------------------------------------------------------------------
                    // Plotting

                    MessageBox.Show(String.Format("Number of clusters = {0}", clusters.Count)); //Console.WriteLine
                    MeansShift.ClearSeries(chartData);
                    int cluster_id = 0;
                    int point_id;
                    double cluster_centerX;
                    double cluster_centerY;
                    foreach (MyCustomDatasetItem[] Cluster in clusters)
                    {
                        {
                            // Cluster.Centroid
                            string clusterName = String.Join("Cluster", cluster_id);// "Cluster" + cluster.Id;
                                                                                    // MessageBox.Show(String.Format("Cluster id = {0}\n Cluster number = {0}", cluster_id, clusterName));
                                                                                    // cluster_centerX = Cluster.Centroid.GetAttribute(0);
                                                                                    //  cluster_centerY = Cluster.Centroid.GetAttribute(1);

                            // MessageBox.Show(String.Format("Cluster number = {0}", clusterName));
                            chartData.Series.Add(clusterName);
                            chartData.Series[clusterName].ChartType = SeriesChartType.Point;
                            chartData.Series[clusterName].MarkerSize = 1;
                            chartData.Series[clusterName].MarkerStyle = MarkerStyle.Circle;
                            chartData.Series[clusterName].MarkerColor = colors[cluster_id];

                            foreach (MyCustomDatasetItem point in clusters.ElementAt(cluster_id))

                                chartData.Series[cluster_id].Points.AddXY(point.X, point.Y);

                        }

                        cluster_id++;
                    }
                    chartData.BringToFront();
                    MessageBox.Show("Press a button...");
                    int N_lymphocytes = 0;
                    int N_Neutrophils = 0;
                    int N_Monocytes = 0;

                    // Location of centroids found by DBscan
                    int cluster_DBscan = clusters.Count;
                    double[][] dbsCentroids = null;

                    HashSet<double[]> dbsCentroid_hs = new HashSet<double[]>();

                    bool applyKmeansClustering = true;
                    int clusterPointCount;
                    if (cluster_DBscan == 3)
                    {
                        int[] NLMcounts = new int[3]; // filter by location 
                                                      //                    N_lymphocytes++;


                        for (int i = 0; i < 3; i++)
                        {
                            double Xloc = clusters.ElementAt(i).Average(point => point.X);
                            double Yloc = clusters.ElementAt(i).Average(point => point.Y);
                            dbsCentroid_hs.Add(new double[2] { Xloc, Yloc });
                            clusterPointCount = clusters.ElementAt(i).Length;
                            NLMcounts[i] = clusterPointCount;
                            //centers //  Gate1_hsMax.Add(new double[2] { x, y });
                        }

                        dbsCentroids = dbsCentroid_hs.ToArray();


                        double Xloc1 = clusters.ElementAt(0).Average(point => point.X);
                        double Yloc1 = clusters.ElementAt(0).Average(point => point.Y);

                        double Xloc2 = clusters.ElementAt(1).Average(point => point.X);
                        double Yloc2 = clusters.ElementAt(1).Average(point => point.Y);

                        double Xloc3 = clusters.ElementAt(2).Average(point => point.X);
                        double Yloc3 = clusters.ElementAt(2).Average(point => point.Y);
                        MessageBox.Show(String.Format("Cluster1: Loc = ({0},{1}), N={2}" +
                            "\n Cluster2: Loc = ({3},{4}), N={5}" +
                            "\n Cluster3: Loc  = ({6},{7}), N={8}" +
                            "\n epsilon = {9}, min = {10}", +
                            Math.Round(Xloc1), Math.Round(Yloc1), NLMcounts[0], Math.Round(Xloc2), Math.Round(Yloc2), NLMcounts[1], Math.Round(Xloc3), Math.Round(Yloc3), NLMcounts[2], eps, minNumPoints));
                        // MessageBox.Show(String.Format("Number of clusters = {0}", clusters.Count)); //Console.WriteLine
                        if (applyKmeansClustering)
                        {
                            double xVal;
                            double yVal;
                            // MessageBox.Show("Applying Kmeans clustgering to 3-diff data");
                            //polygons

                            string centroidGating_3Diff = "gating Cell Types Centroids.csv";
                            string Gating_3Diff = Path.Combine(filePath_gates, Path.GetFileName(centroidGating_3Diff));
                            polygons = loadPolygon(Gating_3Diff);

                            //   centers = LoadClusterCenters(BasoClusterCenters);
                            int nCentroids = 12;
                            bool hasClass = false;
                            centers = FlowCytometry.FCMeasurement.LoadClusterCenters(diff3_ClusterCenters); // double[][] 
                            double[][] JoinedCentroidResult = dbsCentroids.Union(centers).ToArray();
                            SharpCluster.Partitional.KMeans kmeans = new SharpCluster.Partitional.KMeans(Gate1_hs, hasClass);
                            // SharpCluster.Cluster[] kMeansClusters = kmeans.ExecuteClustering(nCentroids, centers);
                            SharpCluster.Cluster[] kMeansClusters = kmeans.ExecuteClustering(nCentroids, JoinedCentroidResult);

                            int count_inside = 0;

                            foreach (SharpCluster.Cluster Cluster in kMeansClusters)
                            {
                                cluster_centerX = Cluster.Centroid.GetAttribute(0);
                                cluster_centerY = Cluster.Centroid.GetAttribute(1);

                                // classify clasters based on centroid being within fixed gates
                                if (polygons[0].IsInsidePoly(cluster_centerX, cluster_centerY))
                                {
                                    count_inside += Cluster.GetPatterns().Length;
                                    foreach (SharpCluster.Pattern pattern in Cluster.GetAllPatterns())
                                    {
                                        xVal = pattern.GetAttribute(0);
                                        yVal = pattern.GetAttribute(1);
                                        chartData.Series[threeDiffGated].Points.AddXY(xVal, yVal);
                                    }
                                }
                                else
                                {
                                    foreach (SharpCluster.Pattern pattern in Cluster.GetAllPatterns())
                                    {
                                        xVal = pattern.GetAttribute(0);
                                        yVal = pattern.GetAttribute(1);
                                        chartData.Series[outsideFixedGates].Points.AddXY(xVal, yVal);
                                    }
                                    // ASSIGN CLUSTER IDENTITY 2
                                    // DO NOT COUNT CELLS
                                }
                            }
                        }
                        chartData.BringToFront();
                        MessageBox.Show("Press a button...");
                    }
                    else if (cluster_DBscan == 2)
                    {
                        // get the points within the second cluster
                        //  MeansShift.DataSet = clusters.ElementAt(0).Select((point) => new ClusterAPI.DataPoint(point.X, point.Y)).ToList();
                    }

                    // get the points within the second cluster
                    MeansShift.DataSet = clusters.ElementAt(0).Select((point) => new ClusterAPI.DataPoint(point.X, point.Y)).ToList();


                    chartData.BringToFront();
                    MessageBox.Show("Press a key ...");


                    // clusters.Centroids
                    // System.ArgumentOutOfRangeException: 'Specified argument was out of the range of valid values.
                    // Draw first cluster (blue)
                    foreach (MyCustomDatasetItem point in clusters.ElementAt(1))
                    //System.ArgumentOutOfRangeException: 'Specified argument was out of the range of valid values.
                    {
                        chartData.Series[0].Points.AddXY(point.X, point.Y);
                        N_lymphocytes++;
                    }

                    //   count_inside += Cluster.GetPatterns().Length;

                    //------------------------- CHANGE PARAMS HERE -------------------------
                    // apply means shift on the dataset
                    HashSet<ClusterAPI.DataPoint> subClusters = MeansShift.fit_shift(bandwidth: 1200); //1610

                    int length_subclusters = subClusters.Count;
                    MessageBox.Show(String.Format("Number of subclusters found by MeansShift = {0}", length_subclusters));
                    // creating a new landmark which will be the average of the two centroids found by means shift
                    ClusterAPI.DataPoint landmark = new ClusterAPI.DataPoint(
                       subClusters.Average(point => point.X),
                       subClusters.Average(point => point.Y)
                       );

                    // rotate one of the centroids found by means shift by the new landmark (it will be considered as (0,0) ) 
                    // the angle is set to -37 and can be adjusted
                    ClusterAPI.DataPoint rotated = MeansShift.rotate_point(landmark.X, landmark.Y, -37, subClusters.ElementAt(1));

                    // get the hyperplane equation (the separator)
                    Equation eq = MeansShift.getHyperPlaneEquation(landmark, rotated);

                    // draw the data 
                    foreach (var item in MeansShift.DataSet)
                    {
                        if (item.Y > (eq.a * item.X + eq.b))
                        {
                            chartData.Series[1].Points.AddXY(
                                                         item.X,
                                                         item.Y);
                            N_Neutrophils++;
                        }
                        else
                        {
                            chartData.Series[2].Points.AddXY(
                                                     item.X,
                                                     item.Y);
                            N_Monocytes++;
                        }
                    }
                    MessageBox.Show(String.Format("N_Monocytes = {0},N_lymphocytes = {1}, N_Neutrophils = {2}", N_Monocytes, N_lymphocytes, N_Neutrophils));

                }
                #endregion

                //string fileTot = Path.Combine(filePath, Path.GetFileName(fileName));

                #region set combobox values (unused)
                //comboBox2.Text(set)
                // comboBox1.Text = comboBox1.Items.IndexOf("FSC1LG,Area"); // DisplayMember

                //1) By using "SelectedIndex"
                //            mycomboBox.SelectedIndex = mycomboBox.Items.IndexOf("value");
                //            OR
                //Combox1.SelectedIndex = Combox1.FindStringExact("value");
                //            2) By using "SelectedValue"
                //ComboBox1.SelectedItem = "value";
                #endregion

                string[] argumentsIn = new string[4];
                int[] NMLE = new int[4];
                argumentsIn[0] = fileTot; // full FCS filename with path an extension
                argumentsIn[1] = filePath_gates; // file path to folder with fixed gates
                argumentsIn[2] = channelNomenclature; //  type of nomeclature: "old_names", "new_names", "middleaged_names"
                argumentsIn[3] = "3-diff";//sampleType ; // type of analysis: "EOS", "BASO",

                bool ouputExcel = false;
                NMLE = FlowCytometry.FCMeasurement.WBC_analysis(argumentsIn, ouputExcel);
                MessageBox.Show(String.Format("NML Report: {0},{1},{2}\n", NMLE[0], NMLE[1], NMLE[2]));
                //CalculateWBC(filePath, fileName); // ERROR HERE for EOS
                //            sample.WriteExcelFile(outputFileName);
                MessageBox.Show("Finished fixed gating of 3-diff");
                */
                

            }
            else if (WBC_file_type == "EOS")
            {
                var TupleIn = gate_3diff(sample);

                bool[] NeutrophilsTF = TupleIn.Item2;

                int[] EOSreport = new int[4];
                EOSreport[0] = TupleIn.Item1[0];
                EOSreport[1] = TupleIn.Item1[1];
                EOSreport[2] = TupleIn.Item1[2];

                string FL_H = FlowCytometry.FCMeasurement.GetChannelName("FLpeak", channelNomenclature);
                int FL_H_max = sample.Channels[FL_H].Range;
            
                //bool plotFixedGates = true;//  false; ///
                //bool plotKmeansGated = false;
                //   bool plotClustering = !plotFixedGates;
                //int countInside = 0;
                //int countOutside = 0;

                HashSet<double[]> GateEOS_hs = GenerateDataSet(FL_H, SSC_H);
               // GenerateDataSet(FSC1_H, SSC_H);
                HashSet<double[]> Neutrophils_hs = new HashSet<double[]>();
                double[][] EOS_array = GateEOS_hs.ToArray();

                for (int i = 0; i < TotalDataLength; i++)
                {
                    x = EOS_array[i][0];
                    y = EOS_array[i][1];
                    if (NeutrophilsTF[i])
                    {
                        Neutrophils_hs.Add(new double[2] { x, y });
                    }
                }
                double[][] Neutrophils_array = Neutrophils_hs.ToArray();
                bool[,] indexGate_EOS = FlowCytometry.FCMeasurement.GateArray(Neutrophils_array, Loaded_TotalFileName, FL_H_max, SSC_H_max);
                //            bool[,] indexGate_EOS = GateArray(EOS_array, GateEOS_file, FSC1_H_max, SSC_H_max);
                bool[] EOS_Max = FlowCytometry.FCMeasurement.GetColumn(indexGate_EOS, 0); //getColumn
                bool[] EOS_Gate = FlowCytometry.FCMeasurement.GetColumn(indexGate_EOS, 1);
                int count_EOS_Max = CountBoolTrue(EOS_Max);
                int count_GateEOS = CountBoolTrue(EOS_Gate);
                EOSreport[3] = count_GateEOS;
                Console.WriteLine(String.Format("EOS (max): {0} \nInside EOS gate: {1}", count_EOS_Max, count_GateEOS));
            

                int[] WBC_counts = FlowCytometry.FCMeasurement.processEOS(Loaded_TotalFileName, filePath_gates, channelNomenclature); 
                int totalWBC = WBC_counts[0] + WBC_counts[1] + WBC_counts[2] + WBC_counts[3];

                double pctNeutrophils = 100 * WBC_counts[0] / totalWBC; // percent Neutrophils
                double pctLympocytes = 100 * WBC_counts[1] / totalWBC; // percent Lymphocytes
                double pctMonocytes = 100 * WBC_counts[2] / totalWBC;  // percent Monocytes
                double pctEosinophils = 100 * WBC_counts[3] / totalWBC;  // percent Eosinophils

                pctNeutrophils = Math.Round(pctNeutrophils, 1);
                pctLympocytes = Math.Round(pctLympocytes, 1);
                pctMonocytes = Math.Round(pctMonocytes, 1);
                pctEosinophils = Math.Round(pctEosinophils, 1);

                Console.WriteLine(String.Format("4-diff analysis for {0}: \n" +
                               "Total WBC: {1}\n" +
                               "Neutrophils: {2}, {3}%\n" +
                               "Monocytes: {4}, {5}%\n" +
                               "Lymphocytes: {6}, {7}%\n" +
                               "Eosinophils: {8}, {9}%\n", Loaded_TotalFileName, totalWBC, WBC_counts[0], pctNeutrophils,
                                WBC_counts[1], pctLympocytes, WBC_counts[2], pctMonocytes, WBC_counts[3], pctEosinophils));
                
                /*
                //  ProcessEOS = true;
                //  MessageBox.Show("Process EOS passed to Fixed btn");

                string fileName_GateEOS = "gating Eosinophils.csv";
                string GateEOS_file = Path.Combine(filePath_gates, Path.GetFileName(fileName_GateEOS));

                polygons = loadPolygon(GateEOS_file);
                bool[] indexGateEOS = new bool[TotalDataLength];
                string FL_H = FlowCytometry.FCMeasurement.GetChannelName("FLpeak", channelNomenclature);
                MessageBox.Show(String.Format("Channel names:FL_H = {0}\n" +
                                              "              SSC_H = {1}\n", FL_H, SSC_H));

                GateEOS_hs = GenerateDataSet(FL_H, SSC_H);
                double[][] Gate_EOS = GateEOS_hs.ToArray();

                bool[] indexGateEOS_Max = FlowCytometry.FCMeasurement.findMaxValues(sample, Gate_EOS, FL_H, SSC_H);
                int count_GateEOSMax = CountBoolTrue(indexGateEOS_Max);
                

                //                indexGateEOS = FlowCytometry.FCMeasurement.GateArray(Gate_EOS, GateEOS_file);
                //  int EOS_count = 0;
                // foreach (var series in chartData.Series)
                // {
                //     series.Points.Clear();
                // }

                for (int j = 0; j < TotalDataLength; j++)
                {
                    x = Gate_EOS[j][0];    //x = Gate1_arrayMax[j][0];
                    y = Gate_EOS[j][1];    //y = Gate1_arrayMax[j][1];

                    if (polygons != null)
                    {
                        if (polygons[0].IsInsidePoly(x, y))
                        {
                            indexGateEOS[j] = true;
                            chartData.Series[threeDiffGated].Points.AddXY(x, y); // EOS_count++;
                        }
                        else
                        {
                            indexGateEOS[j] = false;
                            chartData.Series[outsideFixedGates].Points.AddXY(x, y);
                        }
                    }
                    else
                    {
                        throw new InvalidDataException("Eosinophils Gate not found");
                    }
                }
                chartData.BringToFront();
                int count_GateEOS = CountBoolTrue(indexGateEOS);
                MessageBox.Show(String.Format("Total data length = {0} \n Inside EOS gate = {1}", TotalDataLength, count_GateEOS));
                // LymphNum = cytes[1].Count;      

                #region charts finish

                chartData.ChartAreas[0].AxisX.Title = Channel_X;
                chartData.ChartAreas[0].AxisY.Title = Channel_Y;

                chartData.ChartAreas[0].AxisX.RoundAxisValues();
                chartData.ChartAreas[0].AxisY.RoundAxisValues();

                chartData.ChartAreas[0].AxisX.IsStartedFromZero = true;
                chartData.ChartAreas[0].AxisY.IsStartedFromZero = true;

                chartData.BringToFront();
                chartData.Invalidate();
                #endregion
                */
            }

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private List<Polygon> loadPolygon(string fileName)
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
                this.polygons = polygons.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return polygons;
        }

        private int CountBoolTrue(bool[] testArray)
        {
            bool val = true;
            return testArray.Count(c => c == val);
        }

        /*
        private static double[][] LoadClusterCenters(string fileName)
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
            catch (Exception ex)
            {
                centers = null;
            }
            return centers;

        }
        */                          

        private void gate_Basophils(FlowCytometry.FCMeasurement sample)
        {
            //  DynamicGating.Checked();
            // MessageBox.Show("Inside Gating Basophils Function");
            //if (Gating_Type == "DynamicGating")//FixedGating
            // {
            //    MessageBox.Show("Dynamic Gating currently not enabled");
            //    return;
            //}                
            string fileName = Path.GetFileNameWithoutExtension(Loaded_TotalFileName);
            string source_dir = Path.GetDirectoryName(Loaded_TotalFileName);

            #region get BASO gate file

            string fileName_Basophils = "gating Basophils.csv";
            string Gate1_file = Path.Combine(filePath_gates, Path.GetFileName(fileName_Basophils));

            string Basophiles_ClusterCenters = "clusterCenters Baso2.csv";
            string BasoClusterCenters = Path.Combine(filePath_gates, Path.GetFileName(Basophiles_ClusterCenters));

            List<Polygon> polygons = loadPolygon(Gate1_file);

            FSC1_H = FlowCytometry.FCMeasurement.GetChannelName("FCS1peak", channelNomenclature);
            SSC_H = FlowCytometry.FCMeasurement.GetChannelName("SSCpeak", channelNomenclature);

            HashSet<double[]> Gate1_hs = GenerateDataSet(FSC1_H, SSC_H);
            double[][] Gate1_array = Gate1_hs.ToArray();

            #endregion

            #region remove max values
            bool[] indexGate1_Max = FlowCytometry.FCMeasurement.findMaxValues(sample, Gate1_array, FSC1_H, SSC_H);
            int count_Gate1out_Max = FlowCytometry.FCMeasurement.CountBoolTrue(indexGate1_Max);
            #endregion

            //  Want to replace explicit loops below with a function of polygon gate
            //  indexGate1 = FlowCytometry.FCMeasurement.onePolygonGate(polygons, Gate1_array); 
            #region perform fixed and dynamic gating
            int TotalDataLength = Gate1_array.GetLength(0);
            double x, y;
            bool[] indexGate1 = new bool[TotalDataLength];
            for (int j = 0; j < TotalDataLength; j++)   // for (int j = 0; j < Gate1Max_Length; j++)
            {
                x = Gate1_array[j][0];    //x = Gate1_arrayMax[j][0];
                y = Gate1_array[j][1];    //y = Gate1_arrayMax[j][1];
                indexGate1[j] = false;
                if (polygons != null)
                {
                    if (polygons[0].IsInsidePoly(x, y))
                        indexGate1[j] = true;
                    else
                        indexGate1[j] = false;
                }
            }

            int Baso_FixedGateCount = CountBoolTrue(indexGate1);// indexGate1.Count(c => c == true);     

            HashSet<double[]> filteredData_hs = new HashSet<double[]>();

            for (int i = 0; i < Gate1_array.Length; i++)
            {
                if (indexGate1_Max[i])
                {
                    double xVal = Gate1_array[i][0];
                    double yVal = Gate1_array[i][1];

                    filteredData_hs.Add(new double[2] { xVal, yVal });
                }
            }

            double[][] centers = FlowCytometry.FCMeasurement.LoadClusterCenters(BasoClusterCenters);

            int numberOfClusters = 11; //Number of clusters for the debris (10) and Basopihils (1)
                                       // This allows to pick out all of the clustered debris

            bool hasClass = false;
            SharpCluster.Partitional.KMeans kmeans = new SharpCluster.Partitional.KMeans(filteredData_hs, hasClass);
            SharpCluster.Cluster[] clusters = kmeans.ExecuteClustering(numberOfClusters, centers);

            bool colorByClusterCentroid = true; //dynamic gating using centroids of K-means clusters
            int count_inside = 0;
            if (!colorByClusterCentroid)
            {
                // MessageBox.Show("Plotting without selection by centroids");
                int index = 0;
                foreach (SharpCluster.Cluster Cluster in clusters)
                {
                    string clusterNameInside = "InsideCluster" + Cluster.Id;
                    chartData.Series.Add(clusterNameInside);
                    chartData.Series[clusterNameInside].ChartType = SeriesChartType.Point;
                    chartData.Series[clusterNameInside].MarkerSize = 2;
                    chartData.Series[clusterNameInside].MarkerStyle = MarkerStyle.Circle;
                    chartData.Series[clusterNameInside].MarkerColor = colors[index];

                    string clusterNameOutside = "OutsideCluster" + Cluster.Id;
                    chartData.Series.Add(clusterNameOutside);
                    chartData.Series[clusterNameOutside].ChartType = SeriesChartType.Point;
                    chartData.Series[clusterNameOutside].MarkerSize = 1;
                    chartData.Series[clusterNameOutside].MarkerStyle = MarkerStyle.Circle;
                    chartData.Series[clusterNameOutside].MarkerColor = colors[index];

                    //double centerX;
                    //double centerY;

                    foreach (SharpCluster.Pattern pattern in Cluster.GetAllPatterns())
                    {
                        x = pattern.GetAttribute(0);
                        y = pattern.GetAttribute(1);

                        //        public Pattern Centroid
                        //      centerX = cluster.Pattern;
                        //SharpCluster.Pattern pattern cluster.Centroid(0);

                        bool added = false;

                        if (polygons != null)
                        {
                            if (polygons[0].IsInsidePoly(x, y))
                            {
                                chartData.Series[clusterNameInside].Points.AddXY(x, y);
                                added = true;
                            }

                        }
                        if (!added)
                            chartData.Series[clusterNameOutside].Points.AddXY(x, y);
                    }
                    index++;
                }
            }
            else if (colorByClusterCentroid)
            {
                // MessageBox.Show("Plotting WITH selection by centroids");
                int index = 0;
                double cluster_centerX;
                double cluster_centerY;
                foreach (SharpCluster.Cluster Cluster in clusters)
                {
                    string clusterNameInside = "InsideCluster" + Cluster.Id;
                    chartData.Series.Add(clusterNameInside);
                    chartData.Series[clusterNameInside].ChartType = SeriesChartType.Point;
                    chartData.Series[clusterNameInside].MarkerSize = 2;
                    chartData.Series[clusterNameInside].MarkerStyle = MarkerStyle.Circle;
                    chartData.Series[clusterNameInside].MarkerColor = colors[index];

                    cluster_centerX = Cluster.Centroid.GetAttribute(0);
                    cluster_centerY = Cluster.Centroid.GetAttribute(1);

                    // classify clasters based on centroid being within fixed gates
                    if (polygons[0].IsInsidePoly(cluster_centerX, cluster_centerY))
                    {
                        count_inside += Cluster.GetPatterns().Length;
                    }

                    foreach (SharpCluster.Pattern pattern in Cluster.GetAllPatterns())
                    {
                        x = pattern.GetAttribute(0);
                        y = pattern.GetAttribute(1);

                        chartData.Series[clusterNameInside].Points.AddXY(x, y);
                    }
                    index++;
                }

            }
            #endregion

            #region charts finish
            string Channel_X = FSC1_H;
            string Channel_Y = SSC_H;
            chartData.ChartAreas[0].AxisX.Title = Channel_X;
            chartData.ChartAreas[0].AxisY.Title = Channel_Y;

            chartData.ChartAreas[0].AxisX.RoundAxisValues();
            chartData.ChartAreas[0].AxisY.RoundAxisValues();

            chartData.ChartAreas[0].AxisX.IsStartedFromZero = true;
            chartData.ChartAreas[0].AxisY.IsStartedFromZero = true;

            chartData.BringToFront();
            chartData.Invalidate();
            #endregion

            #region write text Output
            double percentBasophiles = 100 * count_inside / count_Gate1out_Max; //Math.Round(),1);
                                                                                //  MessageBox.Show(String.Format("Number of Basophiles in cluster = {0} \n Percent of sample = {1}", count_inside, percentBasophiles));

            string BasophilsReport = String.Format("Number of points (w/o Max values) in dataset = {0}" +
              "\n# Basophils in cluster = {1}" +
              "\n # Basophils fixed gate = {2}", count_Gate1out_Max, count_inside, Baso_FixedGateCount);
            MessageBox.Show(BasophilsReport);      

            string Output_appendix = "_Baso_output.txt";
            string Joined_output_filename = string.Join(",", fileName, Output_appendix);
            string Total_output_filename = Path.Combine(source_dir, Joined_output_filename);

            File.WriteAllText(@Total_output_filename, BasophilsReport); 
            #endregion
        }
        
        public Tuple<int[], bool[]> gate_3diff(FlowCytometry.FCMeasurement sample) //static 
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();

            int eps;
            int minNumPoints;
            eps = 160; // radius  
            minNumPoints = 200; // minimum number of points in the cluster

            #region get Cells gate file location, Channel names
            // string filePath_gates = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Csharp/Build9/WindowsFormsApplication1/bin/Debug";

          //  MessageBox.Show(String.Format("filePath_gates = {0}", filePath_gates));

            string fileName_gatedCells = "gating Cells.csv";
            string Gate1_file = Path.Combine(filePath_gates, Path.GetFileName(fileName_gatedCells));

            string diff3_ClusterCentersFile = "clusterCenters 3diff.csv";
            string diff3_ClusterCenters = Path.Combine(filePath_gates, Path.GetFileName(diff3_ClusterCentersFile));

            string FSC1_H = FlowCytometry.FCMeasurement.GetChannelName("FCS1peak", channelNomenclature);
            string SSC_H = FlowCytometry.FCMeasurement.GetChannelName("SSCpeak", channelNomenclature);

            
            string source_dir = Path.GetDirectoryName(Loaded_TotalFileName);
           
            #region charts re-initialization
           
            //ChartArea chartArea1 = new ChartArea();
            //chartData.ChartAreas.Add(chartArea1);

            //chartData.Annotations.Clear();          
            //chartData.Legends.Clear();
            chartData.Series.Clear();

            string threeDiffGated = "threeDiffGated";
            string outsideFixedGates = "OutsideGates";

            chartData.Series.Add(threeDiffGated);
            chartData.Series[threeDiffGated].ChartType = SeriesChartType.Point;
            chartData.Series[threeDiffGated].MarkerSize = 1;
            chartData.Series[threeDiffGated].MarkerStyle = MarkerStyle.Circle; //Diamond
            chartData.Series[threeDiffGated].MarkerColor = Color.Blue;// colors[index];

            chartData.Series.Add(outsideFixedGates);
            chartData.Series[outsideFixedGates].ChartType = SeriesChartType.Point;
            chartData.Series[outsideFixedGates].MarkerSize = 1;
            chartData.Series[outsideFixedGates].MarkerStyle = MarkerStyle.Circle;
            chartData.Series[outsideFixedGates].MarkerColor = Color.Red; //colors[index];

            #endregion


            HashSet<double[]> Gate1_hs = GenerateDataSet(FSC1_H, SSC_H);
            HashSet<double[]> gatedData_hs = new HashSet<double[]>();
            HashSet<double[]> Gate2_hs = new HashSet<double[]>();
            HashSet<double[]> Gate3_hs = new HashSet<double[]>();
            HashSet<double[]> GateEOS_hs = new HashSet<double[]>();

            double[][] Gate1_array = Gate1_hs.ToArray();
            int TotalDataLength = Gate1_array.GetLength(0);
            
            //   MessageBox.Show(String.Format("Total data length = {0}", TotalDataLength));
            double[,] Array_XY = MeansClustering.MeansCluster.To2D(Gate1_array);
            double[] Array_X = new double[TotalDataLength];
            double[] Array_Y = new double[TotalDataLength];

            //XY_norm
            for (int i = 0; i < TotalDataLength; i++)//Array_XY.Length
            {
                Array_X[i] = Array_XY[i, 0];
                Array_Y[i] = Array_XY[i, 1];
                //System.IndexOutOfRangeException: 'Index was outside the bounds of the array.'
            }
                      
            bool[] indexGate1_Max = new bool[TotalDataLength];
            bool[] indexGate1 = new bool[TotalDataLength];
            bool[] indexGate2_Max = new bool[TotalDataLength];
            bool[] indexGate2 = new bool[TotalDataLength];
            bool[] indexGate3_Max = new bool[TotalDataLength];
            bool[] indexGate3 = new bool[TotalDataLength];
            bool[,] indexGateFinal = new bool[TotalDataLength,2];
            #endregion
            double x, y;




            #region Kernel calculation

            #region add chart type
            string KernelPlot = "KDE plot";
            chartData.Series.Add(KernelPlot);
            chartData.Series[KernelPlot].ChartType = SeriesChartType.Point;
            chartData.Series[KernelPlot].MarkerSize = 2;
            chartData.Series[KernelPlot].MarkerStyle = MarkerStyle.Circle; //Diamond
                                                                        // chartData.Series[KernelPlot].MarkerColor = Color.Red;// colors[index];
            #endregion

            double sigma = 0.5; // RELATED TO bandwidth
            double sigma_2D = 3;
            double sigma_localDensity = 0.125; // RELATED TO bandwidth
            int nbins = 100;

            double[,] result1D = null;
            double[,] result2D = null;
            //double densityExpFactor = 

            bool solveKDE1D = false;
            bool solveKDE2D = false; //true; //
            bool localDensity = false;
            int nbinsX = nbins;
            int nbinsY = nbins;
            //double NormalizationFactor = 0;
            double[] localDens;
            //double DensityMax = 0;
            double denseMax;
            double denseMin;
            //Color[] localColor;
            double[] array_1D = new double[nbinsX];

            if (solveKDE1D)
            {
                result1D = MeansClustering.MeansCluster.KDE1d(Array_Y, sigma, nbins);            //double[] data, double sigma, int nbins)
                                                                                                 // return double[,]
                for (int j = 0; j < nbins; j++)
                {
                    x = result1D[j, 0]; // should equal result[j,0]  //Gate1_array = GateFinal_array, so using already created Gate1_array
                    y = result1D[j, 1];
                    chartData.Series[KernelPlot].Points.AddXY(x, y);                     //  y = result[j, 1];
                }
            }
            else if (solveKDE2D)
            {
                // array_1D = MeansClustering.MeansCluster.KDE2d(Array_XY, sigma, nbins);            //double[] data, double sigma, int nbins)

                result2D = MeansClustering.MeansCluster.KDE2d(Array_XY, sigma_2D, nbins);
                StringBuilder sb = new StringBuilder();
                // b = reader.ReadByte();
                // if (b != textSeparator)
                // sb.Append((char)b);

                double minX = Array_X.Min();
                double maxX = Array_X.Max();
                double minY = Array_Y.Min();
                double maxY = Array_Y.Max();
                // To get index use:       int indMax = Array.IndexOf(data, Max);

                // Like MATLAB linspace(MIN, MAX, nbins);
                double result2D_min = 1000000000;
                double result2D_max = 0;
                double[] fields = new double[nbinsX];

                for (int j = 0; j < nbinsX; j++)
                {
                    //x = minX + j * (maxX - minX) / nbinsX;
                    for (int k = 0; k < nbinsY; k++)
                    {
                        if (result2D_min > result2D[j, k])
                        {
                            result2D_min = result2D[j, k];
                        }
                        if (result2D_max < result2D[j, k])
                        {
                            result2D_max = result2D[j, k];
                        }
                        //   fields[k] = result2D[j, k];
                    }
                    //  sb.AppendLine(string.Join(",", fields));
                }


                for (int j = 0; j < nbinsX; j++)
                {
                    x = minX + j * (maxX - minX) / nbinsX;
                    for (int k = 0; k < nbinsY; k++)
                    {
                        fields[k] = (result2D[j, k] - result2D_min) / (result2D_max - result2D_min);
                    }
                    sb.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText("2D_N50xN50.csv", sb.ToString());
                MessageBox.Show(String.Format("Min density = {0}, Max density = {1}", result2D_min, result2D_max));
                // string readLine;
                //   readLine = string.Join(", ", ScaledCrossSection1.ToString(), ScaledCrossSection2.ToString(), radius.ToString(), HC.ToString()); //String.Format(
                // sb.AppendLine(readLine); //string.Join(",", fields));
                //   Console.WriteLine(String.Format("For x={0}, cross-section1 = {1}, cross-section2 = {2}\n ", Math.Round(x, 3), Math.Round(CrossSection1, 3), Math.Round(CrossSection2, 3)));


                //  int Ndata = Array_XY.GetLength(0);
                int point_index = 0;
                for (int j = 0; j < nbinsX; j++)
                {
                    x = minX + j * (maxX - minX) / nbinsX;
                    for (int k = 0; k < nbinsY; k++)
                    {
                        y = minY + k * (maxY - minY) / nbinsY;
                        //     chartData.Series[KernelPlot].Points.AddXY(x, y);                     //  y = result[j, 1];

                        //   Color colorVal = HeatColorMap(result2D[j,k], result2D_min, result2D_max);
                        Color colorVal = RedColorMap(result2D[j, k], result2D_min, result2D_max);
                        //  Color colorVal = RedColorMap(KDE2d_Result[j, k], result_KDE_2D_min, result_KDE_2D_max);


                        chartData.Series[KernelPlot].Points.AddXY(x, y);

                        chartData.Series[KernelPlot].Points[point_index].Color = colorVal; //j*nbinsX+k
                        point_index++;

                    }
                }

                #region commented out
                //   MessageBox.Show(String.Format("Length of data = {0}", Ndata));
                //   MessageBox.Show("Starting calculation of local density");
                // double calcTest = 1.0 / (Math.Sqrt(2 * Math.PI * sigma * sigma));
                //  MessageBox.Show(String.Format("Test of norm factor = {0}", calcTest));

                // int countsInLoop = MeansClustering.MeansCluster.localDensity(Array_XY, sigma);

                //  localDens = MeansClustering.MeansCluster.localDensity(Array_XY, sigma_localDensity);

                //   MessageBox.Show(String.Format("MaxDensity = {0}", denseMax));
                // MessageBox.Show(String.Format("Counts in loop = {0}", countsInLoop));
                // int arraySize = MeansClustering.MeansCluster.localDensity(Array_XY, sigma);
                //MessageBox.Show(String.Format("arraySize = {0}", arraySize));

                //double NormalizationFactor = result2D.Sum();
                //  MessageBox.Show(String.Format("denseMin denseMax = {0},{1}", denseMin, denseMax));
                #endregion
            }
            else if (localDensity)
            {
                double[] localDensRescaled = new double[TotalDataLength];
                localDens = MeansClustering.MeansCluster.localDensity(Array_XY, sigma_localDensity);//XY_norm

                denseMax = localDens.Max();
                denseMin = localDens.Min(); ///Ndata

                for (int j = 0; j < TotalDataLength; j++)
                {
                    x = Array_X[j];
                    y = Array_Y[j];

                    localDensRescaled[j] = (localDens[j] - denseMin) / (denseMax - denseMin);
                    // Color colorVal = HeatColorMap(localDens[j], denseMin, denseMax);
                    Color colorVal = RedColorMap(localDens[j], denseMin, denseMax);
                    //Color colorVal = HeatColorMap(localDensRescaled[j], 0, 1);

                    //  R = Convert.ToByte(255 * localDensRescaled[j]);
                    //  MessageBox.Show(String.Format("Color R = {0}", R));
                    chartData.Series[KernelPlot].Points.AddXY(x, y);
                    // chartData.Series[KernelPlot].Points.AddXY(j, localDensRescaled[j]);
                    chartData.Series[KernelPlot].Points[j].Color = colorVal;
                    //                    chartData.Series[KernelPlot].Points[j].Color = colorVal;
                }


                MessageBox.Show("Plotted local density, scaled");

                /*
                for (int j = 0; j < TotalDataLength; j++)
                {
                    //  Color colorVal = HeatColorMap(localDens[j], denseMin, denseMax);
                    Color colorVal = HeatColorMap(localDensRescaled[j], 0, 1);
                    //MessageBox.Show(String.Format("Normalization Factor = {0}", colorVal));
                    //  Color colorVal = HeatColorMap(localDens[j], 0, 1);
                    x = Array_X[j]; // should equal result[j,0]  //Gate1_array = GateFinal_array, so using already created Gate1_array
                    y = Array_Y[j];

                    chartData.Series[KernelPlot].Points.AddXY(x, y);
                    chartData.Series[KernelPlot].Points[j].Color = colorVal;
                    //chartData.Series[KernelPlot].Points[0].Color = HeatColorMap(localDens[j], 0, 1);
                    NormalizationFactor += localDens[j]; // TotalDataLength
                }

                */

                //chartData.Invalidate();
            }
 
            #endregion

            #region junk
            /*
             *  chartData.Series.ResumeUpdates()
              chartData.Series[1].Points.Item[0].YValues = Double(1) {MyNewValue, 0}
               chartData.Series[1].Points[0].Color = Color.Red
              chartData.DataBind()
                 chartData.Series.Invalidate()
                 chartData.Series.SuspendUpdates()


                        // = Z                                        // return double[,]
            for (int j = 0; j < nbinsY; j++) // row designation -> Y
            {
                for (int k = 0; k < nbinsX; k++) // column designation -> X
                {
                    localDensity = result2D[j, k];

                    NormalizationFactor += localDensity;
                    x = result2D[j, 0]; // should equal result[j,0]  //Gate1_array = GateFinal_array, so using already created Gate1_array
                    y = result2D[j, 1];
                    if (localDensity > DensityMax)
                    {
                        DensityMax = localDensity;
                    } //ContourLayer layer = c.addContourLayer(dataX, dataY, dataZ);
                    //  ContourLayer layer = chartData.addContourLayer(Array_X, Array_X, Array_X);

                    chartData.Series[KernelPlot].Points.AddXY(x, y);
                    chartData.Series[KernelPlot].Points[0].Color = HeatColorMap(localDensity, 0, 1);  //Color.Red
                                                                                                   // chartData.Series[KernelPlot].MarkerColor = HeatColorMap(localDensity, 0, 1);
                                                                                                   // colors[index];
                                                                                                   // HeatColorMap
                                                                                                   // color points according to density
                }
            }

            */

            // double MaxResult = result2D.Max();
            //  IEnumerable<int> allValues = result2D.Cast<int>();
            //   int MinResult = allValues.Min();
            //   int MaxResult = allValues.Max();

            //  MessageBox.Show(String.Format("Normalization Factor = {0}", NormalizationFactor));

            //            MessageBox.Show("Press a key...");
            //  Console.ReadKey(true); //Console.Read


            /*
            for (int j = 0; j < TotalDataLength; j++)   // for (int j = 0; j < Gate1Max_Length; j++)
            {
                x = Gate1_array[j][0]; //Gate1_array = GateFinal_array, so using already created Gate1_array
                y = Gate1_array[j][1];
                //                chartData.Series[KernelPlot].Points.AddXY(x, y);
            }
*/


            // Create a new Mean-Shift algorithm for 3 dimensional samples
            // MeanShift meanShift = new MeanShift()
            // {
            //     // Use a uniform kernel density
            //     Kernel = new UniformKernel(),
            //     Bandwidth = 2.0
            // };


            // MessageBox.Show(String.Format("Shift X = {0}", shiftX)); //
            // shiftXY = MeansClustering.MeansCluster.mShift(test_X, test_Y, test2D);

            //  MessageBox.Show(String.Format("Kernel density = {0}", shiftXY)); //shiftX
            //            MessageBox.Show(String.Format("Shifts X,Y = {0},{1}", shiftXY[0], shiftXY[1])); //shiftX
            //System.FormatException: 'Index (zero based) must be greater than or equal to zero and less than the size of the argument list.'
            #endregion

            List<Polygon> polygons = loadPolygon(Gate1_file);

            #region analyze "Cells" selection gate

            indexGate1_Max = FlowCytometry.FCMeasurement.findMaxValues(sample, Gate1_array, FSC1_H, SSC_H);
            int count_Gate1out_Max = CountBoolTrue(indexGate1_Max);
            
            for (int j = 0; j < TotalDataLength; j++)   // for (int j = 0; j < Gate1Max_Length; j++)
            {
                x = Gate1_array[j][0];    //x = Gate1_arrayMax[j][0];
                y = Gate1_array[j][1];    //y = Gate1_arrayMax[j][1];
                                          // indexGate1[j] = false;
                if (polygons != null)
                {
                    if (polygons[0].IsInsidePoly(x, y))
                    {
                        indexGate1[j] = true;
                        chartData.Series[threeDiffGated].Points.AddXY(x, y);//PLOT results of CELLS GATE 
                                                                         //chartData.Series[cluster_id].Points.AddXY(point.X, point.Y);
                    }
                    else
                    {
                        indexGate1[j] = false;
                        chartData.Series[outsideFixedGates].Points.AddXY(x, y);
                    }
                }
            }
            //  indexGateFinal = FlowCytometry.FCMeasurement.GateArray(Gate1_array, Gate1_file);
          
            //MessageBox.Show("Supposed to draw chartData here. Press a button...");
            //Console.ReadKey();

            int count_Gate1out = CountBoolTrue(indexGate1);
            if (checkBoxGate1.Checked)
            {
                chartData.BringToFront();
                chartData.Invalidate();
                // MessageBox.Show(String.Format("Total data length = {0} \n Inside Cells gate = {1}", TotalDataLength, count_Gate1out));
            }

            #endregion

            // GATE 2: Singlets

            #region get Singlets gate file location
            string filePath_gate2 = filePath_gates; // "C:/Users/begem/OneDrive/Desktop/General Fluidics/Csharp/Build9/WindowsFormsApplication1/bin/Debug";
            string fileName_gatedSinglets = "gating Singlets.csv";
            string Gate2_file = Path.Combine(filePath_gate2, Path.GetFileName(fileName_gatedSinglets));

            string FSC1_A = FlowCytometry.FCMeasurement.GetChannelName("FCS1area", channelNomenclature);

            Gate2_hs = GenerateDataSet(FSC1_A, FSC1_H);
            double[][] Gate2_array = Gate2_hs.ToArray();
            #endregion

            polygons = loadPolygon(Gate2_file);

            double[] SingletsFit = new double[3];
            SingletsFit = FlowCytometry.FCMeasurement.LinearRegression(Gate2_array);
            MessageBox.Show(String.Format("Slope = {0}, Y-intercept = {1}, R^2 = {2}", 
                Math.Round(SingletsFit[0],1), Math.Round(SingletsFit[1], 1), Math.Round(SingletsFit[2],1)));

            double[] X1Y1X2Y2 = new double[4];
            X1Y1X2Y2[0] = 0;
            X1Y1X2Y2[1] = 0;
            X1Y1X2Y2[2] = SingletsFit[0];
            X1Y1X2Y2[3] = SingletsFit[1];

            // DrawLineInt(PaintEventArgs e, X1Y1X2Y2); //System.Windows.Forms.
            // DrawLineInt(ChartPaintEventArgs e, X1Y1X2Y2);
            // ChartPaintEventArgs e
            // Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
            // e.Graphics.DrawLine(pen, X1Y1X2Y2[0],X1Y1X2Y2[1],X1Y1X2Y2[2],X1Y1X2Y2[3]);

            indexGate2_Max = FlowCytometry.FCMeasurement.findMaxValues(sample, Gate1_array, FSC1_A, FSC1_H);
            int count_Gate2Max = CountBoolTrue(indexGate2_Max);

            #region analyze Gate2 "Singlets"

            for (int j = 0; j < TotalDataLength; j++)
            {
                x = Gate2_array[j][0];    //x = Gate1_arrayMax[j][0];
                y = Gate2_array[j][1];    //y = Gate1_arrayMax[j][1];

                if (polygons != null)
                {
                    // indexGate2[j] = false; //setting default value (outside "singlets")
                    if (polygons[0].IsInsidePoly(x, y))
                    {
                        indexGate2[j] = true;
                        chartData.Series[threeDiffGated].Points.AddXY(x, y);
                    }
                    else
                    {
                        indexGate2[j] = false;
                        chartData.Series[outsideFixedGates].Points.AddXY(x, y);
                    }
                }
                else
                {
                    throw new InvalidDataException("Singlets Gate not found");
                }
            }

            int count_Gate2 = CountBoolTrue(indexGate2);
            if (checkBoxGate2.Checked)
            {
                foreach (var series in chartData.Series)
                {
                    series.Points.Clear();
                }

                chartData.BringToFront();
                MessageBox.Show(String.Format("Total data length = {0} \n Inside Singlets gate = {1}", TotalDataLength, count_Gate2));
            }

            #endregion

            // GATE 4: Neutrophils,Lymphocytes, Monocytes
            // Channels for Gate 3 are the same as Gate 1 (FCS1 vs SSC)

            #region read Final Gate 
            //  string filePath_gates = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Csharp/Build9/WindowsFormsApplication1/bin/Debug";
            string fileName_GateFinal = "gating Cell Types.csv"; // gating2.csv"; //4
            string GateFinal_file = Path.Combine(filePath_gates, Path.GetFileName(fileName_GateFinal));
            #endregion

            polygons = loadPolygon(GateFinal_file);

            #region analyze GateFinal for positive selection by Cell size or Granularity (FSC vs SSC)

            int FSC1_H_max = sample.Channels[FSC1_H].Range;
            int SSC_H_max = sample.Channels[SSC_H].Range;

            bool[] NeutrophilsTF = new bool[TotalDataLength];
            bool testCombinedGate = false;
            if (testCombinedGate)
            {
                indexGateFinal = FlowCytometry.FCMeasurement.GateArray(Gate1_array, Gate1_file, FSC1_H_max, SSC_H_max);
                // only works for one gate

            }
            else
            {
                HashSet<double[]>[] cytes = new HashSet<double[]>[polygons.Count];
                //
                for (int j = 0; j < polygons.Count; j++)
                {
                    cytes[j] = new HashSet<double[]>();
                }

                for (int j = 0; j < TotalDataLength; j++)//Gate2Max_Length 
                {
                    x = Gate1_array[j][0]; //Gate1_array = GateFinal_array, so using already created Gate1_array
                    y = Gate1_array[j][1];
                    indexGateFinal[j, 1] = false;
                    if (polygons != null)
                    {
                        for (int k = 0; k < polygons.Count; k++)
                        {
                            if (polygons[k].IsInsidePoly(x, y))                   //  (Polygon.IsInsidePoly(x, y))
                            {
                                cytes[k].Add(new double[2] { x, y }); //[x][y]
                                                                      // chartData.Series[threeDiffGated].Points.AddXY(x, y);
                                indexGateFinal[j, 1] = true;

                                if (k == 0)
                                {
                                    NeutrophilsTF[j] = true;
                                }
                            }
                            else
                            {
                                //  chartData.Series[outsideFixedGates].Points.AddXY(x, y);
                                //indexGateFinal[j] = false;
                            }
                        }
                    }
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
            bool[] indexGatesG1G2Gfin = new bool[TotalDataLength];
            for (int j = 0; j < TotalDataLength; j++)
            {
                indexUnGated[j] = true;
                indexDataShown[j] = indexGate1_Max[j] && indexGate2_Max[j] && indexGate3_Max[j];
                indexGatesTotal[j] = indexDataShown[j] && indexGate1[j] && indexGate2[j] && indexGate3[j] && indexGateFinal[j, 1];
                indexGatesG1G2[j] = indexGate1_Max[j] && indexGate1[j] && indexGate2_Max[j] && indexGate2[j];
                indexGatesG1G2G3[j] = indexDataShown[j] && indexGate1[j] && indexGate2[j] && indexGate3[j];
                indexGatesG1G2Gfin[j] = indexDataShown[j] && indexGate1[j] && indexGate2[j] && indexGateFinal[j, 1];
                indexDataShownNotGated[j] = indexDataShown[j] && !indexGatesTotal[j];
            }
            int totalGatedCells = FlowCytometry.FCMeasurement.CountBoolTrue(indexGatesTotal);
            //int GateFinalCells = FlowCytometry.FCMeasurement.CountBoolTrue2D(indexGateFinal,1);

            bool[] Final_Gate = FlowCytometry.FCMeasurement.GetColumn(indexGateFinal, 1);

            //  Console.WriteLine(stopwatch.ElapsedMilliseconds);


            bool output_Gating_Stats = false;
            if (output_Gating_Stats)
            {
                int GateFinalCells = FlowCytometry.FCMeasurement.CountBoolTrue(Final_Gate);
                stopwatch.Stop();
                // MessageBox.Show(String.Format("Time for fixed gating = {0}ms", stopwatch.ElapsedMilliseconds));
                string Count_Ini = String.Format("Length of initial / raw data  = {0}", TotalDataLength);
                string Count_G1Max = String.Format("Gate1 removed Max values count = {0}", count_Gate1out_Max);
                string Count_G1 = String.Format("Gate1: FCS vs SSC (peak) Count = {0}", count_Gate1out);
                string Count_G2Max = String.Format("Gate2 removed Max values count = {0}", count_Gate2Max);
                string Count_G2 = String.Format("Gate2 Count FSC1 Peaks vs Area = {0}", count_Gate2);
                string Count_GFinal = String.Format("Gate3 (FSC1 vs SSC) gated cells = {0}", GateFinalCells);
                string Count_Total = String.Format("Total gated cells (G1*G2*G3) = {0}", totalGatedCells); //G3*
                double calculatedExcludedPercent = 100 * (TotalDataLength - totalGatedCells) / TotalDataLength; //, 1 Math.Round((
                                                                                                                //  calculatedExcludedPercent = Math.Round(calculatedExcludedPercent,1);
                string excludedPercent = String.Format("Percent events gated out: {0}%\n" +
                    "Time for fixed gating: {1}ms", calculatedExcludedPercent, stopwatch.ElapsedMilliseconds);

                string[] joined_report = new string[8] {Count_Ini, Count_G1Max, Count_G1, Count_G2Max, Count_G2,
                    Count_GFinal, Count_Total, excludedPercent}; //Count_G3Max, Count_G3,
                MessageBox.Show(String.Join(" \n", joined_report));
            }

            #endregion

            #region plot Final Gate for 3-diff
            bool plotFixedGates = true;//  false; ///
            bool plotKmeansGated = false;
            //   bool plotClustering = !plotFixedGates;
            int countInside = 0;
            int countOutside = 0;

            if (plotFixedGates)
            {
                stopwatch.Start();
                for (int j = 0; j < TotalDataLength; j++)//Gate2Max_Length 
                {
                    if (indexDataShown[j])
                    {
                        x = Gate1_array[j][0]; //Gate1_array = GateFinal_array, so using already created Gate1_array
                        y = Gate1_array[j][1];

                        if (indexGatesG1G2[j])
                        {
                            chartData.Series[threeDiffGated].Points.AddXY(x, y);
                            countInside++;
                        }
                        else
                        {
                            chartData.Series[outsideFixedGates].Points.AddXY(x, y);
                            countOutside++;
                        }
                    }
                }
                stopwatch.Stop();

                MessageBox.Show(String.Format("Time for plotting = {0}", stopwatch.ElapsedMilliseconds));
                MessageBox.Show(String.Format("Overall Gating: In = {0}, , out = {1}", countInside, countOutside));
            }
            else if (plotKmeansGated)
            {
                int number = 5; //4
                if (centers != null)
                {
                    number = centers.Length;
                    MessageBox.Show("Loaded centers");
                }
                else
                {
                    MessageBox.Show("Centers not loaded");
                }

               // HashSet<double[]> firstGate = GenerateDataSet(FSC1_H, SSC_H);             
                double[][] arrayG1 = Gate1_hs.ToArray();     
                bool hasClass = false;

                // HashSet<double[]> dataSet_hs = GenerateDataSet(channel1, channel2);

                bool[] indexGatesUsed;
                //= G1G2G3
                if (checkBox_PrefilterData.Checked)
                {
                    indexGatesUsed = indexGatesG1G2;
                }
                else
                {
                    indexGatesUsed = indexUnGated;
                }

                for (int j = 0; j < TotalDataLength; j++)
                {
                    x = arrayG1[j][0];
                    y = arrayG1[j][1];
                    if (indexGatesG1G2[j]) //G3
                    {
                        gatedData_hs.Add(new double[2] { x, y });
                    }
                }

                //  MessageBox.Show("Starting clustering inside loop");
                SharpCluster.Partitional.KMeans kmeans = new SharpCluster.Partitional.KMeans(gatedData_hs, hasClass);
                SharpCluster.Cluster[] clusters = kmeans.ExecuteClustering(number, centers);
                
                //   MessageBox.Show("Done with clustering inside loop");
                //Converte clusters (hierarquico) para clusters (particional) como o k-means, para utilizar Dunn e Davies-Bouldin

            }
            else
            {
                double[][] arrayInit;  // FlowCytometry.FCMeasurement sample = new FlowCytometry.FCMeasurement(@DataFile.FileName);
                double[][] arrayG1;

                //  FCS1_H = FlowCytometry.FCMeasurement.GetChannelName("FCS1peak", channelNomenclature);

                // MessageBox.Show(String.Format("FSC1_H = {0}\n SSC_H = {1}", FSC1_H, SSC_H));
                HashSet<double[]> filteredData_hs = GenerateDataSet(FSC1_H, SSC_H); //firstGate
                List<double[]> filteredData = filteredData_hs.ToList();

                // List<double[]> filteredData = filteredData_hs.ToList();
                HashSet<double[]> filteredData2_hs = new HashSet<double[]>();
                arrayInit = filteredData.ToArray();//arrayG1;// 
                                                   // HashSet<double[]> filteredData;

                for (int i = 0; i < arrayInit.Length; i++)
                {
                    if (indexGatesG1G2[i])
                    {
                        double xVal = arrayInit[i][0];//.GetAttribute(0);
                        double yVal = arrayInit[i][1];// filteredData2_hs[i].GetAttribute(1);
                        filteredData2_hs.Add(new double[2] { xVal, yVal });
                    }
                }

                arrayG1 = filteredData2_hs.ToArray();

                Array_XY = MeansClustering.MeansCluster.To2D(arrayG1);
                string[] XY_strArr = new string[arrayG1.Length];

                for (int j = 0; j < arrayG1.Length; j++)
                {
                    XY_strArr[j] = string.Join(",", Array_XY[j, 0], Array_XY[j, 1]);
                }

                //  File.WriteAllText("2D_N50xN50.csv", sb.ToString());

                MeansShift ms = new MeansShift(XY_strArr); //sb.ToString()
                MeansShift.ClearSeries(chartData);
                MeansShift.visualizeRealData(chartData);

                //-------------------------------
                MyCustomDatasetItem[] featureData = { };

                // Setting DataPoints
                List<MyCustomDatasetItem> testPoints = new List<MyCustomDatasetItem>();
                for (int i = 0; i < MeansShift.DataSet.Count; i++)
                {
                    testPoints.Add(new MyCustomDatasetItem(
                        MeansShift.DataSet[i].X,
                        MeansShift.DataSet[i].Y
                        ));
                }
                featureData = testPoints.ToArray();
                HashSet<MyCustomDatasetItem[]> clusters;
                // Setting the distance method (euclidean)
                var dbs = new DbscanAlgorithm<MyCustomDatasetItem>((x1, y1) => Math.Sqrt(((x1.X - y1.X) * (x1.X - y1.X)) + ((x1.Y - y1.Y) * (x1.Y - y1.Y))));
                // Computing the clusters (params must be tested)

                //------------------------- CHANGE PARAMS HERE -------------------------

                dbs.ComputeClusterDbscan(allPoints: featureData,
                    epsilon: eps, minPts: minNumPoints, clusters: out clusters); // epsilon: 175, minPts: 25, clusters: out clusters); FOR WBC03

                //-----------------------------------------------------------------------
                // Plotting

                MessageBox.Show(String.Format("Number of clusters = {0}", clusters.Count)); //Console.WriteLine
                MeansShift.ClearSeries(chartData);
                int cluster_id = 0;
                //int point_id;
                double cluster_centerX;
                double cluster_centerY;
                foreach (MyCustomDatasetItem[] Cluster in clusters)
                {
                    {
                        // Cluster.Centroid
                        string clusterName = String.Join("Cluster", cluster_id);// "Cluster" + cluster.Id;
                                                                                // MessageBox.Show(String.Format("Cluster id = {0}\n Cluster number = {0}", cluster_id, clusterName));
                                                                                // cluster_centerX = Cluster.Centroid.GetAttribute(0);
                                                                                //  cluster_centerY = Cluster.Centroid.GetAttribute(1);

                        // MessageBox.Show(String.Format("Cluster number = {0}", clusterName));
                        chartData.Series.Add(clusterName);
                        chartData.Series[clusterName].ChartType = SeriesChartType.Point;
                        chartData.Series[clusterName].MarkerSize = 1;
                        chartData.Series[clusterName].MarkerStyle = MarkerStyle.Circle;
                        chartData.Series[clusterName].MarkerColor = colors[cluster_id];

                        foreach (MyCustomDatasetItem point in clusters.ElementAt(cluster_id))

                            chartData.Series[cluster_id].Points.AddXY(point.X, point.Y);

                    }

                    cluster_id++;
                }
                chartData.BringToFront();
                MessageBox.Show("Press a button...");
                int N_lymphocytes = 0;
                int N_Neutrophils = 0;
                int N_Monocytes = 0;

                // Location of centroids found by DBscan
                int cluster_DBscan = clusters.Count;
                double[][] dbsCentroids = null;

                HashSet<double[]> dbsCentroid_hs = new HashSet<double[]>();

                bool applyKmeansClustering = true;
                int clusterPointCount;
                if (cluster_DBscan == 3)
                {
                    int[] NLMcounts = new int[3]; // filter by location 
                                                  //                    N_lymphocytes++;


                    for (int i = 0; i < 3; i++)
                    {
                        double Xloc = clusters.ElementAt(i).Average(point => point.X);
                        double Yloc = clusters.ElementAt(i).Average(point => point.Y);
                        dbsCentroid_hs.Add(new double[2] { Xloc, Yloc });
                        clusterPointCount = clusters.ElementAt(i).Length;
                        NLMcounts[i] = clusterPointCount;
                        //centers //  Gate1_hsMax.Add(new double[2] { x, y });
                    }

                    dbsCentroids = dbsCentroid_hs.ToArray();


                    double Xloc1 = clusters.ElementAt(0).Average(point => point.X);
                    double Yloc1 = clusters.ElementAt(0).Average(point => point.Y);

                    double Xloc2 = clusters.ElementAt(1).Average(point => point.X);
                    double Yloc2 = clusters.ElementAt(1).Average(point => point.Y);

                    double Xloc3 = clusters.ElementAt(2).Average(point => point.X);
                    double Yloc3 = clusters.ElementAt(2).Average(point => point.Y);
                    MessageBox.Show(String.Format("Cluster1: Loc = ({0},{1}), N={2}" +
                        "\n Cluster2: Loc = ({3},{4}), N={5}" +
                        "\n Cluster3: Loc  = ({6},{7}), N={8}" +
                        "\n epsilon = {9}, min = {10}", +
                        Math.Round(Xloc1), Math.Round(Yloc1), NLMcounts[0], Math.Round(Xloc2), Math.Round(Yloc2), NLMcounts[1], Math.Round(Xloc3), Math.Round(Yloc3), NLMcounts[2], eps, minNumPoints));
                    // MessageBox.Show(String.Format("Number of clusters = {0}", clusters.Count)); //Console.WriteLine
                    if (applyKmeansClustering)
                    {
                        double xVal;
                        double yVal;
                        // MessageBox.Show("Applying Kmeans clustgering to 3-diff data");
                        //polygons

                        string centroidGating_3Diff = "gating Cell Types Centroids.csv";
                        string Gating_3Diff = Path.Combine(filePath_gates, Path.GetFileName(centroidGating_3Diff));
                        polygons = loadPolygon(Gating_3Diff);

                        //   centers = LoadClusterCenters(BasoClusterCenters);
                        int nCentroids = 12;
                        bool hasClass = false;
                        centers = FlowCytometry.FCMeasurement.LoadClusterCenters(diff3_ClusterCenters); // double[][] 
                        double[][] JoinedCentroidResult = dbsCentroids.Union(centers).ToArray();
                        SharpCluster.Partitional.KMeans kmeans = new SharpCluster.Partitional.KMeans(Gate1_hs, hasClass);
                        // SharpCluster.Cluster[] kMeansClusters = kmeans.ExecuteClustering(nCentroids, centers);
                        SharpCluster.Cluster[] kMeansClusters = kmeans.ExecuteClustering(nCentroids, JoinedCentroidResult);

                        int count_inside = 0;

                        foreach (SharpCluster.Cluster Cluster in kMeansClusters)
                        {
                            cluster_centerX = Cluster.Centroid.GetAttribute(0);
                            cluster_centerY = Cluster.Centroid.GetAttribute(1);

                            // classify clasters based on centroid being within fixed gates
                            if (polygons[0].IsInsidePoly(cluster_centerX, cluster_centerY))
                            {
                                count_inside += Cluster.GetPatterns().Length;
                                foreach (SharpCluster.Pattern pattern in Cluster.GetAllPatterns())
                                {
                                    xVal = pattern.GetAttribute(0);
                                    yVal = pattern.GetAttribute(1);
                                    chartData.Series[threeDiffGated].Points.AddXY(xVal, yVal);
                                }
                            }
                            else
                            {
                                foreach (SharpCluster.Pattern pattern in Cluster.GetAllPatterns())
                                {
                                    xVal = pattern.GetAttribute(0);
                                    yVal = pattern.GetAttribute(1);
                                    chartData.Series[outsideFixedGates].Points.AddXY(xVal, yVal);
                                }
                                // ASSIGN CLUSTER IDENTITY 2
                                // DO NOT COUNT CELLS
                            }
                        }
                    }
                    chartData.BringToFront();
                    MessageBox.Show("Press a button...");
                }
                else if (cluster_DBscan == 2)
                {
                    // get the points within the second cluster
                    //  MeansShift.DataSet = clusters.ElementAt(0).Select((point) => new ClusterAPI.DataPoint(point.X, point.Y)).ToList();
                }

                // get the points within the second cluster
                MeansShift.DataSet = clusters.ElementAt(0).Select((point) => new ClusterAPI.DataPoint(point.X, point.Y)).ToList();


                chartData.BringToFront();
                MessageBox.Show("Press a key ...");


                // clusters.Centroids
                // System.ArgumentOutOfRangeException: 'Specified argument was out of the range of valid values.
                // Draw first cluster (blue)
                foreach (MyCustomDatasetItem point in clusters.ElementAt(1))
                //System.ArgumentOutOfRangeException: 'Specified argument was out of the range of valid values.
                {
                    chartData.Series[0].Points.AddXY(point.X, point.Y);
                    N_lymphocytes++;
                }

                //   count_inside += Cluster.GetPatterns().Length;

                //------------------------- CHANGE PARAMS HERE -------------------------
                // apply means shift on the dataset
                HashSet<ClusterAPI.DataPoint> subClusters = MeansShift.fit_shift(bandwidth: 1200); //1610

                int length_subclusters = subClusters.Count;
                MessageBox.Show(String.Format("Number of subclusters found by MeansShift = {0}", length_subclusters));
                // creating a new landmark which will be the average of the two centroids found by means shift
                ClusterAPI.DataPoint landmark = new ClusterAPI.DataPoint(
                   subClusters.Average(point => point.X),
                   subClusters.Average(point => point.Y)
                   );

                // rotate one of the centroids found by means shift by the new landmark (it will be considered as (0,0) ) 
                // the angle is set to -37 and can be adjusted
                ClusterAPI.DataPoint rotated = MeansShift.rotate_point(landmark.X, landmark.Y, -37, subClusters.ElementAt(1));

                // get the hyperplane equation (the separator)
                Equation eq = MeansShift.getHyperPlaneEquation(landmark, rotated);

                // draw the data 
                foreach (var item in MeansShift.DataSet)
                {
                    if (item.Y > (eq.a * item.X + eq.b))
                    {
                        chartData.Series[1].Points.AddXY(
                                                     item.X,
                                                     item.Y);
                        N_Neutrophils++;
                    }
                    else
                    {
                        chartData.Series[2].Points.AddXY(
                                                 item.X,
                                                 item.Y);
                        N_Monocytes++;
                    }
                }
                MessageBox.Show(String.Format("N_Monocytes = {0},N_lymphocytes = {1}, N_Neutrophils = {2}", N_Monocytes, N_lymphocytes, N_Neutrophils));

            }
            #endregion

            //string fileTot = Path.Combine(filePath, Path.GetFileName(fileName));

            #region set combobox values (unused)
            //comboBox2.Text(set)
            // comboBox1.Text = comboBox1.Items.IndexOf("FSC1LG,Area"); // DisplayMember

            //1) By using "SelectedIndex"
            //            mycomboBox.SelectedIndex = mycomboBox.Items.IndexOf("value");
            //            OR
            //Combox1.SelectedIndex = Combox1.FindStringExact("value");
            //            2) By using "SelectedValue"
            //ComboBox1.SelectedItem = "value";
            #endregion
           
            string[] argumentsIn = new string[4];
            int[] NML = new int[3];
            argumentsIn[0] = Loaded_TotalFileName; // full FCS filename with path an extension
            argumentsIn[1] = filePath_gates; // file path to folder with fixed gates
            argumentsIn[2] = channelNomenclature; //  type of nomeclature: "old_names", "new_names", "middleaged_names"
            argumentsIn[3] = "3-diff";//sampleType ; // type of analysis: "EOS", "BASO",

            bool ouputExcel = false;
            NML = FlowCytometry.FCMeasurement.WBC_analysis(argumentsIn, ouputExcel);
            MessageBox.Show(String.Format("NML Report: {0},{1},{2}\n", NML[0], NML[1], NML[2]));
            //CalculateWBC(filePath, fileName); // ERROR HERE for EOS
            //            sample.WriteExcelFile(outputFileName);
            //    MessageBox.Show("Finished fixed gating of 3-diff");

            #region charts finish

            chartData.ChartAreas[0].AxisX.Title = FSC1_H;
            chartData.ChartAreas[0].AxisY.Title = SSC_H;

            chartData.ChartAreas[0].AxisX.RoundAxisValues();
            chartData.ChartAreas[0].AxisY.RoundAxisValues();

            chartData.ChartAreas[0].AxisX.IsStartedFromZero = true;
            chartData.ChartAreas[0].AxisY.IsStartedFromZero = true;

            chartData.BringToFront();
            chartData.Invalidate();
            #endregion

            var tuple = new Tuple<int[], bool[]>(NML, NeutrophilsTF);
            return tuple; 

        }
        


        private void BtnPlotOnly_Click(object sender, EventArgs e)
        {
            string channel1 = comboBox1.Text;
            string channel2 = comboBox2.Text;
            string fileTot;
            if (Loaded_TotalFileName != null)
            {
                fileTot = Loaded_TotalFileName;
                MessageBox.Show(String.Format("Loaded file {0}", Loaded_TotalFileName));
            }
            else
            {
                /*                string default_filePath = "C:/Users/begem/OneDrive/Desktop/General Fluidics/Data/MGH/WBC/140725/";
                                string default_fileName = "H43069_CD16APC_25mW_02.fcs"; //H43069_CD16APC_20mW_02.fcs";
                */
                string default_filePath = "F:\\VisualStudio\\C#\\Clustering\\TestData\\FCS";
                string default_fileName = "WBC_sample.fcs";

                fileTot = Path.Combine(default_filePath, Path.GetFileName(default_fileName));
            }

            //int[] index_Cells;
            //int[] index_Singles;
            //int[] index_SizeGated;
            //int number = 5;

            sample = new FlowCytometry.FCMeasurement(fileTot);

            if (!channel1.Equals(channel2))
            {
                //bool hasClass = false;
                //       HashSet<double[]> Gate1_hs = new HashSet<double[]>();
                HashSet<double[]> dataSet_hs = GenerateDataSet(channel1, channel2);
                HashSet<double[]> gatedData_hs = new HashSet<double[]>();
                //  SharpCluster.Partitional.KMeans kmeans = new SharpCluster.Partitional.KMeans(_dataSet, hasClass);
                // SharpCluster.Cluster[] clusters = kmeans.ExecuteClustering(number, centers);

                #region initialize plots
//                chartData.Annotations.Clear();
//                chartData.ChartAreas.Clear();
//                chartData.Legends.Clear();
                chartData.Series.Clear();

//                ChartArea chartArea1 = new ChartArea();
//                chartData.ChartAreas.Add(chartArea1);
//                chartData.Series.Clear();

                //int index = 0;
                //foreach (Polygon.polygon in polygons) // .Cluster cluster in clusters)

                string insideGate = "Inside Gate";// + cluster.Id;
                chartData.Series.Add(insideGate);
                chartData.Series[insideGate].ChartType = SeriesChartType.Point;
                chartData.Series[insideGate].MarkerSize = 1; //
                chartData.Series[insideGate].MarkerStyle = MarkerStyle.Diamond;
                chartData.Series[insideGate].MarkerColor = Color.Red; // colors[index];

                string outsideGate = "Outside Gate";// + cluster.Id;
                chartData.Series.Add(outsideGate);
                chartData.Series[outsideGate].ChartType = SeriesChartType.Point;
                chartData.Series[outsideGate].MarkerSize = 1;
                chartData.Series[outsideGate].MarkerStyle = MarkerStyle.Circle;
                chartData.Series[outsideGate].MarkerColor = Color.Blue; //colors[index];
                //wait
                string PlotwithoutGating = "Plot without Gating";// + cluster.Id;
                chartData.Series.Add(PlotwithoutGating);
                chartData.Series[PlotwithoutGating].ChartType = SeriesChartType.Point;
                chartData.Series[PlotwithoutGating].MarkerSize = 3;
                chartData.Series[PlotwithoutGating].MarkerStyle = MarkerStyle.Circle;
                chartData.Series[PlotwithoutGating].MarkerColor = Color.Blue; //colors[index];
                chartData.BringToFront();
                //foreach (SharpCluster.Pattern pattern in cluster.GetAllPatterns())
                #endregion
                //    MessageBox.Show("Initialized plotting");
                double[][] dataSet_array = dataSet_hs.ToArray();
                int dataLength = dataSet_array.GetLength(0);
                bool[] indexGated = new bool[dataLength];
                double x;
                double y;

                if (polygons != null)
                {
                    for (int j = 0; j < dataLength; j++)
                    {
                        x = dataSet_array[j][0]; // Gate2_array
                        y = dataSet_array[j][1]; //Gate2_array

                        // bool added = false;
                        for (int k = 0; k < polygons.Length; k++) //polygons.Count
                        {
                            if (polygons[k].IsInsidePoly(x, y)) //  (Polygon.IsInsidePoly(x, y))
                            {
                                gatedData_hs.Add(new double[2] { x, y });
                                indexGated[j] = true;
                                chartData.Series[insideGate].Points.AddXY(x, y);
                            }
                            else
                            {
                                indexGated[j] = false;
                                chartData.Series[outsideGate].Points.AddXY(x, y);
                            }
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Polygons not loaded, Plotting without gating");

                    for (int j = 0; j < dataLength; j++)
                    {
                        x = dataSet_array[j][0]; // Gate2_array
                        y = dataSet_array[j][1]; //Gate2_array

                        chartData.Series[PlotwithoutGating].Points.AddXY(x, y);
                    }
                }
                MessageBox.Show("Finished plotting individual data points");
                #region finish plots
                chartData.ChartAreas[0].AxisX.Title = channel1;
                chartData.ChartAreas[0].AxisY.Title = channel2;

                chartData.ChartAreas[0].AxisX.RoundAxisValues();
                chartData.ChartAreas[0].AxisY.RoundAxisValues();

                chartData.ChartAreas[0].AxisX.IsStartedFromZero = true;
                chartData.ChartAreas[0].AxisY.IsStartedFromZero = true;
                chartData.Invalidate();
                #endregion

                MessageBox.Show("Finished plotting");
                bool outputExcel = false;
                if (outputExcel)
                {
                    sample.WriteMetadata("meta.csv");
                    string outputFileName = "Data Excel format.xlsx";
                    sample.WriteExcelFile(outputFileName);
                    //CalculateWBC();
                    //WriteWBC_Counts("WBC_counts_test.csv");
                }
            }
            else
            {
                MessageBox.Show("Channels are the same");
            }
        }

        private void Button1_Click_1(object sender, EventArgs e) //rename LoadFile
        {
            // MessageBox.Show(String.Format("Initial directry {0}", starting_filePath));
            //  thrtb1 = new Thread(updateTextBox1);
            //  thrtb1.Start();
            // thrtb2 = new Thread(tb2ControlsUpdate);
            // .Start();

            //OpenFileDialog dlg = new OpenFileDialog();
            starting_filePath = starting_filePath.TrimEnd(new char[] { '\\' });
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.InitialDirectory = starting_filePath; //GetDataPath
                dlg.Filter = "FCS files|*.fcs";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    //   Loaded_filePath = dlg.FilePath;
                    Loaded_TotalFileName = dlg.FileName;
                    //   MessageBox.Show(String.Format("Loaded file {0}", Loaded_TotalFileName));
                }
            }
            //FileNameBox.Text = Convert.ToString(myThread.myValue);
            FileNameBox.Text = Loaded_TotalFileName;


            sample = new FlowCytometry.FCMeasurement(Loaded_TotalFileName); //fileWBC

            foreach (String name in sample.ChannelsNames)
            {
                comboBox1.Items.Add(name);
                comboBox2.Items.Add(name);
            }
            // }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            //CheckBox1_CheckedChanged.Checked = true;
            //CheckState property represents the state of a CheckBox. It can be checked or unchecked. Usually, we check if a CheckBox is checked or not and decide to take an action on that state something like following code snippet.

            if (checkBox_PrefilterData.Checked)
            {
                MessageBox.Show("Prefilter Checked");
            }
            else
            {
                MessageBox.Show("Prefilter NOT Checked");
            }
        }

        public Color HeatColorMap(double value, double min, double max)
        {
            //            double value, double min, double max

            double val = (value - min) / (max - min); // scale value to [0,1]
                                                      // double logMin = - Math.Log10(min);
                                                      // double logMax = - Math.Log10(max);
                                                      // double logVal = - Math.Log10(value);
                                                      // double val = (logVal - logMin) / (logMax - logMin);
            double logVal = Math.Log(value / min);
            //double logMin = 0; // = Math.Log(min/min);
            double logMax = Math.Log(max / min);
            double log_Sval = logVal / logMax;

            //            localDens = MeansClustering.MeansCluster.localDensity(Array_XY, sigma);
            //          denseMax = localDens.Max();
            // double val = (logVal - logMin) / (logMax - logMin);

            int r;
            int g;
            int b;
            int power_Stiffness = 3;
            double sVal;
            if (val >= 0.95) // (val >= 0.9995) //
            {
                //val = (val - 1 / 2) * 2; 
                power_Stiffness = 1 / 2;
                sVal = Math.Pow(val, power_Stiffness);
                r = Convert.ToByte(255 * sVal); // r = Convert.ToByte(255 * sVal); //
                g = Convert.ToByte(255 * (1 - sVal)); //  g = Convert.ToByte(255 * (1- sVal)); //
                b = 0;
            }
            else if (val > 0.5) //(log_Sval > 0.2 && log_Sval < 1000000) // && log_Sval < 0.995
            {
                //val = (val - 1 / 2) * 2; 
                power_Stiffness = 5;
                sVal = Math.Pow(val, power_Stiffness);
                r = Convert.ToByte(255 * val); // r = Convert.ToByte(255 * val);
                g = Convert.ToByte(255 * (1 - sVal)); //g = Convert.ToByte(255 * (1 - val)); //1 - sVal
                b = 0;
            }
            else
            {
                //  val = val * 2;
                r = 0;
                g = Convert.ToByte(255 * val); //val
                b = Convert.ToByte(255 * (1 - val)); //(1 - val)
            }
            return Color.FromArgb(255, r, g, b);
        }


        public Color RedColorMap(double value, double min, double max)
        {
            double val = (value - min) / (max - min);
            // Integral has been normalized to 1, so average density per point is value/Ndata

            if (val < Double.Epsilon)
            {
                val = 0.1;
            }
            int r;
            int g;
            int b;
            r = Convert.ToByte(255 * Math.Sqrt(val)); // r = Convert.ToByte(255 * sVal); // 
            g = 0; //  g = Convert.ToByte(255 * (1- sVal)); //
            b = 0;
            return Color.FromArgb(255, r, g, b);
        }

        public void DrawLineInt(PaintEventArgs e, double[] X1Y1X2Y2) //PaintEventArgs 
        {
            Pen blackPen = new Pen(Color.Black, 3);   // Create pen.

            // Coordinates of points that define line.
            int x1 = Convert.ToInt32(Math.Round(X1Y1X2Y2[0], 0));
            int y1 = Convert.ToInt32(Math.Round(X1Y1X2Y2[1], 0));
            int x2 = Convert.ToInt32(Math.Round(X1Y1X2Y2[2], 0));
            int y2 = Convert.ToInt32(Math.Round(X1Y1X2Y2[3], 0));

            // Draw line to screen.
            e.Graphics.DrawLine(blackPen, x1, y1, x2, y2);
        }

        private void btnSeparation_Click(object sender, EventArgs e)
        {
            HashSet<double[]> firstGate = GenerateDataSet(FSC1_H, SSC_H);

            double[,] Array_XY;
            double[][] arrayG1;
            DataFile.FileName = "";

            bool loadCSVfile = false;
            bool loadFCSfile = !loadCSVfile;
            if (DataFile.ShowDialog() == DialogResult.OK)
            {// do whatever you like xd go pls the alternative you were showing is probably better
                // but  this is just code optimization ... exactly.. I have basic function issues :)

                if (loadCSVfile)
                {
                    // FIlter the file extension
                    DataFile.Filter = "dataPoints (*.csv)| *.csv";

                    MeansShift ms = new MeansShift(File.ReadAllLines(@DataFile.FileName));
                }
                else if (loadFCSfile)
                {
                    DataFile.Filter = "FCS file (*.fcs)| *.fcs";

                    // MeansShift ms = new MeansShift(File.ReadAllLines(@DataFile.FileName));
                    FlowCytometry.FCMeasurement sample = new FlowCytometry.FCMeasurement(@DataFile.FileName);

                    arrayG1 = firstGate.ToArray();
                    Array_XY = MeansClustering.MeansCluster.To2D(arrayG1);
                    string[] XY_strArr = new string[arrayG1.Length];
                    //   MeansShift ms = new MeansShift(File.ReadAllLines(@DataFile.FileName));

                    //StringBuilder sb = new StringBuilder();

                    for (int j = 0; j < arrayG1.Length; j++)
                    {
                        //      sb.AppendLine(string.Join(",", Array_XY[j,0], Array_XY[j,1]));
                        XY_strArr[j] = string.Join(",", Array_XY[j, 0], Array_XY[j, 1]);
                    }

                    //  File.WriteAllText("2D_N50xN50.csv", sb.ToString());

                    MeansShift ms = new MeansShift(XY_strArr); //sb.ToString()
                }

                MeansShift.ClearSeries(chartData);
                MeansShift.visualizeRealData(chartData);

                //-------------------------------
                MyCustomDatasetItem[] featureData = { };

                // Setting DataPoints
                List<MyCustomDatasetItem> testPoints = new List<MyCustomDatasetItem>();
                for (int i = 0; i < MeansShift.DataSet.Count; i++)
                {
                    testPoints.Add(new MyCustomDatasetItem(
                        MeansShift.DataSet[i].X,
                        MeansShift.DataSet[i].Y
                        ));
                }
                featureData = testPoints.ToArray();
                HashSet<MyCustomDatasetItem[]> clusters;
                // Setting the distance method (euclidean)
                var dbs = new DbscanAlgorithm<MyCustomDatasetItem>((x, y) => Math.Sqrt(((x.X - y.X) * (x.X - y.X)) + ((x.Y - y.Y) * (x.Y - y.Y))));
                // Computing the clusters (params must be tested)

                //------------------------- CHANGE PARAMS HERE -------------------------
                dbs.ComputeClusterDbscan(allPoints: featureData,
                    epsilon: 500, minPts: 300, clusters: out clusters);
                // try changing the epsilon value

                //-----------------------------------------------------------------------
                // Plotting
                Console.WriteLine("Number of clusters = " + clusters.Count);
                MeansShift.ClearSeries(chartData);
                // Draw first cluster (blue)
                foreach (MyCustomDatasetItem point in clusters.ElementAt(1))
                    chartData.Series[1].Points.AddXY(point.X, point.Y);

                // get the points within the second cluster
                MeansShift.DataSet = clusters.ElementAt(0).Select((point) => new ClusterAPI.DataPoint(point.X, point.Y)).ToList();

                //------------------------- CHANGE PARAMS HERE -------------------------
                // apply means shift on the dataset
                HashSet<ClusterAPI.DataPoint> subClusters = MeansShift.fit_shift(bandwidth: 1100); //1610

                // creating a new landmark which will be the average of the two centroids found by means shift
                ClusterAPI.DataPoint landmark = new ClusterAPI.DataPoint(
                   subClusters.Average(point => point.X),
                   subClusters.Average(point => point.Y)
                   );

                // rotate one of the centroids found by means shift by the new landmark (it will be concidered as (0,0) ) 
                // the angle is set to -37 and can be adjusted
                ClusterAPI.DataPoint rotated = MeansShift.rotate_point(landmark.X, landmark.Y, -37, subClusters.ElementAt(1));

                // get the hyperplane equation (the separator)
                Equation eq = MeansShift.getHyperPlaneEquation(landmark, rotated);

                // draw the data 
                foreach (var item in MeansShift.DataSet)
                {
                    if (item.Y > (eq.a * item.X + eq.b))
                    {
                        chartData.Series[1].Points.AddXY(
                                                     item.X,
                                                     item.Y);
                    }
                    else
                    {
                        chartData.Series[2].Points.AddXY(
                                                 item.X,
                                                 item.Y);
                    }
                }

                //-----------------------------------------
                chartData.BringToFront();
            }
        }

        private void radioButton_3diff_CheckedChanged(object sender, EventArgs e)
        {
            WBC_file_type = "3-diff";
        }

        private void radioButton_EOS_CheckedChanged(object sender, EventArgs e)
        {
            WBC_file_type = "EOS";
        }

        private void radioButton_BASO_CheckedChanged(object sender, EventArgs e)
        {
            WBC_file_type = "BASO";
        }

        private void FileNameBox_TextChanged(object sender, EventArgs e)
        {

        }
        private void checkBoxGate1_CheckedChanged(object sender, EventArgs e)
        {

        }


        private void checkBox_FixedGating_CheckedChanged(object sender, EventArgs e)
        {
            Gating_Type = "FixedGating";
        }

        private void checkBox_DynamicGating_CheckedChanged(object sender, EventArgs e)
        {
            Gating_Type = "DynamicGating";
            MessageBox.Show("Dynamic Gating currently not enabled");
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        public string Set_FixedGatingFolder()
        {

            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);

                    // System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                    string gates_filePath = "Fixed Gating folder location.txt";
                    File.WriteAllText(@gates_filePath, filePath_gates);
                    string cwd = Directory.GetCurrentDirectory(); ;
                    MessageBox.Show(String.Format("Wrote Fixed Gating folder location to {0}", cwd));

                    filePath_gates = " C:/Users/begem/OneDrive/Desktop/General Fluidics/Fixed gating";
                }
                else
                {
                    MessageBox.Show("Fixed Gating folder not set");
                }
            }            
            return filePath_gates;
        }

        private void button_SetGateFolder_Click(object sender, EventArgs e)
        {
            //string filePath_gates = " C:/Users/begem/OneDrive/Desktop/General Fluidics/Fixed gating";
            filePath_gates = Set_FixedGatingFolder();
        }
    }
}