using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.ArrayPartition
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

        public void determineSidesOfSplit(int spaceDimension, int splitDimIdx, int slidingWindowSize,
            int histogramResolution, int[] indicesArray, int[] movingIndicesArray,
            out int[] firstSideOfSplit, out int[] secondSideOfSplit)
        {
            firstSideOfSplit = new int[2 * spaceDimension];
            secondSideOfSplit = new int[2 * spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                if (idx == splitDimIdx)
                {
                    int movingIdx = movingIndicesArray[idx];
                    //firstSideOfSplit[2 * idx] = firstSideOfSplit[2 * idx + 1] = movingIdx;
                    //secondSideOfSplit[2 * idx] = secondSideOfSplit[2 * idx + 1] = movingIdx + 1;
                    firstSideOfSplit[2 * idx] = (movingIdx - slidingWindowSize + 1 >= 0) ?
                        movingIdx - slidingWindowSize + 1 : 0;
                    firstSideOfSplit[2 * idx + 1] = movingIdx;
                    secondSideOfSplit[2 * idx] = movingIdx + 1;
                    secondSideOfSplit[2 * idx + 1] = (movingIdx + slidingWindowSize < histogramResolution) ?
                        movingIdx + slidingWindowSize : histogramResolution - 1;
                }
                else
                {
                    int lowerBound = indicesArray[2 * idx];
                    int upperBound = indicesArray[2 * idx + 1];
                    firstSideOfSplit[2 * idx] = secondSideOfSplit[2 * idx] = lowerBound;
                    firstSideOfSplit[2 * idx + 1] = secondSideOfSplit[2 * idx + 1] = upperBound;

                }
            }
        }
               
        public void initializeBoundArray(int spaceDimension, int histogramResolution, int serverNO,
            int initializationValue, Array boundArray)
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
                        boundArray.SetValue(initializationValue, movingIndicesArray);
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
    }
}
