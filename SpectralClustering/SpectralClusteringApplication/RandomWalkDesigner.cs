using MathNet.Numerics.LinearAlgebra;
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
    }
}
