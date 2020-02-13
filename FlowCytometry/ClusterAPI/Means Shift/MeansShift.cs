using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace ClusterAPI
{
    public class MeansShift
    {
        // The dataset
        public static List<DataPoint> DataSet = new List<DataPoint>();
        // Constructor
        public MeansShift(string [] data_points)
        {
            // Fill Data
            foreach (var data_point in data_points)
            {
                DataSet.Add(new DataPoint(
                    double.Parse(data_point.Split(',')[0]),
                    double.Parse(data_point.Split(',')[1])
                ));
                
            }
            // Normalize data [0 , 1]
            //Normalize_data();

        }
        public static void Normalize_data()
        {
            //Normalize on X
            double max_X = DataSet.Aggregate((a,b)=> a.X > b.X ? a: b).X;
            Console.WriteLine("MAX X ="+max_X );
            DataSet = DataSet.Select((point) => new DataPoint(point.X / max_X, point.Y)).ToList();
            //Normalize on Y
            double max_Y = DataSet.Aggregate((a, b) => a.Y > b.Y ? a : b).Y;
            DataSet = DataSet.Select((point) => new DataPoint(point.X, point.Y / max_Y)).ToList();
        }

        private static double euclidean(DataPoint A,DataPoint B) => Math.Sqrt(Math.Pow(A.X - B.X, 2)+ Math.Pow(A.Y - B.Y, 2));
        #region Mean Shift with Gaussian Kernel Convergence
        //main method
        public static List<DataPoint> m_shift(double kernelBandwidth,double convergenceEpsilon,int minDensity)
        {
            // Make a copy of the original data and shift it until each point reachs the the peak with a difference of epsilon 
            //(at the end the datapoints inside this variable will be identical with (Delta)Datapoint < epsilon)
            List<DataPoint> copy = makeCopy();
            for (int i = 0; i < copy.Count; i++)
            {
                // for each point it will be shifted until it converges 
                while (true)
                {
                    DataPoint oldPoint = copy[i];
                    // Visualization
                    //removePoint(chart, oldPoint);

                    // change datapoint weight
                    copy[i] = shift(copy[i], kernelBandwidth);
                    // Visualization
                    //addPoint(chart, copy[i]);

                    //check convergence
                    if (euclidean(oldPoint, copy[i]) <= convergenceEpsilon)
                        break;
                }
                
            }
            // get culusters with a tolerance (in this case we rounded the cluster peak to 2 decimal numbers after the decimal point)
            copy = getUniquePeaks(copy, minDensity);
            // Visualization
            //dumpPoints(chart,chartOrg,copy);
            // the returned peaks
            return copy;
        }
        
        public static DataPoint shift(DataPoint copy_point,double kernelBandwidth)
        {
            double shift_x = 0;
            double shift_y = 0;
            double scale_factor = 0;
            for (int i = 0; i < DataSet.Count; i++)
            {
                //distance
                double dist = euclidean(copy_point,DataSet.ElementAt(i));
                //KDE
                double weight = gaussian_kernel(dist, kernelBandwidth);
                shift_x += DataSet.ElementAt(i).X * weight ;
                shift_y += DataSet.ElementAt(i).Y * weight ;
                scale_factor += weight;

            }
            shift_x = shift_x / scale_factor;
            shift_y = shift_y / scale_factor;

            return new DataPoint(shift_x, shift_y);
        }
        public static double gaussian_kernel(double dist,double sigma)
        {
           double U = dist / sigma;
            return Math.Exp(-0.5 * Math.Pow(U, 2)) / (sigma * Math.Sqrt(2 * Math.PI));
        }
        #endregion

        #region Mean Shift with Mean Convergence
        public static HashSet<DataPoint> fit_shift(double bandwidth)
        {
            HashSet<DataPoint> Centroids = new HashSet<DataPoint>();
            // Considering each dataPoint as a centroid at first
            foreach (DataPoint Point in DataSet)
                Centroids.Add(new DataPoint(Point.X,Point.Y));
            // Converging to Centers of masses
            while (true && Centroids.Count > 4)
            {
                Console.WriteLine("Start ------While-----");
                HashSet<DataPoint> New_Centroids = new HashSet<DataPoint>();
                for (int i = 0; i < Centroids.Count; i++)
                {
                    //Console.WriteLine($"Centroid ----- X : {Centroids.ElementAt(i).X} ----- Y : {Centroids.ElementAt(i).Y} ----");
                    List<DataPoint> in_bandwidth = new List<DataPoint>();
                    DataPoint Centroid = Centroids.ElementAt(i);
                    // Get the datapoints within the bandwidth
                    for (int j = 0; j < DataSet.Count; j++)
                        if (euclidean(Centroid, DataSet[j]) <= bandwidth)
                            in_bandwidth.Add(DataSet[j]);

                    // Center of mass of the bandwidth
                    DataPoint new_centroid = new DataPoint(in_bandwidth.Select(p => p.X).ToList().Sum() / in_bandwidth.Count,
                         in_bandwidth.Select(p => p.Y).ToList().Sum() / in_bandwidth.Count);
                    //Console.WriteLine($"Average Centroid Found ----- X : {new_centroid.X} ----- Y : {new_centroid.Y} ----");

                    New_Centroids.Add(new_centroid);

                }
                //Console.WriteLine("New Centroids = " + New_Centroids.Count);

                // Get Unique Centroids Only
                List<DataPoint> Uniques = UniqueCentroids(New_Centroids);
                //Console.WriteLine("Unique Centroids = " + Uniques.Count);

                // Save Last Version of centroids to check the convergence
                HashSet<DataPoint> PreviousCentroids = Centroids;

                // Re-Assigne the centroids with the new found
                Centroids = new HashSet<DataPoint>();
                for (int k = 0; k < Uniques.Count; k++)
                    Centroids.Add(Uniques[k]);
               // Console.WriteLine("Current Centroids = "+ Centroids.Count);
                //Console.WriteLine("Previous Centroids  = " + PreviousCentroids.Count);
                bool Optimized = IdenticalCentroids(Centroids, PreviousCentroids);
                //Console.WriteLine(Optimized);
                if (Optimized)
                    break;

            }
            Console.WriteLine(Centroids.Count);

            Centroids = new HashSet<DataPoint>(removeCentroidNoise(Centroids, bandwidth));
            Console.WriteLine(Centroids.Count);

            return Centroids;
        }
        
        private static List<DataPoint> removeCentroidNoise(HashSet<DataPoint> Centroids,double bandwidth) {

            Dictionary<List<DataPoint>, double> orderedList = new Dictionary<List<DataPoint>, double>();
            for (int i = 0; i < Centroids.Count; i++)
                for (int j = i + 1; j < Centroids.Count; j++)
                    orderedList.Add(new List<DataPoint> { Centroids.ElementAt(i), Centroids.ElementAt(j) },
                        euclidean(Centroids.ElementAt(i), Centroids.ElementAt(j)));
            return orderedList.Aggregate((a,b) => a.Value > b.Value ? a : b).Key;

        }
        private static bool IdenticalCentroids(HashSet<DataPoint> New, HashSet<DataPoint> Old)
        {
            //Console.WriteLine(New.Count);
           // Console.WriteLine(Old.Count);
            foreach (var Npoint in New)
            {
                bool exist = false;
                foreach (var Opoint in Old)
                {
                    if (Npoint.X == Opoint.X && Npoint.Y == Opoint.Y)
                    {
                        //Console.WriteLine($"####### {{{Npoint.X} ==== {Npoint.Y} ===> {Opoint.X} ==== {Opoint.Y} }}");
                        exist = true;
                        break;
                    }
                }
                    
                if (!exist)
                    return false;

            }
            return true;
        }
        private static List<DataPoint> UniqueCentroids(HashSet<DataPoint> Centroids)
        {
            Dictionary<string, int> temp = new Dictionary<string, int>();
            foreach (var item in Centroids)
                if (!temp.ContainsKey(item.X + "," + item.Y))
                    temp.Add(item.X + "," + item.Y, 1);
                
            return (from t in temp
                    select new DataPoint(double.Parse(t.Key.Split(',')[0]), double.Parse(t.Key.Split(',')[1]))).ToList<DataPoint>();

        }

        public static Dictionary<DataPoint, List<DataPoint>>  getSubClustersPoints(HashSet<DataPoint>  subClusters, List<DataPoint> DataSet)
        {
            Dictionary<DataPoint, List<DataPoint>> clusters = subClusters.ToDictionary(point => point,p => new List<DataPoint>());
            foreach (var Point in DataSet)
                clusters[subClusters
                            .ToDictionary(center => center, center => euclidean(center, Point))
                            .Aggregate((a,b) => a.Value < b.Value ? a : b).Key].Add(Point);
            Console.WriteLine("POINT "+ clusters.ElementAt(1).Value.Count);
            return clusters;

        }
        #endregion

        #region Just For Visualization
        public static List<DataPoint> makeCopy()
        {
            List<DataPoint> copy = new List<DataPoint>();
            foreach (DataPoint point in DataSet)
                copy.Add(new DataPoint(
                    point.X,
                    point.Y
                    ));
            return copy;
        }
        private static void addPoint(Chart chart,DataPoint newPoint)
        {
            chart.Invoke(new Action(()=> chart.Series[0].Points.AddXY(newPoint.X, newPoint.Y)));
                
            
        }
        private static void dumpPoints(Chart chart, Chart chartOrg, List<DataPoint>copy)
        {

            foreach (var item in copy)
            {
                chart.Invoke(new Action(() => chart.Series[1].Points.AddXY(item.X, item.Y)));
                chartOrg.Invoke(new Action(() => chartOrg.Series[1].Points.AddXY(item.X, item.Y)));

            }


        }
        private static void removePoint(Chart chart, DataPoint oldPoint)
        {
            chart.Invoke(new Action(() => chart.Series[0].Points.Remove(new System.Windows.Forms.DataVisualization.Charting.DataPoint(oldPoint.X, oldPoint.Y))));

        }
        private static List<DataPoint> getUniquePeaks(List<DataPoint> centroids,double minDensity)
        {
            Dictionary<string,int> temp = new Dictionary<string, int>();
            centroids = centroids.Select(point => new DataPoint(Math.Round(point.X, 5), Math.Round(point.Y, 5))).ToList();

            foreach (var item in centroids)
                if (temp.ContainsKey(item.X + "," + item.Y))
                    temp[item.X + "," + item.Y]++;
                else
                    temp.Add(item.X + "," + item.Y, 1);
            return (from t in temp
                   where t.Value > minDensity
                   select new DataPoint(double.Parse(t.Key.Split(',')[0]), double.Parse(t.Key.Split(',')[0]))).ToList<DataPoint>();
        }
        #endregion
        public static void ClearSeries(Chart chart) {
            foreach (var item in chart.Series)
                chart.Invoke(new Action(() => item.Points.Clear()));
 
        }
        public static void visualizeRealData(Chart chart)
        {
            foreach (var item in DataSet)
                chart.Invoke(new Action(() => chart.Series[0].Points.AddXY(item.X, item.Y)));
        }
        public static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
       
        public static DataPoint rotate_point(double cx, double cy, double angle, DataPoint p)
        {

            return new DataPoint(Math.Cos(ConvertToRadians(angle)) * (p.X - cx) - Math.Sin(ConvertToRadians(angle)) * (p.Y - cy) + cx,
                         Math.Sin(ConvertToRadians(angle)) * (p.X - cx) + Math.Cos(ConvertToRadians(angle)) * (p.Y - cy) + cy);
        }

        public static Equation getHyperPlaneEquation(DataPoint pA, DataPoint pB)
        {
            double a = (pA.Y - pB.Y) / (pA.X - pB.X);
            double b = pA.Y - (a * pA.X);
            return new Equation(a, b);

        }

    }
    public class Equation
    {
        public double a, b;
        public Equation(double a , double b)
        {
            this.a = a;
            this.b = b;
        }
    }
}
