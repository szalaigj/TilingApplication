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
            InputParser parser = new InputParser();
            RandomWalkDesigner randomWalkDesigner = new RandomWalkDesigner();
            ThetaMatrixFormation thetaMatrixFormation = new ThetaMatrixFormation();
            Matrix<double> weightMX = parser.parseWeightMatrix();
            Console.Out.WriteLine("Weight matrix:");
            Console.Out.WriteLine(weightMX);
            Matrix<double> pageRankMX = randomWalkDesigner.createPageRankMX(weightMX, 0.85);
            Console.Out.WriteLine("PageRank matrix:");
            Console.Out.WriteLine(pageRankMX);
            Matrix<double> theta = thetaMatrixFormation.createThetaMX(pageRankMX);
            Console.Out.WriteLine("Theta matrix:");
            Console.Out.WriteLine(theta);
            Vector<double> secondLargestEigVec = thetaMatrixFormation.determineSecondLargestEigVec(theta);
            Console.Out.WriteLine("The eigenvector of theta matrix which belongs to the second largest eigenvalue:");
            Console.Out.WriteLine(secondLargestEigVec);
            Console.WriteLine("Press any key to exit!");
            Console.Read();
        }
    }
}
