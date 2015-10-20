using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectralClusteringApplication
{
    public class ThetaMatrixFormation
    {
        public Matrix<double> createThetaMX(Matrix<double> randomWalkMX)
        {
            Vector<double> pi = createStationaryDistributionOf(randomWalkMX);
            Matrix<double> squaredRootPiMX = Matrix<double>.Build.DenseDiagonal(randomWalkMX.RowCount,
                randomWalkMX.ColumnCount, i => Math.Sqrt(pi[i]));
            Matrix<double> squaredRootPiInverseMX = Matrix<double>.Build.DenseDiagonal(randomWalkMX.RowCount,
                randomWalkMX.ColumnCount, i => (1.0 / Math.Sqrt(pi[i])));
            Matrix<double> firstPartOfTheta = squaredRootPiMX.Multiply(randomWalkMX).Multiply(squaredRootPiInverseMX);
            Matrix<double> theta = firstPartOfTheta.Add(firstPartOfTheta.Transpose()).Divide(2.0);
            return theta;
        }

        private Vector<double> createStationaryDistributionOf(Matrix<double> randomWalkMX)
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
            return pi;
        }

        public Vector<double> determineSecondLargestEigVec(Matrix<double> theta)
        {
            Evd<double> evdOfTheta = theta.Evd();
            //List<Complex> eigenComplexValuesOfTheta = evdOfTheta.EigenValues.ToList();
            List<double> eigenValuesOfTheta = evdOfTheta.EigenValues.
                Select(eigComplexVal => eigComplexVal.Real).ToList();
            eigenValuesOfTheta.Sort();
            double secondLargestEigVal = eigenValuesOfTheta[theta.RowCount - 2];
            int secondLargestIdx = 0;
            for (int idx = 0; idx < evdOfTheta.EigenValues.Count; idx++)
            {
                if (Math.Abs(evdOfTheta.EigenValues[idx].Real - secondLargestEigVal) < 0.0000001)
                {
                    secondLargestIdx = idx;
                }
            }
            return evdOfTheta.EigenVectors.Column(secondLargestIdx);
        }
    }
}
