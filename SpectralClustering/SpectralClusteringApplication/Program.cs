﻿using KMedoids;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
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
            int serverNO = 9;
            int spaceDimension = 2;
            int histogramResolution = 10;
            //int depth = 2;
            int depth = serverNO;
            // The following may be better choice than depth = K:
            //int depth = K - 1;// However, the 'graph-dimension' (histogram dimension) may be enough.
            InputParser parser = new InputParser();
            RandomWalkDesigner randomWalkDesigner = new RandomWalkDesigner();
            ThetaMatrixFormation thetaMatrixFormation = new ThetaMatrixFormation();
            NormCutUtils normCutUtils = new NormCutUtils();
            PartitioningBasedOnSpectrumAlgo partitioningBasedOnSpectrumAlgo = new PartitioningBasedOnSpectrumAlgo();
            PartitioningAroundMedoidsAlgo kMedoidsAlgo = new PartitioningAroundMedoidsAlgo();
            KMeansAlgo kMeansAlgo = new KMeansAlgo();
            IndexTransformator transformator = new IndexTransformator();
            
            int vertexNO;
            Matrix<double> weightMX = parser.parseWeightMatrix(out vertexNO);
            Console.Out.WriteLine("Weight matrix:");
            Console.Out.WriteLine(weightMX);
            BisectionAlgo bisectionAlgo = new BisectionAlgo(randomWalkDesigner, thetaMatrixFormation, weightMX, 
                serverNO, alpha);
            SpectralTreeNode[] spectralTreeLeaves = bisectionAlgo.apply(vertexNO);
            writeOutFiles(spectralTreeLeaves, transformator, spaceDimension, histogramResolution, vertexNO);

            Matrix<double> pageRankMX = randomWalkDesigner.createPageRankMX(weightMX, alpha);
            Console.Out.WriteLine("PageRank matrix:");
            Console.Out.WriteLine(pageRankMX);
            Vector<double> pi = randomWalkDesigner.createStationaryDistributionOf(pageRankMX);
            Console.Out.WriteLine("Stationary distribution of the random walk:");
            Console.Out.WriteLine(pi);
            Matrix<double> theta = thetaMatrixFormation.createThetaMX(pageRankMX, pi);
            Console.Out.WriteLine("Theta matrix:");
            Console.Out.WriteLine(theta);

            // The following is unused but it may be good for later use:
            //normCutUtils.determineLeaves(nodeNO, theta, pageRankMX, pi);

            Dictionary<string, List<int>> dict = partitioningBasedOnSpectrumAlgo.apply(serverNO, vertexNO, theta);
            printCluster<string>(dict);

            double[] weights;
            Matrix<double> objCoords = thetaMatrixFormation.determineCoordsBasedOnEigVecs(theta, depth, out weights);

            //Dictionary<int, List<int>> dict = kMeansAlgo.apply(objCoords, K, depth);
            //printCluster<int>(dict);

            //printObjCoords(objCoords, vertexNO, depth);
            //Cluster[] clusters = kMedoidsAlgo.apply(objCoords, K, weights);
            //foreach (var item in clusters)
            //{
            //    item.printCluster();
            //}
            Console.WriteLine("Press any key to exit!");
            Console.Read();
        }

        private static void writeOutFiles(SpectralTreeNode[] spectralTreeLeaves, IndexTransformator transformator,
            int spaceDimension, int histogramResolution, int vertexNO)
        {
            StringBuilder strBldr = new StringBuilder();
            for (int leafIdx = 0; leafIdx < spectralTreeLeaves.Length; leafIdx++)
            {
                spectralTreeLeaves[leafIdx].printCoords(leafIdx);
                spectralTreeLeaves[leafIdx].writeToStringBuilder(strBldr);
            }
            string serversOutput = @"c:\temp\data\servers.dat";
            System.IO.File.WriteAllText(serversOutput, strBldr.ToString());
            strBldr.Clear();
            writeVerticesToStringBuilder(transformator, spaceDimension, histogramResolution, vertexNO, strBldr);
            string verticesOutput = @"c:\temp\data\vertices.dat";
            System.IO.File.WriteAllText(verticesOutput, strBldr.ToString());
        }

        private static void writeVerticesToStringBuilder(IndexTransformator transformator, int spaceDimension, 
            int histogramResolution, int vertexNO, StringBuilder strBldr)
        {
            for (int vertexIdx = 0; vertexIdx < vertexNO; vertexIdx++)
            {
                int[] indicesArray = new int[spaceDimension];
                transformator.transformCellIdxToIndicesArray(histogramResolution, indicesArray, vertexIdx);
                // TODO: the following is no valid heft for vertex:
                int heft = 0;
                strBldr.Append(heft);
                for (int idx = 0; idx < spaceDimension; idx++)
                {
                    // the following contains the lower and upper bound for bin which is the same:
                    strBldr.Append(" ").Append(indicesArray[idx]).Append(" ").Append(indicesArray[idx]);
                }
                strBldr.AppendLine();
            }
        }

        private static void printCluster<T>(Dictionary<T, List<int>> dict)
        {
            foreach (var key in dict.Keys)
            {
                string str = "";
                foreach (var item in dict[key])
                {
                    str += " " + item;
                }
                Console.WriteLine(key + str);
            }
        }

        private static void printObjCoords(Matrix<double> objCoords, int vertexNO, int depth)
        {
            for (int idxObj = 0; idxObj < vertexNO; idxObj++)
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
