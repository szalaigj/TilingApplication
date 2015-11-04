using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMedoids
{
    public class EuclideanDistMXComputer
    {
        public Matrix<double> determineEuclideanDistMX(Matrix<double> objCoords)
        {
            int nObj = objCoords.RowCount;
            Matrix<double> distanceMX = Matrix<double>.Build.Dense(nObj, nObj);
            for (int idx = 0; idx < nObj; idx++)
            {
                for (int subIdx = 0; subIdx < nObj; subIdx++)
                {
                    distanceMX[idx, subIdx] = Distance.Euclidean<double>(objCoords.Row(idx), objCoords.Row(subIdx));
                }
            }
            return distanceMX;
        }

        public Matrix<double> determineWeightedEuclideanDistMX(Matrix<double> objCoords, double[] weights)
        {
            int nObj = objCoords.RowCount;
            int dim = objCoords.ColumnCount;
            Matrix<double> distanceMX = Matrix<double>.Build.Dense(nObj, nObj);
            for (int idx = 0; idx < nObj; idx++)
            {
                for (int subIdx = 0; subIdx < nObj; subIdx++)
                {
                    double currentDist = 0.0;
                    for (int idxDim = 0; idxDim < dim; idxDim++)
                    {
                        currentDist += weights[idxDim] * 
                            Math.Pow(objCoords.Row(idx)[idxDim] - objCoords.Row(subIdx)[idxDim], 2);
                    }
                    distanceMX[idx, subIdx] = Math.Sqrt(currentDist);
                }
            }
            return distanceMX;
        }


    }
}
