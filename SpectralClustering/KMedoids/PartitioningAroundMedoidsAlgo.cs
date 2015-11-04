using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMedoids
{
    /// <summary>
    /// This implementation is based on the following articles:
    /// 1.  Kaufman, Leonard and Rousseeuw, Peter. (1987)
    ///     Clustering by means of Medoids.
    ///     Statistical Data Analysis Based on the L1–Norm and Related Methods, North-Holland, 405–416.
    ///     
    /// 2.  Park, Hae-Sang and Jun, Chi-Hyuck. (2009)
    ///     A simple and fast algorithm for K-medoids clustering.
    ///     Expert Systems with Applications 36.2: 3336–3341.
    /// </summary>
    public class PartitioningAroundMedoidsAlgo
    {
        public Cluster[] apply(Matrix<double> objCoords, int K, double[] weights)
        {
            Cluster[] clusters;
            EuclideanDistMXComputer distMXComp = new EuclideanDistMXComputer();
            ClusteringManagementUtils utils = new ClusteringManagementUtils();
            InitialMedoidsSelection initMedoidsSelection = new InitialMedoidsSelection(utils);
            MedoidsUpdater medoidsUpdater = new MedoidsUpdater();
            ObjectsAssigner objectsAssigner = new ObjectsAssigner(utils);

            Matrix<double> distanceMX = distMXComp.determineEuclideanDistMX(objCoords);
            //Matrix<double> distanceMX = distMXComp.determineWeightedEuclideanDistMX(objCoords, weights);
            double currentTotalCost;
            initMedoidsSelection.selectInitialMedoids(distanceMX, K, out clusters, out currentTotalCost);
            double newTotalCost = currentTotalCost;
            bool firstIter = true;
            do
            {
                if (firstIter)
                {
                    firstIter = false;
                }
                else
                {
                    currentTotalCost = newTotalCost;
                }
                medoidsUpdater.updateMedoids(K, clusters);
                newTotalCost = objectsAssigner.assignObjsToClusters(distanceMX, K, clusters);
            } while (newTotalCost < currentTotalCost);
            return clusters;
        }
    }
}
