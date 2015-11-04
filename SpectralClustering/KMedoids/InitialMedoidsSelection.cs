using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMedoids
{
    public class InitialMedoidsSelection
    {
        private ClusteringManagementUtils utils;

        public InitialMedoidsSelection(ClusteringManagementUtils utils)
        {
            this.utils = utils;
        }

        public void selectInitialMedoids(Matrix<double> distanceMX, int K, 
            out Cluster[] clusters, out double totalCost)
        {
            Dictionary<double, int> dictOfMiddleness = computeMiddlenessOfObjs(distanceMX);
            int[] selectedMedoids = determineMedoidsBasedOnMiddleness(dictOfMiddleness, K);
            clusters = buildClusters(distanceMX, K, selectedMedoids);
            totalCost = utils.calculateTotalCost(K, clusters);
        }

        private Dictionary<double, int> computeMiddlenessOfObjs(Matrix<double> distanceMX)
        {
            int nObj = distanceMX.RowCount;
            Matrix<double> middlenessMX = Matrix<double>.Build.Dense(nObj, nObj,
                (i, j) => distanceMX[i, j] / distanceMX.Row(i).Sum());
            Dictionary<double, int> dictOfMiddleness = new Dictionary<double, int>();
            for (int idx = 0; idx < nObj; idx++)
			{
                double currentMiddleness = middlenessMX.Column(idx).Sum();
                dictOfMiddleness[currentMiddleness] = idx;
			}
            return dictOfMiddleness;
        }

        private int[] determineMedoidsBasedOnMiddleness(Dictionary<double, int> dictOfMiddleness, int K)
        {
            int[] medoids = new int[K];
            List<double> middlenessKeys = dictOfMiddleness.Keys.ToList();
            middlenessKeys.Sort();
            for (int idx = 0; idx < K; idx++)
            {
                medoids[idx] = dictOfMiddleness[middlenessKeys[idx]];
            }
            return medoids;
        }

        private Cluster[] buildClusters(Matrix<double> distanceMX, int K, int[] selectedMedoids)
        { 
            Cluster[] clusters = new Cluster[K];
            for (int selectedMedoidIdx = 0; selectedMedoidIdx < K; selectedMedoidIdx++)
            {
                clusters[selectedMedoidIdx] = new Cluster(distanceMX, selectedMedoids[selectedMedoidIdx]);
            }
            utils.assign(distanceMX, K, selectedMedoids, clusters);
            return clusters;
        }
    }
}
