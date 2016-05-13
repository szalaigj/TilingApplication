using Accord.MachineLearning;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpectralClusteringApplication
{
    public class KMeansAlgo
    {
        public Dictionary<int, List<int>> apply(Matrix<double> objCoords, int K, int depth)
        {
            Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
            double[][] objects = new double[objCoords.RowCount][];
            for (int rowIdx = 0; rowIdx < objCoords.RowCount; rowIdx++)
            {
                objects[rowIdx] = new double[objCoords.ColumnCount];
                for (int columnIdx = 0; columnIdx < objCoords.ColumnCount; columnIdx++)
                {
                    objects[rowIdx][columnIdx] = objCoords[rowIdx, columnIdx];
                }
            }
            KMeans kmeans = new KMeans(K);
            int[] clusterIDs = kmeans.Compute(objects);
            for (int objIdx = 0; objIdx < clusterIDs.Length; objIdx++)
            {
                int clusterID = clusterIDs[objIdx];
                List<int> currentElements;
                if (dict.TryGetValue(clusterID, out currentElements))
                {
                    currentElements.Add(objIdx);
                    dict[clusterID] = currentElements;
                }
                else
                {
                    dict[clusterID] = new List<int>(new int[] { objIdx });
                }
            }
            return dict;
        }
    }
}
