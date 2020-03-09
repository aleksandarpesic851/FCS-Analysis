using FlowCytometry;
using FlowCytometry.CustomCluster;
using FlowCytometry.Mie_Scatter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Windows_FCS_Analysis
{
    public partial class RBCForm : Form
    {
        #region mean values calculated from all rbc.fcs files. 
        //this is used for calculate conversion factor Kx, Ky
        private const int MEAN_RBC_1 = 5710;
        private const int MEAN_RBC_2 = 4187;
        private const int MEAN_PLATELETE_1 = 1088;
        private const int MEAN_PLATELETE_2 = 1604;
        private const double MEAN_HC = 34;
        private const double MEAN_V = 85;
        private const double MEAN_S1 = 0.30327732397673;
        private const double MEAN_S2 = 0.323369234247477;
        #endregion

        private FCMeasurement fcMeasure = null;
        private string channel1 = "";                                   //channel name
        private string channel2 = "";                                   //channel name
        private List<double[]> orgRBCData = new List<double[]>();       // rbc data in fcs file
        private List<double[]> orgPlateleteData = new List<double[]>(); // platelete data in fcs file
        private List<Polygon> arrGatePolygon = null;                    // gate data in csv file
        private int nWBCCnt = 0;                                        // the number of WBC 

        string[] FCS1_H = new string[] { "FSC1LG,Peak", "FSC1HG,Peak", "BS1CH1; fsc1lg-H" };
        string[] FCS2_H = new string[] { "FSC2LG,Peak", "FSC2HG,Peak", "BS1CH2; fsc2lg-H" };
        string[] SSC_H = new string[] { "BS1CH2; ssclg-H", "SSCLG,Peak", "BS1CH4; ssclg-H" };

        private string GATE_FILE = "";

        private double Kx = MEAN_S1 / MEAN_RBC_1;  // s1 = fcs1 * Kx
        private double Ky = MEAN_S2 / MEAN_RBC_2;  // s2 = fcs2 * Ky

        private List<int> arrV = new List<int>();
        private List<int> arrHC = new List<int>();
        private List<Record> arrRecords = new List<Record>();
        private double record_min_s1 = 1000;
        private double record_max_s1 = 0;
        private double record_min_s2 = 1000;
        private double record_max_s2 = 0;

        private int minV = 30;
        private int maxV = 180;
        private int minHC = 20;
        private int maxHC = 46;

        double HGB = 0;    // Mie HC Average
        double HCT = 0;    // Hematocrit RBC volume fraction
        double MCV = 0;    // Average RBC volume
        double MCH = 0;    // Average absolute RBC hemoglobin per cell
        double MCHC = 0;   // Average concentration of hemoglobin per cell volume
        double PLT = 0;    // Number of platelets per microliter
        double RDW_SD = 0; // standard deviation of RBC volume distribution
        double RDW_CV = 0; // Coefficient of variation of RBC distribution (std / average)
        double PDW_SD = 0; // Standard deviation of platelets distribution
        double MPV = 0;    // Average volume of platelets
        double P_LCR = 0;  //Ratio of Platelets to total WBC count

        /// <summary>
        /// FOR WBC
        /// </summary>
        private FCMeasurement WBC_fcMeasure = null;
        private string WBC_channel1 = "";                                   //channel name
        private string WBC_channel2 = "";                                   //channel name
        private List<double[]> totalWBCData = new List<double[]>();
        private string gate1 = "gating Cells.csv";
        //private string gate2 = "gating Singlets.csv";
        private string gate3 = "gating Cell Types.csv";
        private List<Polygon> arrGatePolygon_WBC = null;
        private List<Polygon> arrGate3Polygon = null;
        private string WBCGatePath;
        private Custom_Meanshift meanshift;
        List<Cluster> clusters = null;

        public static MarkerStyle[] CELL_MARKER = new MarkerStyle[] { MarkerStyle.Diamond, MarkerStyle.Cross, MarkerStyle.Triangle };

        #region
        public RBCForm()
        {
            InitializeComponent();
            chartV.Series[0]["PixelPointWidth"] = "1";
            chartHC.Series[0]["PixelPointWidth"] = "1";
            txtKx.Text = "" + Kx;
            txtKy.Text = "" + Ky;
        }

        public void setGatePath(string path)
        {
            GATE_FILE = Path.Combine(path, "gating RBCs.csv");
            WBCGatePath = path;
        }

        private void btnRBC_Click(object sender, EventArgs e)
        {
            LoadFile();
            DrawFCSChart();
            drawGates();
        }

        private void btnLoadWBC_Click(object sender, EventArgs e)
        {
            LoadWBC();
            drawWBC();
        }
        private void btnCalculate_Click(object sender, EventArgs e)
        {
            arrV.Clear();
            arrHC.Clear();

            double s1, s2;
            int nRbcCnt;

            foreach (double[] point in orgRBCData)
            {
                s1 = point[0] * Kx;
                s2 = point[1] * Ky;

                Record record = arrRecords.Aggregate((e1, e2) =>
                    Math.Abs(e1.S1 - s1) + Math.Abs(e1.S2 - s2) <
                    Math.Abs(e2.S1 - s1) + Math.Abs(e2.S2 - s2) ?
                    e1 : e2);

                if (record.V <= minV || record.V >= maxV || record.HC <= minHC || record.HC >= maxHC)
                    continue;

                arrV.Add(record.V);
                arrHC.Add(record.HC);

                HGB += record.HC;
                MCV += record.V;
            }
            nRbcCnt = arrV.Count;
            HGB /= nRbcCnt;
            MCV /= nRbcCnt;
            RDW_SD = CalculateSD(arrV, MCV);
            RDW_CV = RDW_SD / MCV;

            DrawVChart();
            DrawHCChart();

            txtHGB.Text = "" + HGB;
            txtHCT.Text = "" + HCT;
            txtMCV.Text = "" + MCV;
            txtMCH.Text = "" + MCH;
            txtMCHC.Text = "" + MCHC;
            txtPLT.Text = "" + PLT;
            txtRDW_SD.Text = "" + RDW_SD;
            txtRDW_CV.Text = "" + RDW_CV;
            txtPDW_SD.Text = "" + PDW_SD;
            txtMPV.Text = "" + MPV;

            this.Refresh();

            CalculateWBCCounts();
            if (nWBCCnt == 0)
                P_LCR = 0;
            else
                P_LCR = orgPlateleteData.Count / nWBCCnt;

            txtP_LCR.Text = "" + P_LCR;
        }

        private double CalculateSD(List<int> data, double mean)
        {
            if (data.Count < 2)
                return 0;
            double result = 0;
            foreach (int v in data)
            {
                result += (v - mean) * (v - mean);
            }
            result = Math.Sqrt(result / (data.Count - 1));
            return result;
        }

        // this is used for calculate Kx, Ky.
        private void CalculateMeanValues()
        {
            double meanRBCX = 0;
            double meanRBCY = 0;
            double meanPlatleteX = 0;
            double meanPlatleteY = 0;
            int nCntRbc = orgRBCData.Count;
            int nCntPlatelete = orgPlateleteData.Count;

            foreach (double[] point in orgRBCData)
            {
                meanRBCX += point[0] / nCntRbc;
                meanRBCY += point[1] / nCntRbc;
            }
            Console.WriteLine("X: " + meanRBCX + ", Y: " + meanRBCY);
            foreach (double[] point in orgPlateleteData)
            {
                meanPlatleteX += point[0] / nCntPlatelete;
                meanPlatleteY += point[1] / nCntPlatelete;
            }
            Console.WriteLine("P-X: " + meanPlatleteX + ", P-Y: " + meanPlatleteY);
        }

        // Load FCS file from dialog.
        // Update combo box with loaded file data
        private void LoadFile()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "FCS files|*.fcs";
                if (dlg.ShowDialog() == DialogResult.OK)
                {

                    nWBCCnt = 0;
                    btnLoadWBC.Enabled = false;
                    btnCalculate.Enabled = false;
                    channel1 = "";
                    channel2 = "";
                    fcMeasure = new FCMeasurement(dlg.FileName);

                    foreach (String name in fcMeasure.ChannelsNames)
                    {
                        if (FCS1_H.Contains(name))
                            channel1 = name;
                        if (FCS2_H.Contains(name))
                            channel2 = name;
                    }
                }
                else
                {
                    return;
                }
            }

            if (String.IsNullOrEmpty(channel1) || String.IsNullOrEmpty(channel2))
            {
                MessageBox.Show("There is no proper channel. Please select right file.");
                return;
            }
            if (!ExtractDataFromFCS())
                return;

            btnLoadWBC.Enabled = true;
        }

        // extract channel data from FCS data
        private bool ExtractDataFromFCS()
        {
            if (fcMeasure == null || String.IsNullOrEmpty(channel1) || String.IsNullOrEmpty(channel2))
            {
                return false;
            }
            int i = 0;

            orgRBCData.Clear();

            if (String.IsNullOrEmpty(channel1) || String.IsNullOrEmpty(channel2))
                return false;

            if (channel1 == channel2)
            {
                MessageBox.Show("Two channels are equal. Please select different channel!");
                return false;
            }

            // Get gates according to channel
            if (!GetGates())
                return false;

            double x, y;
            for (i = 0; i < fcMeasure.Counts; i++)
            {
                x = fcMeasure.Channels[channel1].Data.ElementAt(i);
                y = fcMeasure.Channels[channel2].Data.ElementAt(i);

                if (arrGatePolygon[0].IsInsidePoly(x, y))
                {
                    orgPlateleteData.Add(new double[2] { x, y });
                }

                if (arrGatePolygon[1].IsInsidePoly(x, y))
                {
                    orgRBCData.Add(new double[2] { x, y });
                }
            }

            if (orgRBCData.Count < 2 || orgPlateleteData.Count < 2)
                return false;
            return true;
        }

        // get gate polygon according to channel
        private bool GetGates()
        {
            if (!File.Exists(GATE_FILE))
            {
                MessageBox.Show("There isn't gate file - gating RBCs.csv. Please add gate file in exe path.");
                return false;
            }
            arrGatePolygon = null;
            arrGatePolygon = FCMeasurement.loadPolygon(GATE_FILE);
            if (arrGatePolygon.Count != 2)
            {
                MessageBox.Show("The gate file is incorrect. there must be 2 gate polygon.");
                return false;
            }
            return true;
        }

        // Draw FCS data on chart
        private void DrawFCSChart()
        {
            chartFCS.Series.Clear();

            chartFCS.ChartAreas[0].AxisX.Title = channel1;
            chartFCS.ChartAreas[0].AxisY.Title = channel2;

            chartFCS.Series.Add("RBCs");
            chartFCS.Series[0].ChartType = SeriesChartType.Point;
            chartFCS.Series[0].MarkerSize = 3;
            chartFCS.Series[0].MarkerStyle = MarkerStyle.Circle;
            chartFCS.Series[0].MarkerColor = Color.Red;
            foreach (double[] point in orgRBCData)
            {
                chartFCS.Series[0].Points.AddXY(point[0], point[1]);
            }

            chartFCS.Series.Add("Plateletes");
            chartFCS.Series[1].ChartType = SeriesChartType.Point;
            chartFCS.Series[1].MarkerSize = 2;
            chartFCS.Series[1].MarkerStyle = MarkerStyle.Circle;
            chartFCS.Series[1].MarkerColor = Color.Blue;
            foreach (double[] point in orgPlateleteData)
            {
                chartFCS.Series[1].Points.AddXY(point[0], point[1]);
            }

            chartFCS.Refresh();
        }

        // Draw V data on chart
        private void DrawVChart()
        {
            chartV.Series[0].Points.Clear();

            IEnumerable<IGrouping<int, int>> VData = arrV.GroupBy(e => e);
            int nCnt = 0, nVal = 0;
            foreach (IGrouping<int, int> data in VData)
            {
                nVal = data.ElementAt(0);
                nCnt = data.Count();
                chartV.Series[0].Points.AddXY(nVal, nCnt);
            }

            chartV.Refresh();
        }

        //Draw HC data on chart
        private void DrawHCChart()
        {
            chartHC.Series[0].Points.Clear();
            IEnumerable<IGrouping<int, int>> HCData = arrHC.GroupBy(e => e);

            int nCnt = 0, nVal = 0;
            foreach (IGrouping<int, int> data in HCData)
            {
                nVal = data.ElementAt(0);
                nCnt = data.Count();
                chartHC.Series[0].Points.AddXY(nVal, nCnt);
            }

            chartHC.Refresh();

        }

        private void drawGates()
        {
            int i = 1;
            Color[] colors = new Color[2] { Color.Blue, Color.Red };
            // Draw Gates
            if (arrGatePolygon != null)
            {
                foreach (Polygon polygon in arrGatePolygon)
                {
                    chartFCS.Series.Add("Gate-" + i);
                    chartFCS.Series["Gate-" + i].Color = colors[i - 1];
                    chartFCS.Series["Gate-" + i].ChartType = SeriesChartType.Line;
                    foreach (PointF point in polygon.poly)
                    {
                        chartFCS.Series["Gate-" + i].Points.AddXY(point.X, point.Y);
                    }
                    i++;
                }
            }
        }

        private void drawWBC()
        {
            chartWBC.Series.Clear();

            if (totalWBCData.Count < 2)
                return;

            chartWBC.ChartAreas[0].AxisX.Title = WBC_channel1;
            chartWBC.ChartAreas[0].AxisY.Title = WBC_channel2;

            chartWBC.Series.Add("WBC Points");
            chartWBC.Series["WBC Points"].MarkerColor = Color.Blue;
            chartWBC.Series["WBC Points"].MarkerStyle = MarkerStyle.Circle;
            chartWBC.Series["WBC Points"].MarkerSize = 3;
            chartWBC.Series["WBC Points"].ChartType = SeriesChartType.Point;

            foreach (double[] point in totalWBCData)
            {
                chartWBC.Series["WBC Points"].Points.AddXY(point[0], point[1]);
            }
        }
        private bool ReadRecords()
        {
            arrRecords.Clear();
            record_min_s1 = 1000;
            record_max_s1 = 0;
            record_min_s2 = 1000;
            record_max_s2 = 0;
            try
            {
                using (System.IO.StreamReader file = new System.IO.StreamReader(@".\result.txt"))
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
                            record_min_s1 = Math.Min(record_min_s1, record.S1);
                            record_max_s1 = Math.Max(record_max_s1, record.S1);
                            record_min_s2 = Math.Min(record_min_s2, record.S2);
                            record_max_s2 = Math.Max(record_max_s2, record.S2);

                            arrRecords.Add(record);
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }

            }
            catch
            {
                return false;
            }

            return true;
        }

        private void FormRBC_Load(object sender, EventArgs e)
        {
            btnLoadWBC.Enabled = false;
            btnCalculate.Enabled = false;
            if (!ReadRecords())
            {
                MessageBox.Show("There is not result.txt file, which contains Mie V,HC -> S1, S2 calculation result. Please add the file and restart app.");
                btnRBC.Enabled = false;
                return;
            }

            chartWBC.ChartAreas[0].AxisX.IsMarginVisible = false;
            chartWBC.ChartAreas[0].AxisY.IsMarginVisible = false;

            chartFCS.ChartAreas[0].AxisX.IsMarginVisible = false;
            chartFCS.ChartAreas[0].AxisY.IsMarginVisible = false;

            chartV.ChartAreas[0].AxisX.Title = "V (fl)";
            chartV.ChartAreas[0].AxisY.Title = "Number of RBC for V";

            chartHC.ChartAreas[0].AxisX.Title = "HC (g/dl)";
            chartHC.ChartAreas[0].AxisY.Title = "Number of RBC for HC";

        }
        #endregion


        #region WBC
        private void CalculateWBCCounts()
        {
            if (meanshift == null || !meanshift.kdeEnable)
            {
                meanshift = new Custom_Meanshift(totalWBCData);
                meanshift.CalculateKDE();
            }
            if (!meanshift.clusterEnalble)
            {
                clusters = meanshift.CalculateCluster();
            }

            int nNeutrophils = 0, nLymphocytes = 0, nMonocytes = 0;
            foreach (Cluster cluster in clusters)
            {
                switch (cluster.clusterName)
                {
                    case "Neutrophils":
                        nNeutrophils = cluster.points.Count;
                        break;
                    case "Lymphocytes":
                        nLymphocytes = cluster.points.Count;
                        break;
                    case "Monocytes":
                        nMonocytes = cluster.points.Count;
                        break;
                }
            }

            nWBCCnt = nNeutrophils + nLymphocytes + nMonocytes;

            drawClusters();
        }


        private void LoadWBC()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "FCS files|*.fcs";
                if (dlg.ShowDialog() == DialogResult.OK)
                {

                    btnCalculate.Enabled = false;
                    WBC_fcMeasure = new FCMeasurement(dlg.FileName);

                    foreach (String name in WBC_fcMeasure.ChannelsNames)
                    {
                        if (FCS1_H.Contains(name))
                            WBC_channel1 = name;
                        if (SSC_H.Contains(name))
                            WBC_channel2 = name;
                    }

                    if (String.IsNullOrEmpty(WBC_channel1) || String.IsNullOrEmpty(WBC_channel2))
                    {
                        MessageBox.Show("Invalid WBC file. Please select correct WBC file.");
                        return;
                    }

                    //                    nWBCCnt = fcWBC.Counts;
                }
                else
                {
                    return;
                }
            }
            if (extractWBCDataFromFCS())
                btnCalculate.Enabled = true;
        }
        // extract channel data from FCS data
        private bool extractWBCDataFromFCS()
        {
            if (WBC_fcMeasure == null)
            {
                return false;
            }

            int i = 0;

            totalWBCData.Clear();

            if (String.IsNullOrEmpty(WBC_channel1) || String.IsNullOrEmpty(WBC_channel2))
                return false;

            if (WBC_channel1 == WBC_channel2)
            {
                MessageBox.Show("Two channels are equal. Please select different channel!");
                return false;
            }

            // Get gates according to channel
            GetGates(WBC_channel1, WBC_channel2);

            double x, y;
            bool validPoint = false;
            for (i = 0; i < WBC_fcMeasure.Counts; i++)
            {
                x = WBC_fcMeasure.Channels[WBC_channel1].Data.ElementAt(i);
                y = WBC_fcMeasure.Channels[WBC_channel2].Data.ElementAt(i);

                validPoint = true;
                if (arrGatePolygon_WBC != null)
                {
                    validPoint = false;
                    foreach (Polygon polygon in arrGatePolygon_WBC)
                    {
                        if (polygon.IsInsidePoly(x, y))
                        {
                            validPoint = true;
                            break;
                        }
                    }
                }

                if (!validPoint)
                    continue;

                /*                if (Global.diff3_enable && x < Global.LEFT_BOTTOM_T)
                                    continue;
                */
                totalWBCData.Add(new double[2]
                {
                    WBC_fcMeasure.Channels[WBC_channel1].Data.ElementAt(i),
                    WBC_fcMeasure.Channels[WBC_channel2].Data.ElementAt(i)
                });
            }

            if (totalWBCData.Count < 2)
                return false;
            return true;
        }

        // get gate polygon according to channel
        private void GetGates(string channel1 = "", string channel2 = "")
        {
            arrGatePolygon_WBC = null;
            arrGate3Polygon = null;

            arrGatePolygon_WBC = FCMeasurement.loadPolygon(Path.Combine(WBCGatePath, gate1));
            arrGate3Polygon = FCMeasurement.loadPolygon(Path.Combine(WBCGatePath, gate3));
            if (arrGate3Polygon.Count < 3)
            {
                MessageBox.Show("The Cell Type gate file is incorrect.");
                return;
            }
            int i = 0;
            for (i = 0; i < 3; i++)
            {
                Global.CELL_CENTER[i] = Global.GetCentroid(arrGate3Polygon[i].poly);
            }
            Global.diff3_enable = true;
            Global.T_Y_1 = (int)arrGate3Polygon[2].poly[0].Y;
            Global.T_Y_2 = (int)arrGate3Polygon[0].poly[0].Y;
        }

        // Draw Clusters
        private void drawClusters()
        {
            if (clusters == null)
                return;

            chartWBC.Series.Clear();
            int i = 0, nCnt = clusters.Count;

            Random random = new Random();

            if (nCnt < 1)
            {
                MessageBox.Show("There is no cluster to display.");
                return;
            }

            foreach (Cluster cluster in clusters)
            {
                if (string.IsNullOrEmpty(cluster.clusterName))
                {
                    chartWBC.Series.Add("C" + (i + 1));
                    chartWBC.Series[i].MarkerColor = Color.FromArgb((int)(random.NextDouble() * 150),
                        (int)(random.NextDouble() * 150),
                        (int)(random.NextDouble() * 150));
                    chartWBC.Series[i].MarkerStyle = MarkerStyle.Circle;
                    chartWBC.Series[i].MarkerSize = 3;
                }
                else
                {
                    chartWBC.Series.Add(cluster.clusterName);
                    int idx = Array.IndexOf(Global.CELL_NAME, cluster.clusterName);
                    chartWBC.Series[i].MarkerColor = arrGate3Polygon[idx].color;
                    chartWBC.Series[i].MarkerStyle = CELL_MARKER[idx];
                    chartWBC.Series[i].MarkerSize = 3;
                }

                chartWBC.Series[i].ChartType = SeriesChartType.Point;
                chartWBC.Series[i].MarkerSize = 2;

                foreach (int idx in cluster.points)
                {
                    chartWBC.Series[i].Points.AddXY(totalWBCData[idx][0], totalWBCData[idx][1]);
                }
                i++;
            }

            chartWBC.Refresh();
        }
        #endregion
    }
}
