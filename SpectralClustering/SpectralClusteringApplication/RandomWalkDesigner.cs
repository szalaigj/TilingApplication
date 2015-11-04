using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectralClusteringApplication
{
    public class RandomWalkDesigner
    {
        public Matrix<double> createDegreeDiagInverseMX(Matrix<double> weightMX)
        {
            Matrix<double> degreeDiagInverseMX = Matrix<double>.Build.DenseDiagonal(weightMX.RowCount, 0.0);
            for (int idx = 0; idx < weightMX.RowCount; idx++)
            {
                double outDegreeOfIdx = 0.0;
                for (int subIdx = 0; subIdx < weightMX.ColumnCount; subIdx++)
                {
                    outDegreeOfIdx += weightMX[idx, subIdx];
                }
                degreeDiagInverseMX[idx, idx] = 1.0 / outDegreeOfIdx;
            }
            return degreeDiagInverseMX;
        }

        public Matrix<double> createTransProbMX(Matrix<double> weightMX)
        {
            Matrix<double> degreeDiagInverseMX = createDegreeDiagInverseMX(weightMX);
            return degreeDiagInverseMX.Multiply(weightMX);
        }

        public Matrix<double> createTransProbLazyMX(Matrix<double> transProbMX)
        {
            return Matrix<double>.Build.DenseIdentity(transProbMX.RowCount).Add(transProbMX).Divide(2.0);
        }

        public Matrix<double> createPageRankMX(Matrix<double> weightMX, double alpha)
        {
            if ((alpha >= 0.0) && (alpha <= 1.0))
            {
                Matrix<double> transProbMX = createTransProbMX(weightMX);
                Matrix<double> transProbLazyMX = createTransProbLazyMX(transProbMX);
                Matrix<double> pageRankMX = transProbLazyMX.Multiply(alpha).Add((1 - alpha) / weightMX.RowCount);
                return pageRankMX;
            }
            else
            {
                throw new ArgumentException("The alpha should be between 0.0 and 1.0.");
            }
        }

        public Vector<double> createStationaryDistributionOf(Matrix<double> randomWalkMX)
        {
            Evd<double> evdOfRandomWalkMXTransposed = randomWalkMX.Transpose().Evd();
            Vector<double> pi = null;
            for (int idx = 0; idx < evdOfRandomWalkMXTransposed.EigenValues.Count; idx++)
            {
                if (Math.Abs(evdOfRandomWalkMXTransposed.EigenValues[idx].Real - 1.0) < 0.00001)
                {
                    pi = evdOfRandomWalkMXTransposed.EigenVectors.Column(idx);
                }
            }
            // see: https://en.wikipedia.org/wiki/Markov_chain#Stationary_distribution_relation_to_eigenvectors_and_simplices
            double sumPi = pi.Sum();
            pi = pi.Multiply(1 / sumPi);
            return pi;
        }
    }
}
