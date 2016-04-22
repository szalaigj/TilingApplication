using HierarchicalTilingApp.SumOfSquares;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.Transformation
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

        public int[] mergeIndicesArrays(int spaceDimension, int splitNO, int[] outerIndicesArray, 
            int[] innerIndicesArray)
        {
            int[] mergedArrayIndices = new int[2 * spaceDimension + 1];
            mergedArrayIndices[0] = splitNO;
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                mergedArrayIndices[2 * idx + 1] = outerIndicesArray[idx];
                mergedArrayIndices[2 * idx + 2] = innerIndicesArray[idx];
            }
            return mergedArrayIndices;
        }
        
        public bool validateRegionHasEnoughBins(int spaceDimension, int[] indicesArray, int splitNO)
        {
            bool hasEnoughBins = true;
            int binNOInThisRegion = 1;
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                int lowerBound = indicesArray[2 * idx];
                int upperBound = indicesArray[2 * idx + 1];
                binNOInThisRegion *= (upperBound - lowerBound + 1);
            }
            if (binNOInThisRegion <= splitNO)
            {
                hasEnoughBins = false;
            }
            return hasEnoughBins;
        }

        public void splitIndicesArrays(int spaceDimension, int splitDimIdx, int[] indicesArray,
            int componentInSplitDim, out int[] firstPartIndicesArray, out int[] secondPartIndicesArray)
        {
            firstPartIndicesArray = new int[2 * spaceDimension];
            secondPartIndicesArray = new int[2 * spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                int lowerBound = indicesArray[2 * idx];
                int upperBound = indicesArray[2 * idx + 1];
                firstPartIndicesArray[2 * idx] = lowerBound;
                secondPartIndicesArray[2 * idx + 1] = upperBound;
                if (idx == splitDimIdx)
                {
                    firstPartIndicesArray[2 * idx + 1] = componentInSplitDim;
                    secondPartIndicesArray[2 * idx] = componentInSplitDim + 1;
                }
                else
                {
                    firstPartIndicesArray[2 * idx + 1] = upperBound;
                    secondPartIndicesArray[2 * idx] = lowerBound;
                }
            }
        }
               
        public void initializeObjectiveValueArray(double initializationValue, Array objectiveValueArray)
        {
            for (int[] movingIndicesArray = determineFirstIndicesArray(objectiveValueArray); 
                    movingIndicesArray != null;
                    movingIndicesArray = determineNextIndicesArray(objectiveValueArray, movingIndicesArray))
            {
                objectiveValueArray.SetValue(initializationValue, movingIndicesArray);
            }
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

        public int[] determineIndicesArray(int spaceDimension, int[] extendedIndicesArray)
        {
            int[] indicesArray = new int[2 * spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                indicesArray[2 * idx] = extendedIndicesArray[2 * idx + 1];
                indicesArray[2 * idx + 1] = extendedIndicesArray[2 * idx + 2];
            }
            return indicesArray;
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
                    if(intTuple.determineIdxArrayRelativeTo(histogramResolution, inputIndicesArray, 
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

        public int[] determineFirstIndicesArray(int[] indicesArrayOfRegion)
        {
            int spaceDimension = indicesArrayOfRegion.Length/2;
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
