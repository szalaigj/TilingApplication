using System;
using System.Collections.Generic;

namespace RecursiveBisectionApp.Utils
{
    public class Transformator
    {
        public void transformCellIdxToIndicesArray(int histogramResolution, int[] indicesArray, int cellIdx)
        {
            indicesArray[0] =
                cellIdx / (int)Math.Pow(histogramResolution, indicesArray.Length - 1);
            for (int coordIdx = 1; coordIdx < indicesArray.Length; coordIdx++)
            {
                indicesArray[coordIdx] = cellIdx;
                for (int subCoordIdx = coordIdx - 1; subCoordIdx >= 0; subCoordIdx--)
                {
                    indicesArray[coordIdx] -=
                        (int)Math.Pow(histogramResolution, indicesArray.Length - (subCoordIdx + 1))
                        * indicesArray[subCoordIdx];
                }
                indicesArray[coordIdx] =
                    indicesArray[coordIdx] /
                    (int)Math.Pow(histogramResolution, indicesArray.Length - (coordIdx + 1));
            }
        }

        public int[] mergeIndicesArrays(int spaceDimension, int[] outerIndicesArray, int[] innerIndicesArray)
        {
            int[] mergedArrayIndices = new int[2 * spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                mergedArrayIndices[2 * idx] = outerIndicesArray[idx];
                mergedArrayIndices[2 * idx + 1] = innerIndicesArray[idx];
            }
            return mergedArrayIndices;
        }

        /// <summary>
        /// The following implementation is based on 
        /// stackoverflow.com/questions/9914230/iterate-through-an-array-of-arbitrary-dimension/9914326#9914326
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public int[] determineFirstIndicesArray(Array array)
        {
            int spaceDimension = array.Rank;
            int[] firstIndicesArray = new int[spaceDimension];
            for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
            {
                firstIndicesArray[dimIdx] = array.GetLowerBound(dimIdx);
            }
            return firstIndicesArray;
        }

        /// <summary>
        /// The following implementation is based on 
        /// stackoverflow.com/questions/9914230/iterate-through-an-array-of-arbitrary-dimension/9914326#9914326
        /// </summary>
        /// <param name="array"></param>
        /// <param name="previousIndicesArray"></param>
        /// <returns></returns>
        public int[] determineNextIndicesArray(Array array, int[] previousIndicesArray)
        {
            int spaceDimension = array.Rank;
            int[] nextIndicesArray = new int[spaceDimension];
            previousIndicesArray.CopyTo(nextIndicesArray, 0);
            for (int dimIdx = spaceDimension - 1; dimIdx >= 0; --dimIdx)
            {
                nextIndicesArray[dimIdx]++;
                if (nextIndicesArray[dimIdx] <= array.GetUpperBound(dimIdx))
                    return nextIndicesArray;
                nextIndicesArray[dimIdx] = array.GetLowerBound(dimIdx);
            }
            return null;
        }

        public int[] determineFirstIndicesArray(int[] indicesArrayOfRegion)
        {
            int spaceDimension = indicesArrayOfRegion.Length / 2;
            int[] firstIndicesArray = new int[spaceDimension];
            for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
            {
                firstIndicesArray[dimIdx] = indicesArrayOfRegion[2 * dimIdx];
            }
            return firstIndicesArray;
        }

        public int[] determineNextIndicesArray(int[] indicesArrayOfRegion, int[] previousIndicesArray)
        {
            int spaceDimension = indicesArrayOfRegion.Length / 2;
            int[] nextIndicesArray = new int[spaceDimension];
            previousIndicesArray.CopyTo(nextIndicesArray, 0);
            for (int dimIdx = spaceDimension - 1; dimIdx >= 0; --dimIdx)
            {
                nextIndicesArray[dimIdx]++;
                int lowerBoundForCurrentDim = indicesArrayOfRegion[2 * dimIdx];
                int upperBoundForCurrentDim = indicesArrayOfRegion[2 * dimIdx + 1];
                if (nextIndicesArray[dimIdx] <= upperBoundForCurrentDim)
                    return nextIndicesArray;
                nextIndicesArray[dimIdx] = lowerBoundForCurrentDim;
            }
            return null;
        }
    }
}
