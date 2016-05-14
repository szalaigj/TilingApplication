using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpectralClusteringApplication
{
    public class BisectionAlgo
    {
        private RandomWalkDesigner randomWalkDesigner;
        private ThetaMatrixFormation thetaMatrixFormation;
        private Matrix<double> weightMX;
        private List<SpectralTreeNode> listOfLeaves;
        private SpectralTreeNodeComparer comparer;
        private int serverNO;
        private double alpha;

        public BisectionAlgo(RandomWalkDesigner randomWalkDesigner, ThetaMatrixFormation thetaMatrixFormation, 
            Matrix<double> weightMX, int serverNO, double alpha)
        {
            this.randomWalkDesigner = randomWalkDesigner;
            this.thetaMatrixFormation = thetaMatrixFormation;
            this.weightMX = weightMX;
            this.serverNO = serverNO;
            this.alpha = alpha;
            this.listOfLeaves = new List<SpectralTreeNode>();
            this.comparer = new SpectralTreeNodeComparer();
        }

        public SpectralTreeNode[] apply(int vertexNO)
        {
            SpectralTreeNode spectralTreeRoot = new SpectralTreeNode()
            {
                VertexList = Enumerable.Range(0, vertexNO).ToList() 
            };
            innerDetermineNormCutAndChildren(spectralTreeRoot, weightMX);
            innerDeterminePartition(spectralTreeRoot.FirstChild, spectralTreeRoot.SecondChild);
            return listOfLeaves.ToArray();
        }

        private void innerDeterminePartition(SpectralTreeNode spectralTreeNode1, SpectralTreeNode spectralTreeNode2)
        {
            innerDetermineNormCutAndChildren(spectralTreeNode1, determinePartOfWeightMX(spectralTreeNode1.VertexList));
            innerDetermineNormCutAndChildren(spectralTreeNode2, determinePartOfWeightMX(spectralTreeNode2.VertexList));
            listOfLeaves.Add(spectralTreeNode1);
            listOfLeaves.Add(spectralTreeNode2);
            if (listOfLeaves.Count < serverNO)
            {
                listOfLeaves.Sort(comparer);
                SpectralTreeNode leafWithMinNormCut = listOfLeaves[0];
                SpectralTreeNode firstChildOfLeafWithMinNormCut = leafWithMinNormCut.FirstChild;
                SpectralTreeNode secondChildOfLeafWithMinNormCut = leafWithMinNormCut.SecondChild;
                listOfLeaves.Remove(leafWithMinNormCut);
                innerDeterminePartition(firstChildOfLeafWithMinNormCut, secondChildOfLeafWithMinNormCut);
            }
        }

        private void innerDetermineNormCutAndChildren(SpectralTreeNode spectralTreeRoot, Matrix<double> partOfWeightMX)
        {
            Matrix<double> randomWalkMX = randomWalkDesigner.createPageRankMX(partOfWeightMX, alpha);
            Vector<double> pi = randomWalkDesigner.createStationaryDistributionOf(randomWalkMX);
            Matrix<double> theta = thetaMatrixFormation.createThetaMX(randomWalkMX, pi);
            Vector<double> secondLargestEigVec = thetaMatrixFormation.determineSecondLargestEigVec(theta);
            List<int> firstPartIdx, secondPartIdx;
            determineBiPartitionOfGraph(secondLargestEigVec, spectralTreeRoot, out firstPartIdx, out secondPartIdx);
            double normCutValue = determineNormCut(randomWalkMX, pi, firstPartIdx, secondPartIdx);
            spectralTreeRoot.NormCutValue = normCutValue;
        }

        private void determineBiPartitionOfGraph(Vector<double> secondLargestEigVec, SpectralTreeNode spectralTreeRoot,
            out List<int> firstPartIdx, out List<int> secondPartIdx)
        {
            firstPartIdx = new List<int>();
            secondPartIdx = new List<int>();
            List<int> firstPart = new List<int>();
            List<int> secondPart = new List<int>();
            for (int idx = 0; idx < secondLargestEigVec.Count; idx++)
            {
                if (secondLargestEigVec[idx] >= 0)
                {
                    firstPartIdx.Add(idx);
                    firstPart.Add(spectralTreeRoot.VertexList[idx]);
                }
                else
                {
                    secondPartIdx.Add(idx);
                    secondPart.Add(spectralTreeRoot.VertexList[idx]);
                }
            }
            SpectralTreeNode spectralFirstTreeNode = new SpectralTreeNode() { VertexList = firstPart };
            spectralTreeRoot.FirstChild = spectralFirstTreeNode;
            SpectralTreeNode spectralSecondTreeNode = new SpectralTreeNode() { VertexList = secondPart };
            spectralTreeRoot.SecondChild = spectralSecondTreeNode;
        }

        private double determineNormCut(Matrix<double> randomWalkMX, Vector<double> pi,
            List<int> firstPartIdx, List<int> secondPartIdx)
        {
            double result;
            double sumOfFirstPartPi = sumAPartOfPi(pi, firstPartIdx);
            double sumOfSecondPartPi = sumAPartOfPi(pi, secondPartIdx);
            double transProbFromFirstToSecondPart = determineTransitionProbBetweenTwoParts(randomWalkMX, pi,
                firstPartIdx, secondPartIdx);
            double transProbFromSecondToFirstPart = determineTransitionProbBetweenTwoParts(randomWalkMX, pi,
                secondPartIdx, firstPartIdx);
            result = (transProbFromFirstToSecondPart / sumOfFirstPartPi) +
                (transProbFromSecondToFirstPart / sumOfSecondPartPi);
            return result;
        }
        private double sumAPartOfPi(Vector<double> pi, List<int> partIdx)
        {
            double sum = 0.0;
            foreach (var idx in partIdx)
            {
                sum += pi[idx];
            }
            return sum;
        }

        private double determineTransitionProbBetweenTwoParts(Matrix<double> randomWalkMX, Vector<double> pi,
            List<int> partAIdx, List<int> partBIdx)
        {
            double transProb = 0.0;
            foreach (var idx in partAIdx)
            {
                double currentStatProb = pi[idx];
                foreach (var subIdx in partBIdx)
                {
                    transProb += currentStatProb * randomWalkMX[idx, subIdx];
                }
            }
            return transProb;
        }

        private Matrix<double> determinePartOfWeightMX(List<int> verticesOfPart)
        {
            Matrix<double> partOfWeightMX = Matrix<double>.Build.Dense(verticesOfPart.Count, verticesOfPart.Count);
            for (int idxOfFromNode = 0; idxOfFromNode < verticesOfPart.Count; idxOfFromNode++)
            {
                for (int idxOfToNode = 0; idxOfToNode < verticesOfPart.Count; idxOfToNode++)
                {
                    partOfWeightMX[idxOfFromNode, idxOfToNode] = 
                        weightMX[verticesOfPart[idxOfFromNode], verticesOfPart[idxOfToNode]];
                }
            }
            return partOfWeightMX;
        }

    }
}
