﻿using System;

namespace CellsToServersApp.ArrayPartition
{
    public class IndexTransformator
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

        public int[] aggregateProjectedIndicesArrays(int spaceDimension, int histogramResolution, int cellNO,
            int lowerBoundForSplitDim, int upperBoundForSplitDim, int splitDimIdx, int[] indicesArray,
            Array heftArray)
        {
            int cellsInProjectedRegion = (upperBoundForSplitDim - lowerBoundForSplitDim + 1);
            int[] projectionArray = new int[cellsInProjectedRegion];
            for (int movingIdx = lowerBoundForSplitDim; movingIdx <= upperBoundForSplitDim; movingIdx++)
            {
                int[] bandOfIndicesArray = new int[2 * spaceDimension];
                for (int idx = 0; idx < spaceDimension; idx++)
                {
                    if (idx == splitDimIdx)
                    {
                        bandOfIndicesArray[2 * idx] = movingIdx;
                        bandOfIndicesArray[2 * idx + 1] = movingIdx;
                    }
                    else
                    {
                        bandOfIndicesArray[2 * idx] = indicesArray[2 * idx];
                        bandOfIndicesArray[2 * idx + 1] = indicesArray[2 * idx + 1];
                    }
                }
                projectionArray[movingIdx - lowerBoundForSplitDim] = (int)heftArray.GetValue(bandOfIndicesArray);
            }
            return projectionArray;
        }
    }
}
