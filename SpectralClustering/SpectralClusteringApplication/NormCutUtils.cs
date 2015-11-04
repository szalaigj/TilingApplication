using MathNet.Numerics.LinearAlgebra;
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
