using System;

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
            int[] innerIndicesArray, out int[] heftArrayIndices)
        {
            bool validHeftArrayIndices = true;
            heftArrayIndices = new int[2 * spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                int outerIdx = outerIndicesArray[idx];
                int innerIdx = innerIndicesArray[idx];
                if (outerIdx <= innerIdx)
                {
                    heftArrayIndices[2 * idx] = outerIdx;
                    heftArrayIndices[2 * idx + 1] = innerIdx;
                }
                else
                {
                    validHeftArrayIndices = false;
                    break;
                }
            }
            return validHeftArrayIndices;
        }

        public bool validateIndicesArrays(int spaceDimension, int[] outerIndicesArray,
            int[] innerIndicesArray, int[] windowIndicesArray)
        {
            bool validHeftArrayIndices = true;
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                int outerIdx = outerIndicesArray[idx];
                int innerIdx = innerIndicesArray[idx];
                int windowIdx = windowIndicesArray[idx];
                if (!((outerIdx <= windowIdx) && (windowIdx <= innerIdx)))
                {
                    validHeftArrayIndices = false;
                    break;
                }
            }
            return validHeftArrayIndices;
        }

        public bool validateIndicesArrays(int spaceDimension, int splitDimIdx, int[] indicesArray,
            int[] windowIndicesArray)
        {
            bool validWindowIndicesArray = true;
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                int lowerBound = indicesArray[2 * idx];
                int upperBound = indicesArray[2 * idx + 1];
                if (idx == splitDimIdx)
                {
                    int windowIdx = windowIndicesArray[idx];
                    if (!((lowerBound <= windowIdx) && (windowIdx < upperBound)))
                    {
                        validWindowIndicesArray = false;
                        break;
                    }
                }
            }
            return validWindowIndicesArray;
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
    }
}
