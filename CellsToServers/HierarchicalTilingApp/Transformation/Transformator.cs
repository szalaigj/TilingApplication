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

        public bool mergeIndicesArrays(int spaceDimension, int[] outerIndicesArray,
            int[] innerIndicesArray, out int[] mergedArrayIndices, out int cells)
        {
            bool validArrayIndices = true;
            cells = 1;
            mergedArrayIndices = new int[2 * spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                int outerIdx = outerIndicesArray[idx];
                int innerIdx = innerIndicesArray[idx];
                if (outerIdx <= innerIdx)
                {
                    mergedArrayIndices[2 * idx] = outerIdx;
                    mergedArrayIndices[2 * idx + 1] = innerIdx;
                    cells *= (innerIdx - outerIdx + 1);
                }
                else
                {
                    validArrayIndices = false;
                    break;
                }
            }
            return validArrayIndices;
        }

        public bool validateIndicesArrays(int spaceDimension, int[] outerIndicesArray,
            int[] innerIndicesArray, int[] windowIndicesArray)
        {
            bool validArrayIndices = true;
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                int outerIdx = outerIndicesArray[idx];
                int innerIdx = innerIndicesArray[idx];
                int windowIdx = windowIndicesArray[idx];
                if (!((outerIdx <= windowIdx) && (windowIdx <= innerIdx)))
                {
                    validArrayIndices = false;
                    break;
                }
            }
            return validArrayIndices;
        }

        public bool validateIndicesArrays(int spaceDimension, int[] indicesArrayOfRegion, int[] indicesArrayOfBin)
        {
            int[] lowerIndicesArray = new int[spaceDimension];
            int[] upperIndicesArray = new int[spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                lowerIndicesArray[idx] = indicesArrayOfRegion[2 * idx];
                upperIndicesArray[idx] = indicesArrayOfRegion[2 * idx + 1];
            }
            return validateIndicesArrays(spaceDimension, lowerIndicesArray, upperIndicesArray, indicesArrayOfBin);
        }

        public bool validateIndicesArrays(int spaceDimension, int splitDimIdx, int[] indicesArray,
            int[] movingIndicesArray)
        {
            bool validMovingIndicesArray = true;
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                int lowerBound = indicesArray[2 * idx];
                int upperBound = indicesArray[2 * idx + 1];
                if (idx == splitDimIdx)
                {
                    int movingIdx = movingIndicesArray[idx];
                    if (!((lowerBound <= movingIdx) && (movingIdx < upperBound)))
                    {
                        validMovingIndicesArray = false;
                        break;
                    }
                }
            }
            return validMovingIndicesArray;
        }

        public void splitIndicesArrays(int spaceDimension, int splitDimIdx, int[] indicesArray,
            int[] movingIndicesArray, out int[] firstPartIndicesArray, out int[] secondPartIndicesArray)
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
                    int movingIdx = movingIndicesArray[idx];
                    firstPartIndicesArray[2 * idx + 1] = movingIdx;
                    secondPartIndicesArray[2 * idx] = movingIdx + 1;
                }
                else
                {
                    firstPartIndicesArray[2 * idx + 1] = upperBound;
                    secondPartIndicesArray[2 * idx] = lowerBound;
                }
            }
        }
               
        public void initializeObjectiveValueArray(int spaceDimension, int histogramResolution, int serverNO,
            double initializationValue, Array objectiveValueArray)
        {
            int movingIdxLimit = (int)Math.Pow(histogramResolution, spaceDimension);
            int[] movingIndicesArray = new int[2 * spaceDimension + 1];
            int[] tempOuterIndicesArray = new int[spaceDimension];
            int[] tempInnerIndicesArray = new int[spaceDimension];
            for (int splitNOIdx = 0; splitNOIdx < serverNO; splitNOIdx++)
            {
                movingIndicesArray[0] = splitNOIdx;
                for (int movingIdx = 0; movingIdx < movingIdxLimit; movingIdx++)
                {
                    transformCellIdxToIndicesArray(histogramResolution, tempOuterIndicesArray, movingIdx);
                    for (int subMovingIdx = 0; subMovingIdx < movingIdxLimit; subMovingIdx++)
                    {
                        transformCellIdxToIndicesArray(histogramResolution, tempInnerIndicesArray, subMovingIdx);
                        for (int idx = 0; idx < spaceDimension; idx++)
                        {
                            movingIndicesArray[2 * idx + 1] = tempOuterIndicesArray[idx];
                            movingIndicesArray[2 * idx + 2] = tempInnerIndicesArray[idx];
                        }
                        objectiveValueArray.SetValue(initializationValue, movingIndicesArray);
                    }
                }
            }
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

        public Dictionary<int, List<int[]>> determineShellIdxArraysInTwoDimSpace(int histogramResolution, 
            int[] inputIndicesArray, int maxShellNO)
        {
            Shell[] shells = shellBuilder.createShellsInTwoDimSpace(maxShellNO);
            return convertIntPairsOfShellsToListOfIdxArrays(histogramResolution, inputIndicesArray, shells);
        }

        private Dictionary<int, List<int[]>> convertIntPairsOfShellsToListOfIdxArrays(int histogramResolution, 
            int[] inputIndicesArray, Shell[] shells)
        {
            Dictionary<int, List<int[]>> result = new Dictionary<int, List<int[]>>();
            int shellIdx = 0;
            foreach (var shell in shells)
            {
                List<int[]> indicesArraysInCurrentShell = new List<int[]>();
                IntPair[] intPairs = shell.getIntPairs();
                foreach (var intPair in intPairs)
                {
                    int[] currentIndicesArray;
                    if(intPair.determineIdxArrayRelativeTo(histogramResolution, inputIndicesArray, 
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
    }
}
