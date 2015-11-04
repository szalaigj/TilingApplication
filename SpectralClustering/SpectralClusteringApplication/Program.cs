using KMedoids;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectralClusteringApplication
{
    class Program
    {
        /// <summary>
        /// This implementation is based on the following articles:
        /// 1.  Gleich, David. (2006)
        ///     Hierarchical directed spectral graph partitioning.
        ///     Tech. rep., Stanford University
        ///     
        /// 2.  Malliaros, Fragkiskos D., and Michalis Vazirgiannis. (2013)
        ///     Clustering and community detection in directed networks: A survey.
        ///     Physics Reports 533.4: 95-142.
        /// </summary>
        static void Main(string[] args)
        {
            double alpha = 0.85;
            int K = 9;
            int depth = 9;
            //int K = 5;
            // The following may be better choice than depth = K:
            //int depth = K - 1;// However, the 'graph-dimension' (histogram dimension) may be enough.
            InputParser parser = new InputParser();
            RandomWalkDesigner randomWalkDesigner = new RandomWalkDesigner();
            ThetaMatrixFormation thetaMatrixFormation = new ThetaMatrixFormation();
            NormCutUtils normCutUtils = new NormCutUtils();
            PartitioningAroundMedoidsAlgo kMedoidsAlgo = new PartitioningAroundMedoidsAlgo();
            int nodeNO;
            Matrix<double> weightMX = parser.parseWeightMatrix(out nodeNO);
            Console.Out.WriteLine("Weight matrix:");
            Console.Out.WriteLine(weightMX);
            Matrix<double> pageRankMX = randomWalkDesigner.createPageRankMX(weightMX, alpha);
            Console.Out.WriteLine("PageRank matrix:");
            Console.Out.WriteLine(pageRankMX);
            Vector<double> pi = randomWalkDesigner.createStationaryDistributionOf(pageRankMX);
            Console.Out.WriteLine("Stationary distribution of the random walk:");
            Console.Out.WriteLine(pi);
            Matrix<double> theta = thetaMatrixFormation.createThetaMX(pageRankMX, pi);
            Console.Out.WriteLine("Theta matrix:");
            Console.Out.WriteLine(theta);
            Vector<double> secondLargestEigVec = thetaMatrixFormation.determineSecondLargestEigVec(theta);
            //Console.Out.WriteLine("The eigenvector of theta matrix which belongs to the second largest eigenvalue:");
            //Console.Out.WriteLine(secondLargestEigVec);
            List<int> firstPart = new List<int>();
            List<int> secondPart = new List<int>();
            normCutUtils.determineBiPartitionOfGraph(secondLargestEigVec, firstPart, secondPart);
            Console.Out.WriteLine("Part A:");
            printPart(firstPart);
            Console.Out.WriteLine("Part B:");
            printPart(secondPart);
            //double normCutValue = normCutUtils.determineNormCut(pageRankMX, pi, firstPart, secondPart);
            //Console.Out.WriteLine("Value of normalized cut:");
            //Console.Out.WriteLine(normCutValue);
            //Vector<double> thirdLargestEigVec = thetaMatrixFormation.determineNthLargestEigVec(theta, 3);
            //Console.Out.WriteLine("The eigenvector of theta matrix which belongs to the third largest eigenvalue:");
            //Console.Out.WriteLine(thirdLargestEigVec);
            double[] weights;
            Matrix<double> objCoords = thetaMatrixFormation.determineCoordsBasedOnEigVecs(theta, depth, out weights);
            printObjCoords(objCoords, nodeNO, depth);
            Cluster[] clusters = kMedoidsAlgo.apply(objCoords, K, weights);
            foreach (var item in clusters)
            {
                item.printCluster();
            }
            Console.WriteLine("Press any key to exit!");
            Console.Read();
        }

        static private void printPart(List<int> part)
        {
            foreach (var item in part)
            {
                Console.Out.WriteLine(item);
            }
        }

        static private void printObjCoords(Matrix<double> objCoords, int nodeNO, int depth)
        {
            for (int idxObj = 0; idxObj < nodeNO; idxObj++)
            {
                string strCoords = "";
                for (int idxCoord = 0; idxCoord < depth; idxCoord++)
                {
                    strCoords += " " + objCoords[idxObj, idxCoord];
                }
                Console.Out.WriteLine("Node " + idxObj + ":" + strCoords);
            }
        }
    }
}
