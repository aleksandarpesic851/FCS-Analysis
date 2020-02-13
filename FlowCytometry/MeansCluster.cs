using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MeansClustering
{
    public class MeansCluster //
    {
        private SharpCluster.Pattern pattern;
        private SharpCluster.PatternMatrix patternMatrix; // matrix of patterns
        private int index = 0; // index of pattern - identifier

        public MeansCluster(HashSet<double[]> dataSet)
        {
            SharpCluster.PatternMatrix patterMatrix = new SharpCluster.PatternMatrix();

            foreach (var item in dataSet)
            {
                pattern = new SharpCluster.Pattern();
                pattern.Id = index;
                pattern.AddAttributes(item);
                patternMatrix.AddPattern(pattern);

                index++;
            }
        }

        public MeansCluster(HashSet<double[]> dataSet, bool hasClass)
        {


        }

        public static double mShift(double x, double y, double[,] dataSet)//double x, double y, double[][] dataSet) //def original_points static
        {
            //double[,] dataSet
            double shift_x = 0;
            double shift_y = 0;
            double scale_factor = 0;
            double dist;
            double weight;
            double[][] p_temp;
            //            this.testData = testData;
            double kernel;
            double BandWidth = 0.8;

            int dataLength = dataSet.GetLength(0);

            double[] onlyX = new double[dataLength];
            double[] onlyY = new double[dataLength];
            //  double[] aveX = dataSet[0,];

            for (int j = 0; j < dataLength; j++)
            {
                onlyX[j] = dataSet[0, j];
                onlyY[j] = dataSet[1, j];
            }

            double MeanX = onlyX.Average();  // do not need to
            double MeanY = onlyY.Average();
            double[] CalcValue = new double[dataLength];
            double distance;
            //myArray.GetLength(0) 
            //dataSet //   phiCalc
            //TESTING KERNEL 
            //DenseMatrix.OfArray(
            //            Vector<double>[] nullspace = A.Kernel();
            //kernel = @(x)mean(phi((x - data) / h) / h);
            // dataSet.Average()
            //    double[] onlyX = dataSet[,0];

            //K_H(x) = (2*PI)^{-d/2}*|H|^{-1/2}*exp(-1/2*x^T*H^{-1}*x)
            //    double distance = Math.Sqrt(Math.Pow(x - MeanX, 2) + Math.Pow(y - MeanY, 2));

            /*

            for (int j = 0; j < dataLength; j++) //dataSet.Length
            {
                
                for (int i = 0; j < dataLength && i != j; j++) //dataSet.Length
                {
                    //                CalcValue[j] = phiCalc(dataSet[j, 0],dataSet[j, 1], MeanX, MeanY); //GetLength(0)
                    //distance
                    //phiCalc(j, dataSet); double phi = 
                }
                distance = 1;
                CalcValue[j] = 1 / Math.Sqrt(2 * Math.PI) * Math.Exp(-0.5 * distance / Math.Sqrt(2 * Math.PI));
                //System.IndexOutOfRangeException: 'Index was outside the bounds of the array.'
                //          CalcValue[j] = phiCalc(dataSet[j, 0]);
            }
            kernel = CalcValue.Average();

            */

            //test2D
            #region unused shifting
            // double kernel = dataSet.Average();
            //Math.Mean(phi((x - data) / h) / h);
            //Math.Mean
            // verify: the following should be approximately (0,0,0)
            //  (matrix * (2 * nullspace[0] - 3 * nullspace[1]))



            //foreach (String channelName in channelNames)
            /*
         foreach (int p in dataSet) //original_points
     {
         // numerator
         dist = euclidean_dist(p, p_temp)
         weight = kernel(dist, kernel_bandwidth)
         shift_x += p_temp[0] * weight
         shift_y += p_temp[1] * weight

       // denominator
         scale_factor += weight
         shift_x = shift_x / scale_factor
         shift_y = shift_y / scale_factor

     }
     */
            #endregion

            double[] shifts = new double[2];

            shifts[0] = shift_x;
            shifts[1] = shift_y;

            //   MessageBox.Show(Format.String(""))
            // shifts = { shift_x, shift_y};
            //return shifts; //shift_x; // 

            // Suppose we have the following data, and we would
            // like to estimate a distribution from this data

            double[][] samples =
            {
                new double[] { 0, 1 },
                new double[] { 1, 2 },
                new double[] { 5, 1 },
                new double[] { 7, 1 },
                new double[] { 6, 1 },
                new double[] { 5, 7 },
                new double[] { 2, 1 },
            };

            // Start by specifying a density kernel
            // IDensityKernel kernel = new GaussianKernel(dimension: 2);

            //gaussian_kernel
            // kernel would be:
            // double z = kernel.Function(new double[] { 0, 1 })



            //  p_new = self._shift_point(p_new, points, kernel_bandwidth)
            // { 0, 1 }
            double returnedValue = dataSet[0, 3];
            return returnedValue;
            //  return kernel; //shift_x; // 
        }

        // public meanShift(double[,] testData)
        //{
        //     this.testData = testData;
        // }

        //public class meanShift
        // {
        //     private double[,] testArray;
        //
        //     
        // }

        //
        //        public static double phiCalc(double x, double y,double MeanX, double MeanY)
        public static double phiCalc(double j, double[,] dataSet)
        {
            // double sigma
            // phi = @(x)exp(-.5 * x.^ 2) / sqrt(2 * pi);


            // double phi = 1/Math.Sqrt(2 * Math.PI)*Math.Exp(-0.5 * distance/ Math.Sqrt(2 * Math.PI));
            // return phi;
            return Math.PI;
        }




        public static double[,] KDE1d(double[] org_data, double sigma, int nbins) // KernelDensityEstimation 1 dimension
        {
            #region definitions
            // probability density function (PDF) signal analysis
            // Works like ksdensity in mathlab. 
            // KDE performs kernel density estimation (KDE)on one - dimensional data

            // http://en.wikipedia.org/wiki/Kernel_density_estimation

            // Input:	-data: input data, one-dimensional
            //          -sigma: bandwidth(sometimes called "h")
            //          -nbins: optional number of abscis points.If nbins is an
            //          array, the abscis points will be taken directly from it. (default 100)
            // Output:	-x: equispaced abscis points
            //          -y: estimates of p(x)

            // This function is part of the Kernel Methods Toolbox(KMBOX) for MATLAB. 

            // http://org_dataforge.net/p/kmbox

            #endregion

            double[,] result = new double[nbins, 2];
            double[] x = new double[nbins];
            double[] y = new double[nbins];

            double MAX;
            double MIN;

            double[] data = excludeZeros1D(org_data);

            int N = data.Length; // number of data points            

            // Find MIN MAX values in data
            MIN = data.Min();
            MAX = data.Max();
            // To get index use:       int indMax = Array.IndexOf(data, Max);

            // Like MATLAB linspace(MIN, MAX, nbins);

            x[0] = MIN;
            for (int i = 1; i < nbins; i++)
            {
                x[i] = x[i - 1] + ((MAX - MIN) / nbins);
            }


            // kernel density estimation

            double c = 1.0 / (Math.Sqrt(2 * Math.PI * sigma * sigma));

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < nbins; j++)
                {
                    y[j] = y[j] + 1.0 / N * c * Math.Exp(-0.5 * Math.Pow((data[i] - x[j]) / sigma, 2));
                    //    y[j] = y[j] + 1.0 / N * c * Math.Exp(-(data[i] - x[j]) * (data[i] - x[j]) / (2 * sigma * sigma));
                }
            }
            //

            // compilation of the X,Y to result. Good for creating plot(x, y)
            for (int i = 0; i < nbins; i++)
            {
                result[i, 0] = x[i]; // linearly spaced steps
                result[i, 1] = y[i]; // density estimate
            }
            return result;
        }


        public static double[,] KDE2d(double[,] org_data, double sigma_in, int nbins) // KernelDensityEstimation 1 dimension double[,] 
        {
            double[,] KDE2d_Result = new double[nbins, nbins];
            double[,] CoordinateVectors = new double[nbins, nbins];
            // double[,,] report3d = new double[nbins, nbins, nbins];
            int nbinsX = nbins;
            int nbinsY = nbins;
            int lengthData = org_data.Length;
            double[] x = new double[nbinsX];
            double[] y = new double[nbinsY];


            int Ndata = org_data.GetLength(0); // number of data points    
            double[] dataX = new double[Ndata];
            double[] dataY = new double[Ndata];
            double[] dataX_Raw = new double[Ndata];
            double[] dataY_Raw = new double[Ndata];

            for (int j = 0; j < Ndata; j++)
            {
                dataX_Raw[j] = org_data[j, 0]; //System.IndexOutOfRangeException: 'Index was outside the bounds of the array.'
                dataY_Raw[j] = org_data[j, 1];
            }

            double sigma = sigma_in; // sd(data) * (4 / 3 / length(data)) ^ (1 / 5); // sigma calculated by Silverman's rule of thumb
            double sigmaX = calculateSD(dataX_Raw) * Math.Pow(4 / 3 / lengthData, 1 / 5);
            double sigmaY = calculateSD(dataY_Raw) * Math.Pow(4 / 3 / lengthData, 1 / 5);
            // Find MIN MAX values in data
            double minX_Raw = dataX_Raw.Min();
            double maxX_Raw = dataX_Raw.Max();

            double minY_Raw = dataY_Raw.Min();
            double maxY_Raw = dataY_Raw.Max();


            // To get index use:       int indMax = Array.IndexOf(data, Max);

            // Like MATLAB linspace(MIN, MAX, nbins);

            bool useNormalizedData = false;

            if (useNormalizedData)
            {
                for (int j = 0; j < Ndata; j++)
                {
                    dataX[j] = (dataX_Raw[j] - minX_Raw) / (maxX_Raw - minX_Raw);
                    dataX[j] = (dataY_Raw[j] - minY_Raw) / (maxY_Raw - minY_Raw);
                }
            }
            else
            {                
                for (int j = 0; j < Ndata; j++)
                {
                      dataX[j] = dataX_Raw[j];
                      dataX[j] = dataY_Raw[j];
                }                
            }

            double minX = dataX.Min();
            double maxX = dataX.Max();

            double minY = dataY.Min();
            double maxY = dataY.Max();


            x[0] = minX;
            y[0] = minY;
            for (int i = 1; i < nbinsX; i++)
            {
                x[i] = x[i - 1] + ((maxX - minX) / nbinsX);
            }
            for (int i = 1; i < nbinsY; i++)
            {
                y[i] = y[i - 1] + ((maxY - minY) / nbinsY);
            }


            // kernel density estimation

            double cNorm = 1/ (Ndata*Math.Sqrt(2 * Math.PI * sigma * sigma)); 
            double Edist;
            // double[,] counterArray = new double[nbins, nbins] ;
            double[,] minDistArray = new double[nbins, nbins] ;
            double NormalizationIntegral = 0;
            for (int j = 0; j < nbinsY; j++) //Y -> rows, vertical variable
            {
                for (int k = 0; k < nbinsX; k++) //X -> columns, horizontal variable
                {
                    KDE2d_Result[j, k] = 0;
                    //  counterArray[j, k] = 0;
                    minDistArray[j, k] = 1000000000000;

                    for (int i = 0; i < Ndata; i++)
                    {
                        Edist = Math.Pow(dataX[i] - x[j], 2) + Math.Pow(dataY[i] - y[k], 2);
                        KDE2d_Result[j, k] += cNorm * Math.Exp(-0.5 * Edist / (sigma * sigma));
                        if (minDistArray[j, k]  > Edist)
                        {
                            minDistArray[j, k] = Edist;
                        }
                      //  counterArray[j, k] ++;
                    }
                    NormalizationIntegral += KDE2d_Result[j, k];
                }
            }

            // renormalize density
            for (int j = 0; j < nbinsY; j++) //Y -> rows, vertical variable
            {
                for (int k = 0; k < nbinsX; k++) //X -> columns, horizontal variable
                {
                    KDE2d_Result[j, k] = KDE2d_Result[j, k] / NormalizationIntegral;
                }
            }

            return KDE2d_Result; //x; /counterArray;// minDistArray; // 
        }

        public static double[] localDensity(double[,] data, double sigma) // KernelDensityEstimation 1 dimension    int
        {
            int Ndata = data.GetLength(0); // number of data points    
         //   int counter = 0;
            double[] localD = new double[Ndata];
            double[] xRaw = new double[Ndata];
            double[] yRaw = new double[Ndata];
            double[] x = new double[Ndata];
            double[] y = new double[Ndata];

            double minX;
            double maxX;
            double minY;
            double maxY;

            for (int j = 0; j < Ndata; j++)
            {
                xRaw[j] = data[j, 0];
                yRaw[j] = data[j, 1];
            }

            // Find MIN MAX values in data
            minX = xRaw.Min();
            maxX = xRaw.Max();

            minY = yRaw.Min();
            maxY = yRaw.Max();

            for (int j = 0; j < Ndata; j++)
            {
                x[j] = (xRaw[j] - minX)/(maxX - minX);
                y[j] = (yRaw[j] - minY)/(maxY - minY);
        }
            // To get index use:       int indMax = Array.IndexOf(data, Max);

            // kernel density estimation
             
            double cNorm = 1/(Ndata * Math.Sqrt(2 * Math.PI)); // constant has a factor proportional to  1/ sigma * sigma
            double Edist;
           // double expFactor = 10/(maxX*maxY);
            double NormalizationFactor = 0;
            //  double[] Edist_Min = new double[Ndata];

            for (int j = 0; j < Ndata; j++) 
            {
                localD[j] = 0;
             //   Edist_Min[j]= 0;
                for (int k = 0; k < Ndata; k++) //  for (int k = 0; (k < Ndata && j != k); k++)
                {
                    if  (j != k)
                    {
                        Edist = Math.Pow(x[j] - x[k], 2) + Math.Pow(y[j] - y[k], 2);
                        localD[j] += cNorm * Math.Exp(-0.5 * Edist / (sigma * sigma)); // normalizing coeff includes 1 / Ndata localD[j]
                    }                    
                }
                NormalizationFactor += localD[j]; // NORMALIZE data (integral = 1)
            }
            for (int j = 0; j < Ndata; j++)
            {
                localD[j] = localD[j] / NormalizationFactor;
            }
                /*
                double maxVal = localD.Max();

                for (int j = 0; j < Ndata; j++)
                {
                     localD[j] = 1 / Ndata * cNorm * localD[j] / maxVal;
                }
                */
                // return counter;


                //   Edist_Min[j]= 0;


                return localD; //Edist_Min;
            //  return Ndata;
        }

        public static double[] excludeZeros1D(double[] org_data)
        {
            //            double[,] result = null;
            // List<Tuple<double,double>> clearedList = new List<Tuple<double, double>>();
            List<double> clearedList = new List<double>();
            double x;
            double y;
            double[] result;
            int lengthData = org_data.GetLength(0);
            for (int j = 0; j < lengthData; j++)
            {
                x = org_data[j];

                if (x != 0)
                {
                    clearedList.Add(x);
                }
            }

            result = clearedList.ToArray();
            //   double[] result = To2D(resultStruct);

            return result;
        }

        public static double[,] excludeZeros2D(double[,] org_data)
        {
            //            double[,] result = null;
            // List<Tuple<double,double>> clearedList = new List<Tuple<double, double>>();
            List<double[]> clearedList = new List<double[]>();
            double x;
            double y;
            double[][] resultStruct = null;
            int lengthData = org_data.GetLength(0);
            for (int j = 0; j < lengthData; j++)
            {
                x = org_data[j, 0];
                y = org_data[j, 1];

                if (x != 0 && y != 0)
                {
                    clearedList.Add(new double[2] { x, y });
                }
            }

            resultStruct = clearedList.ToArray();
            double[,] result = To2D(resultStruct);

            return result;
        }

        public static double[,] To2D(double[][] org_data) //<double>
        {
            try
            {
                int FirstDim = org_data.Length;
                int SecondDim = org_data.GroupBy(row => row.Length).Single().Key; // throws InvalidOperationException if org_data is not rectangular

                var result = new double[FirstDim, SecondDim];
                for (int i = 0; i < FirstDim; ++i)
                    for (int j = 0; j < SecondDim; ++j)
                        result[i, j] = org_data[i][j];

                return result;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The given jagged array is not rectangular.");
            }
        }

        public static double EuclidDist(double[] pointA, double[] pointB)
        {
            double EuclidDist;
            double SumSquares = 0;
            for (int j = 0; j < pointA.Length; j++)
            {
                SumSquares += Math.Pow(pointA[j] - pointB[j], 2); //increment by distance

            }
            EuclidDist = Math.Sqrt(SumSquares);

            return EuclidDist;
        }

        public static double calculateSD(double[] data)
        {
            double average = data.Average();
            double sumOfSquaresOfDifferences = data.Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / (data.Length - 1));
            
            return sd;
        }
   

        /*

        ///public static CreateIsobars: IEnumerable:  (double[][] data, double stepSize = 1)
        public static IEnumerable CreateIsobars(double[][] data, double stepSize = 1)
        {
            if (data == null)
            {
                return null;
            }
            double x;
            double y;
            IEnumerable[][] hgrid = new IEnumerable[data.Length][];
            IEnumerable[][] vgrid = new IEnumerable[data.Length – 1][];
            for (int x = 0; x < data.Length; x++)
            {
                if (x != data.Length – 1)
                {
                vgrid[x] = new IEnumerable[data[x].Length];
                }                
            }
            hgrid[x] = new IEnumerable[data[x].Length – 1];
            for (int y = 0; y < data[x].Length; y++)
            {
                var value = data[x][y];
                if (x != 0)
                {
                    double value2 = data[x – 1][y];
                    int rem0 = (int)(value / stepSize);
                    int rem1 = (int)(value2 / stepSize);
                    var newS = new List();
                    // IF we're crossing a threshold
                    if (rem0 != rem1)
                    {
                        newS = Enumerable.Range(Math.Min(rem0, rem1) + 1, Math.Abs(rem0 – rem1)).Select(a => a * stepSize).ToList();
                    }
                    vgrid[x – 1][y] = newS
                    .Select(v => new IsobarPoint
                    {
                        Coordinate = new Coordinate(x – 1, y),
                        Location = new
                    Point((x – (v – value) / (data[x – 1][y] – value)), y),
Direction = (value > data[x – 1][y]) ? IsobarDirection.East : IsobarDirection.West,
Value = v
}).ToList();
    }
if (y<data[x].Length - 1)
{
double value2 = data[x][y + 1];
    int rem0 = (int)(value / stepSize);
    int rem1 = (int)(value2 / stepSize);
    var newS = new List();
// IF we're crossing a threshold
if (rem0 != rem1)
{
newS = Enumerable.Range(Math.Min(rem0, rem1) + 1, Math.Abs(rem0 – rem1)).Select(a => a* stepSize).ToList();
}
hgrid[x][y] = newS
.Select(v => new IsobarPoint
{
Coordinate = new Coordinate(x, y),
Location = new Point(x, (y + (v – value) / (data[x][y + 1] – value))),
Direction = (value > data[x][y + 1]) ? IsobarDirection.South : IsobarDirection.North,
Value = v
}).ToList();
}
}
}
return GenerateIsobars(vgrid, hgrid);
}

Like
Reply

    */

      

    }
}
