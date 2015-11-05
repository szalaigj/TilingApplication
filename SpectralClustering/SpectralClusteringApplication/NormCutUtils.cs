using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectralClusteringApplication
{
    public class NormCutUtils
    {
        public void determineBiPartitionOfGraph(Vector<double> secondLargestEigVec, 
            List<int> firstPart, List<int> secondPart)
        {
            for (int idx = 0; idx < secondLargestEigVec.Count; idx++)
            {
                if (secondLargestEigVec[idx] >= 0)
                {
                    firstPart.Add(idx);
                }
                else
                {
                    secondPart.Add(idx);
                }
            }
        }

        public void determineLeaves(int nodeNO, Matrix<double> theta, Matrix<double> randomWalkMX, 
            Vector<double> pi)
        {
            string[] signTraces = determineSignTraces(nodeNO, theta);
            int dist = computeHammingDistance(signTraces[0], signTraces[1]);
            List<int> firstPart;
            List<int> secondPart;
            List<int> nodeList = Enumerable.Range(0, nodeNO).ToList();
            determinePotentialParts(signTraces, nodeList, out firstPart, out secondPart);
            NormCutComparer normCutComparer = new NormCutComparer();
            SortedDictionary<NormCutLeaf, int> sortedDict = new SortedDictionary<NormCutLeaf, int>(normCutComparer);
            NormCutLeaf firstPartLeaf = determineNormCutLeafForPart(signTraces, firstPart, randomWalkMX, pi);
            NormCutLeaf secondPartLeaf = determineNormCutLeafForPart(signTraces, secondPart, randomWalkMX, pi);
            sortedDict.Add(firstPartLeaf, 0);
            sortedDict.Add(secondPartLeaf, 0);
            NormCutLeaf n = sortedDict.First().Key;
        }

        private int computeHammingDistance(string signTrace1, string signTrace2)
        {
            int distance = 0;
            for (int idx = 0; idx < signTrace1.Length; idx++)
            {
                if (signTrace1[idx] != signTrace2[idx])
                {
                    distance++;
                }
            }
            return distance;
        }

        private NormCutLeaf determineNormCutLeafForPart(string[] signTraces, List<int> part, 
            Matrix<double> randomWalkMX, Vector<double> pi)
        {
            List<int> firstPart;
            List<int> secondPart;
            determinePotentialParts(signTraces, part, out firstPart, out secondPart);
            double normCutValue = determineNormCut(randomWalkMX, pi, firstPart, secondPart);
            NormCutLeaf normCutLeaf = new NormCutLeaf(normCutValue, part, firstPart, secondPart);
            return normCutLeaf;
        }

        private string[] determineSignTraces(int nodeNO, Matrix<double> theta)
        {
            string[] signTraces = new string[nodeNO];
            Evd<double> evdOfTheta = theta.Evd();
            for (int idx = 0; idx < nodeNO; idx++)
            {
                // The following vector should be reversed because the original ordering of eigen values is ascending:
                Vector<double> currentRow = evdOfTheta.EigenVectors.Row(idx);
                // the first eigen vector is unnecessary for the following so ...Take(currentRow.Count - 1)...
                List<double> currentRowList = currentRow.Take(currentRow.Count - 1).Reverse()
                    .Take(nodeNO * nodeNO - 1).ToList();
                string signTrace = "";
                foreach (var item in currentRowList)
                {
                    signTrace = determineSign(signTrace, item);
                }
                signTraces[idx] = signTrace;
            }
            return signTraces;
        }

        private string determineSign(string str, double item)
        {
            if (item > 0)
            {
                str += "+";
            }
            else
            {
                str += "-";
            }
            return str;
        }

        private void determinePotentialParts(string[] signTraces, List<int> nodeList,
            out List<int> firstPart, out List<int> secondPart)
        {
            Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
            for (int levelIdx = 1; (levelIdx <= signTraces[0].Length) && (dict.Keys.Count < 2); levelIdx++)
            {
                dict.Clear();
                foreach (var nodeIdx in nodeList)
                {
                    string subSignTrace = signTraces[nodeIdx].Substring(0, levelIdx);
                    List<int> currentElements;
                    if (dict.TryGetValue(subSignTrace, out currentElements))
                    {
                        currentElements.Add(nodeIdx);
                        dict[subSignTrace] = currentElements;
                    }
                    else
                    {
                        dict[subSignTrace] = new List<int>(new int[] { nodeIdx });
                    }
                }
            }
            string[] keyArray = dict.Keys.ToArray();
            firstPart = new List<int>(dict[keyArray[0]]);
            secondPart = new List<int>(dict[keyArray[1]]);
        }

        public double determineNormCut(Matrix<double> randomWalkMX, Vector<double> pi,
            List<int> firstPart, List<int> secondPart)
        {
            double result;
            double sumOfFirstPartPi = sumAPartOfPi(pi, firstPart);
            double sumOfSecondPartPi = sumAPartOfPi(pi, secondPart);
            double transProbFromFirstToSecondPart = determineTransitionProbBetweenTwoParts(randomWalkMX, pi,
                firstPart, secondPart);
            double transProbFromSecondToFirstPart = determineTransitionProbBetweenTwoParts(randomWalkMX, pi,
                secondPart, firstPart);
            result = (transProbFromFirstToSecondPart / sumOfFirstPartPi) +
                (transProbFromSecondToFirstPart / sumOfSecondPartPi);
            return result;
        }

        private double sumAPartOfPi(Vector<double> pi, List<int> part)
        {
            double sum = 0.0;
            foreach (var idx in part)
            {
                sum += pi[idx];
            }
            return sum;
        }

        private double determineTransitionProbBetweenTwoParts(Matrix<double> randomWalkMX, Vector<double> pi,
            List<int> partA, List<int> partB)
        {
            double transProb = 0.0;
            foreach (var idx in partA)
            {
                double currentStatProb = pi[idx];
                foreach (var subIdx in partB)
                {
                    transProb += currentStatProb * randomWalkMX[idx, subIdx];
                }
            }
            return transProb;
        }
    }
}
