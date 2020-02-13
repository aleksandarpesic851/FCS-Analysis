using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster.Partitional
{
    public class KMeans
    {
        private Pattern pattern; 
        private PatternMatrix patternMatrix; // matrix of patterns
        private int index = 0; // index of pattern - identifier

        public KMeans(HashSet<double[]> dataSet)
        {
            patternMatrix = new PatternMatrix();

            foreach (var item in dataSet)
            {
                pattern = new Pattern();
                pattern.Id = index;
                pattern.AddAttributes(item);
                patternMatrix.AddPattern(pattern);

                index++;
            }

        }

        public KMeans(HashSet<double[]> dataSet, bool hasClass)
        {
            patternMatrix = new PatternMatrix();

            if (hasClass)
            {
                foreach (var item in dataSet)
                {
                    pattern = new Pattern();
                    pattern.Id = index;
                    pattern.AddAttributes(item);
                    pattern.RemoveAttributeAt(pattern.GetDimension() - 1); //remove class attribute from attribute collection
                    pattern.ClassAttribute = Convert.ToInt32(item[pattern.GetDimension()]); //keep the standard class
                    patternMatrix.AddPattern(pattern);
                    index++;
                }
            }
            else
            {
                foreach (var item in dataSet)
                {
                    pattern = new Pattern();
                    pattern.Id = index;
                    pattern.AddAttributes(item);
                    patternMatrix.AddPattern(pattern);
                    index++;
                }

            }

        }


        public Cluster[] ExecuteClustering(int k, double[][] centers)
        {

            //=============PASS 1=========================================
            Cluster[] clusters = new Cluster[k]; //create k clusters
            
            //select K initial centroid clusters
            SelectCentroids(k, clusters, centers);

            //=============PASS 2=============================================
            //Form K groups of clusters based on promixity to centroids
            Pattern[] patterns = patternMatrix.GetPatterns();
            //MessageBox.Show("Loading cluster centers");

            BuildClusters(clusters, patterns);

            //=============PASS 3=============================================
            // -- Recalculate centroids for the group. Example: Cluster1 = {[2.6, 4.5], [1.5, 2.1], [5.3, 3.4]}
            //                                                             => (2.6 + 1.5 + 5.3)/3 = 3.1 
            //                                                             => (4.5 + 2.1 + 3.4)/3 = 3.3 
            //                                                             centroid = [3.1, 3.3]
            bool changed;
            RecomputeCentroids(clusters, out changed);

            //=============PASS 4=============================================
            // -- Checks if the centroids have changed.
            // -- If yes, repeat steps 2 and 3

            while (changed)
            {
                BuildClusters(clusters,  patterns);
                RecomputeCentroids(clusters, out changed);
            }

            return clusters;
        }

        private void RemovePatternsFromClusters(Cluster[] clusters)
        {
            foreach (Cluster cluster in clusters)
            {
                cluster.ClearPatterns();
            }
        }

        private void RecomputeCentroids(Cluster[] clusters, out bool changed)
        {
            double attributeSum = 0;           
            double attributeMean = 0;

            Pattern newCentroid;
            double[] centroidAttribute = new double[clusters[0].Centroid.GetDimension()];

            changed = false;

            // -- loop over groups
            for (int i = 0; i < clusters.Count(); i++)
            {
                // --  loops over each attribute of the pattern
                for (int j = 0; j < clusters[0].GetPattern(0).GetDimension(); j++)
                {
                    // -- loops over each pattern in the group
                    foreach (Pattern pattern in clusters[i].GetPatterns())
                    {
                        attributeSum += pattern.GetAttribute(j);
                    }

                    attributeMean = attributeSum / clusters[i].QuantityOfPatterns();
                    centroidAttribute[j] = attributeMean;
                    attributeSum = 0;
                }

                newCentroid = new Pattern();
                newCentroid.AddAttributes(centroidAttribute);
                newCentroid.Id = i;

                // checks if the group's centroid has changed. If so, then assign the new centroid to the group.
                if (!clusters[i].Centroid.GetAttributes().SequenceEqual(newCentroid.GetAttributes()))
                {
                    changed = true;
                    clusters[i].Centroid = newCentroid; //assigns new centroid to the group.
                }

            }

        }

        private void SelectCentroids(int k, Cluster[] clusters, double[][] centers)
        {
            //randomly selects K patterns as initial centroids

            Dictionary<int, int> centroidPatternList = new Dictionary<int, int>();
            int centroidPattern = -1;
            if (centers == null)
            {
                Random random = new Random();

                while (centroidPatternList.Count < k)
                {
                    centroidPattern = random.Next(0, patternMatrix.Size() - 1);
                    if (!centroidPatternList.ContainsKey(centroidPattern))
                    {
                        centroidPatternList.Add(centroidPattern, centroidPattern);
                    }
                }
                
            }
            else
            {
                for(int i=0; i<centers.Length; i++)
                {
                    double minRange = Double.MaxValue;
                    int index = -1;
                    for (int j = 0; j < patternMatrix.Size(); j++)
                    {
                        double range = 0;
                        for (int kk = 0; kk < centers[i].Length; kk++)
                        {
                            Pattern p = patternMatrix.GetPattern(j);
                            range += Math.Pow(centers[i][kk] - p.GetAttribute(kk), 2.0);
                        }
                        if (range < minRange)
                        {
                            index = j;
                            minRange = range;
                        }
                    }                    
                    centroidPatternList.Add(index, index);
                }
            }            

            for (int i = 0; i < k; i++)
            {
                //initialize the k groups
                clusters[i] = new Cluster();
                clusters[i].Id = i;
                clusters[i].Centroid = patternMatrix.GetPattern(centroidPatternList.ElementAt(i).Value); // links the centroid to the group
            }           
   
        }

        private void BuildClusters(Cluster[] clusters, Pattern[] patterns)
        {
            double distance = 0;
            Dictionary<Pattern, double> distances = new Dictionary<Pattern, double>(); //stores centroids and distances between patterns
            Pattern closestCentroid;

            // Removes patterns that are already in groups.
            RemovePatternsFromClusters(clusters);


            // -- Calculates the distance from each pattern to each centroid

            for (int i = 0; i < patterns.Count(); i++)
            {
                for (int j = 0; j < clusters.Count(); j++) 
                {
                    distance = Metrics.Distance.CalculatePatternDistance(patterns[i], clusters[j].Centroid); //calculates the distance from the current pattern to the group centroid
                    distances.Add(clusters[j].Centroid, distance); //stores the distance between the current pattern and the centroid
                }

                //check which centroid is closest to the default and add to a cluster
                distance = distances.Min(x => x.Value); //returns the shortest distance between the default and the k centroids.
                closestCentroid = distances.FirstOrDefault(x => x.Value == distance).Key; //returns the nearest centroid of the pattern
                distances.Clear();

                for (int l = 0; l < clusters.Count(); l++)
                {
                    if (clusters[l].Centroid.Id == closestCentroid.Id) //find the centroid group
                    {
                        clusters[l].AddPattern(patterns[i]); //add pattern to group
                    }
                }
            }

        }
    }
}
