using System;
using System.Collections.Generic;

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

        public int determineMaxCellValueAndIdx(int spaceDimension, int histogramResolution, Array array,
            int[] indicesArray, out int[] maxCellIndices)
        {
            int cellNO = (int)Math.Pow(histogramResolution, spaceDimension);
            int maxCellValue = 0;
            int[] tempCellIndices = new int[spaceDimension];
            int[] movingIndicesArray = new int[spaceDimension];
            for (int movingIdx = 0; movingIdx < cellNO; movingIdx++)
            {
                transformCellIdxToIndicesArray(histogramResolution, movingIndicesArray, movingIdx);
                bool validMovingIndicesArray = validateIndicesArrays(spaceDimension, indicesArray, movingIndicesArray);
                if (validMovingIndicesArray && (maxCellValue < (int)array.GetValue(movingIndicesArray)))
                {
                    maxCellValue = (int)array.GetValue(movingIndicesArray);
                    movingIndicesArray.CopyTo(tempCellIndices, 0);
                }
            }
            maxCellIndices = tempCellIndices;
            return maxCellValue;
        }

        public int[] determineRoundedCenterOfMassIdx(int spaceDimension, int histogramResolution, Array array, 
            int heftOfRegion, int[] indicesArray)
        {
            int cellNO = (int)Math.Pow(histogramResolution, spaceDimension);
            int[] tempCellIndices = new int[spaceDimension];
            double[] totalCoords = new double[spaceDimension];
            int[] movingIndicesArray = new int[spaceDimension];
            int heftOfCell;
            for (int movingIdx = 0; movingIdx < cellNO; movingIdx++)
            {
                transformCellIdxToIndicesArray(histogramResolution, movingIndicesArray, movingIdx);
                bool validMovingIndicesArray = validateIndicesArrays(spaceDimension, indicesArray, movingIndicesArray);
                if (validMovingIndicesArray)
                {
                    heftOfCell = (int)array.GetValue(movingIndicesArray);
                    for (int idx = 0; idx < spaceDimension; idx++)
                    {
                        totalCoords[idx] += (double)movingIndicesArray[idx] * heftOfCell;
                    }
                }
            }
            int[] centerOfMassIndices = new int[spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                centerOfMassIndices[idx] = (int)Math.Round(totalCoords[idx] / (double)heftOfRegion);
            }
            return centerOfMassIndices;
        }

        private bool validateIndicesArrays(int spaceDimension, int[] indicesArray, int[] movingIndicesArray)
        {
            bool validMovingIndicesArray = true;
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                int lowerBound = indicesArray[2 * idx];
                int upperBound = indicesArray[2 * idx + 1];
                int movingIdx = movingIndicesArray[idx];
                if (!((lowerBound <= movingIdx) && (movingIdx <= upperBound)))
                {
                    validMovingIndicesArray = false;
                    break;
                }
            }
            return validMovingIndicesArray;
        }

        public int determineCrossBorderHeft(int spaceDimension, int histogramResolution, Array array,
            int[] indicesArray, int[] centerOfMassIndices)
        {
            int crossBorderHeft = 0;
            for (int crossBorderIdx = 0; crossBorderIdx < (int)Math.Pow(3, spaceDimension); crossBorderIdx++)
            {
                int[] crossBorderCandidateIndicesArray = determineCrossBorderIndicesArray(spaceDimension, crossBorderIdx,
                    centerOfMassIndices);
                if (isCrossBorder(spaceDimension, histogramResolution, indicesArray, crossBorderCandidateIndicesArray))
                {
                    crossBorderHeft += (int)array.GetValue(crossBorderCandidateIndicesArray);
                }
            }
            return crossBorderHeft;
        }

        private int[] determineCrossBorderIndicesArray(int spaceDimension, int crossBorderIdx, int[] centerOfMassIndices)
        {
            int[] crossBorderIndicesArray = new int[spaceDimension];
            crossBorderIndicesArray[0] = (crossBorderIdx / (int)Math.Pow(3, spaceDimension - 1));
            for (int coordIdx = 1; coordIdx < spaceDimension; coordIdx++)
            {
                crossBorderIndicesArray[coordIdx] = crossBorderIdx;
                for (int subCoordIdx = coordIdx - 1; subCoordIdx >= 0; subCoordIdx--)
                {
                    crossBorderIndicesArray[coordIdx] -=
                        (int)Math.Pow(3, spaceDimension - (subCoordIdx + 1)) * crossBorderIndicesArray[subCoordIdx];
                }
                crossBorderIndicesArray[coordIdx] = 
                    crossBorderIndicesArray[coordIdx] / (int)Math.Pow(3, spaceDimension - (coordIdx + 1));
            }
            for (int coordIdx = 0; coordIdx < spaceDimension; coordIdx++)
            {
                crossBorderIndicesArray[coordIdx] = centerOfMassIndices[coordIdx] + 
                    crossBorderIndicesArray[coordIdx] - 1;
            }
            return crossBorderIndicesArray;
        }

        private bool isCrossBorder(int spaceDimension, int histogramResolution, int[] indicesArray, 
            int[] crossBorderIndicesArray)
        {
            bool isCrossBorder = false;
            bool isValidIndicesArray = true;
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                int lowerBound = indicesArray[2 * idx];
                int upperBound = indicesArray[2 * idx + 1];
                int crossBorderIdx = crossBorderIndicesArray[idx];
                if ((crossBorderIdx < 0) || (crossBorderIdx >= histogramResolution))
                {
                    isValidIndicesArray = false;
                    break;
                }
                if (!((lowerBound <= crossBorderIdx) && (crossBorderIdx <= upperBound)))
                {
                    isCrossBorder = true;
                }
            }
            return (isCrossBorder && isValidIndicesArray);
        }
    }
}
