using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMedoids
{
    public class ObjectsAssigner
    {
        private ClusteringManagementUtils utils;

        public ObjectsAssigner(ClusteringManagementUtils utils)
        {
            this.utils = utils;
        }

        public double assignObjsToClusters(Matrix<double> distanceMX, int K, Cluster[] clusters)
        {
            int[] selectedMedoids = new int[K];
            for (int selectedMedoidIdx = 0; selectedMedoidIdx < K; selectedMedoidIdx++)
            {
                selectedMedoids[selectedMedoidIdx] = clusters[selectedMedoidIdx].MedoidIdx;
                clusters[selectedMedoidIdx].Members.Clear();
            }
            utils.assign(distanceMX, K, selectedMedoids, clusters);
            double totalCost = utils.calculateTotalCost(K, clusters);
            return totalCost;
        }
    }
}
