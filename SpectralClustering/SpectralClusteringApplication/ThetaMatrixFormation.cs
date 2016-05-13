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
        public Matrix<double> createThetaMX(Matrix<double> randomWalkMX, Vector<double> pi)
        {
            Matrix<double> squaredRootPiMX = Matrix<double>.Build.DenseDiagonal(randomWalkMX.RowCount,
                randomWalkMX.ColumnCount, i => Math.Sqrt(pi[i]));
            Matrix<double> squaredRootPiInverseMX = Matrix<double>.Build.DenseDiagonal(randomWalkMX.RowCount,
                randomWalkMX.ColumnCount, i => (1.0 / Math.Sqrt(pi[i])));
            Matrix<double> firstPartOfTheta = squaredRootPiMX.Multiply(randomWalkMX).Multiply(squaredRootPiInverseMX);
            Matrix<double> theta = firstPartOfTheta.Add(firstPartOfTheta.Transpose()).Divide(2.0);
            return theta;
        }

        public Vector<double> determineSecondLargestEigVec(Matrix<double> theta)
        {
            Evd<double> evdOfTheta = theta.Evd();
            return determineNthLargestEigVec(theta, 2);
            // TODO: remove following comment lines:
            //List<double> eigenValuesOfTheta = evdOfTheta.EigenValues.
            //    Select(eigComplexVal => eigComplexVal.Real).ToList();
            //eigenValuesOfTheta.Sort();
            //double secondLargestEigVal = eigenValuesOfTheta[theta.RowCount - 2];
            //int secondLargestIdx = 0;
            //for (int idx = 0; idx < evdOfTheta.EigenValues.Count; idx++)
            //{
            //    if (Math.Abs(evdOfTheta.EigenValues[idx].Real - secondLargestEigVal) < 0.0000001)
            //    {
            //        secondLargestIdx = idx;
            //    }
            //}
            //return evdOfTheta.EigenVectors.Column(secondLargestIdx);
        }

        public Vector<double> determineNthLargestEigVec(Matrix<double> theta, int index)
        {
            Evd<double> evdOfTheta = theta.Evd();
            // Based on documentation (see: http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Factorization/Evd%601.htm#EigenValues)
            // The property EigenValues of Evd stores eigenvalues of matrix in ascending order
            int nthLargestIdx = theta.RowCount - index;
            // TODO: remove following comment lines:
            //List<double> eigenValuesOfTheta = evdOfTheta.EigenValues.
            //    Select(eigComplexVal => eigComplexVal.Real).ToList();
            //eigenValuesOfTheta.Sort();
            //double nthLargestEigVal = eigenValuesOfTheta[theta.RowCount - index];
            //int nthLargestIdx = 0;
            //for (int idx = 0; idx < evdOfTheta.EigenValues.Count; idx++)
            //{
            //    if (Math.Abs(evdOfTheta.EigenValues[idx].Real - nthLargestEigVal) < 0.0000001)
            //    {
            //        nthLargestIdx = idx;
            //    }
            //}
            return evdOfTheta.EigenVectors.Column(nthLargestIdx);
        }

        public Matrix<double> determineCoordsBasedOnEigVecs(Matrix<double> theta, int depth,
            out double[] normedFilteredEigenValuesOfTheta)
        {
            int nodeNO = theta.RowCount;
            Evd<double> evdOfTheta = theta.Evd();
            List<double> eigenValuesOfTheta = evdOfTheta.EigenValues.
                Select(eigComplexVal => eigComplexVal.Real).ToList();
            eigenValuesOfTheta.Sort();
            Matrix<double> objCoords = Matrix<double>.Build.DenseDiagonal(nodeNO, depth, 0.0);
            int cntOfDepth = 0;
            // TODO: remove following comment lines:
            //for (int idxOfEigVal = evdOfTheta.EigenValues.Count - 1; (idxOfEigVal >= 0) && (cntOfDepth != depth); 
            //    idxOfEigVal--)
            //{
            //    for (int nThEigVal = 2; nThEigVal < depth + 2; nThEigVal++)
            //    {
            //        double currentEigVal = eigenValuesOfTheta[nodeNO - nThEigVal];
            //        if (Math.Abs(evdOfTheta.EigenValues[idxOfEigVal].Real - currentEigVal) < 0.0000001)
            //        {
            //            Vector<double> currentEigVec = evdOfTheta.EigenVectors.Column(idxOfEigVal);
            //            for (int idxOfObj = 0; idxOfObj < nodeNO; idxOfObj++)
            //            {
            //                objCoords[idxOfObj, nThEigVal - 2] = currentEigVec[idxOfObj];
            //            }
            //            cntOfDepth++;
            //            break;
            //        }
            //    }
            //}
            // Based on documentation (see: http://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Factorization/Evd%601.htm#EigenValues)
            // The property EigenValues of Evd stores eigenvalues of matrix in ascending order
            for (int idx = evdOfTheta.EigenValues.Count - 1; (idx >= 0) && (cntOfDepth < depth); idx--)
            {
                Vector<double> currentEigVec = evdOfTheta.EigenVectors.Column(idx);
                for (int idxOfObj = 0; idxOfObj < nodeNO; idxOfObj++)
                {
                    objCoords[idxOfObj, cntOfDepth] = currentEigVec[idxOfObj];
                }
                cntOfDepth++;
            }
            eigenValuesOfTheta.Reverse();
            List<double> filteredEigenValuesOfTheta = eigenValuesOfTheta.GetRange(0, depth);
            double norm = filteredEigenValuesOfTheta.Sum();
            normedFilteredEigenValuesOfTheta = filteredEigenValuesOfTheta.Select(x => x / norm).ToArray();
            return objCoords;
        }
    }
}
