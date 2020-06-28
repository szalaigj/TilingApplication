﻿using MeasureApp.SumOfSquares;
using System;
using System.Collections.Generic;

namespace MeasureApp.Transformation
{
    public class Transformator
    {
        private ShellBuilder shellBuilder;

        public Transformator(ShellBuilder shellBuilder)
        {
            this.shellBuilder = shellBuilder;
        }

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

        public int transformIndicesArrayToCellIdx(int histogramResolution, int[] indicesArray)
        {
            int cellIdx = 0;
            for (int coordIdx = 0; coordIdx < indicesArray.Length; coordIdx++)
            {
                cellIdx += (int)Math.Pow(histogramResolution, indicesArray.Length - (coordIdx + 1))
                    * indicesArray[coordIdx];
            }
            return cellIdx;
        }

        public Dictionary<int, List<int[]>> convertIntPairsOfShellsToListOfIdxArrays(int histogramResolution,
            int[] inputIndicesArray, Shell[] shells)
        {
            Dictionary<int, List<int[]>> result = new Dictionary<int, List<int[]>>();
            int shellIdx = 0;
            foreach (var shell in shells)
            {
                List<int[]> indicesArraysInCurrentShell = new List<int[]>();
                IntTuple[] intTuples = shell.getIntTuples();
                foreach (var intTuple in intTuples)
                {
                    int[] currentIndicesArray;
                    if (intTuple.determineIdxArrayRelativeTo(histogramResolution, inputIndicesArray,
                        out currentIndicesArray))
                    {
                        indicesArraysInCurrentShell.Add(currentIndicesArray);
                    }
                }
                result.Add(shellIdx, indicesArraysInCurrentShell);
                shellIdx++;
            }
            return result;
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

        public int[] determineFirstContainedIndicesArray(int[] indicesArrayOfRegion)
        {
            int spaceDimension = indicesArrayOfRegion.Length / 2;
            int[] firstIndicesArray = new int[spaceDimension];
            for (int dimIdx = 0; dimIdx < spaceDimension; dimIdx++)
            {
                firstIndicesArray[dimIdx] = indicesArrayOfRegion[2 * dimIdx];
            }
            return firstIndicesArray;
        }

        public int[] determineNextContainedIndicesArray(int[] indicesArrayOfRegion, int[] previousIndicesArray)
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

        public int[] determineNextIndicesArray(Array array, int[] lowerBoundArray, int[] previousIndicesArray)
        {
            int spaceDimension = array.Rank;
            int[] nextIndicesArray = new int[spaceDimension];
            previousIndicesArray.CopyTo(nextIndicesArray, 0);
            for (int dimIdx = spaceDimension - 1; dimIdx >= 0; --dimIdx)
            {
                nextIndicesArray[dimIdx]++;
                if (nextIndicesArray[dimIdx] <= array.GetUpperBound(dimIdx))
                    return nextIndicesArray;
                nextIndicesArray[dimIdx] = lowerBoundArray[dimIdx];
            }
            return null;
        }

        /// <summary>
        /// The following method is based on the space diagonal of the hypercube.
        /// The maximum distance between bins is the space diagonal in the histogram. 
        /// We can approximate the maximal shell number based on the proper part volume
        /// of the (spaceDimension)-dimensional spherical sector because the volume of 
        /// this region is proportional to the contained bins by it.
        /// </summary>
        /// <param name="spaceDimension"></param>
        /// <param name="histogramResolution"></param>
        /// <returns></returns>
        public int determineMaxRange(int spaceDimension, int histogramResolution)
        {
            double temp = Math.Pow(histogramResolution, spaceDimension);
            temp /= (factorial(spaceDimension) * doubleFactorial(spaceDimension));
            temp *= Math.Pow(Math.PI * spaceDimension / 2.0, spaceDimension / 2.0);
            int result = (int)Math.Ceiling(temp);
            return result;
        }

        private int factorial(int input)
        {
            int result = 1;
            for (int idx = 1; idx <= input; idx++)
            {
                result *= idx;
            }
            return result;
        }

        private int doubleFactorial(int input)
        {
            int result;
            if ((input % 2) == 0)
            {
                result = 1;
                for (int idx = 1; idx <= input / 2; idx++)
                {
                    result *= 2 * idx;
                }
            }
            else
            {
                result = 1;
                for (int idx = 1; idx <= (input + 1) / 2; idx++)
                {
                    result *= (2 * idx - 1);
                }
            }
            return result;
        }
    }
}