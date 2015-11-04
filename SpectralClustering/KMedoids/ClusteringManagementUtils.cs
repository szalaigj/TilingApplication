using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMedoids
{
    public class ClusteringManagementUtils
    {
        public void assign(Matrix<double> distanceMX, int K, int[] selectedMedoids, Cluster[] clusters)
        {
            int nObj = distanceMX.RowCount;
            for (int idx = 0; idx < nObj; idx++)
            {
                double minMedoidDist = double.MaxValue;
                int minSelectedMedoidIdx = -1;
                for (int selectedMedoidIdx = 0; selectedMedoidIdx < K; selectedMedoidIdx++)
                {
                    double currentMedoidDist = distanceMX[idx, selectedMedoids[selectedMedoidIdx]];
                    if (currentMedoidDist < minMedoidDist)
                    {
                        minMedoidDist = currentMedoidDist;
                        minSelectedMedoidIdx = selectedMedoidIdx;
                    }
                }
                clusters[minSelectedMedoidIdx].Members.Add(idx);
            }
        }

        public double calculateTotalCost(int K, Cluster[] clusters)
        {
            double totalCost = 0.0;
            for (int clusterIdx = 0; clusterIdx < K; clusterIdx++)
            {
                totalCost += clusters[clusterIdx].calculateCost();
            }
            return totalCost;
        }
    }
}
