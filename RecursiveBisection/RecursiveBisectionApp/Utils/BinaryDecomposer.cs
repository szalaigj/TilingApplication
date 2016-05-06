using System;
using System.Collections.Generic;

namespace RecursiveBisectionApp.Utils
{
    public class BinaryDecomposer
    {
        private Array array;
        private Array heftArray;
        private Transformator transformator;
        private int spaceDimension;
        private int histogramResolution;
        private int serverNO;
        private int pointNO;

        public BinaryDecomposer(Array array, Array heftArray, Transformator transformator, int spaceDimension, 
            int histogramResolution, int serverNO, int pointNO)
        {
            this.array = array;
            this.heftArray = heftArray;
            this.transformator = transformator;
            this.spaceDimension = spaceDimension;
            this.histogramResolution = histogramResolution;
            this.serverNO = serverNO;
            this.pointNO = pointNO;
        }

        public Coords[] decompose()
        {
            int[] indicesArray = determineIndicesArray();
            int remainderServerNO = serverNO;
            int splitLevel = 0;
            Coords[] partition = innerDecompose(indicesArray, remainderServerNO, splitLevel);
            return partition;
        }

        private int[] determineIndicesArray()
        {
            int[] indicesArray = new int[2 * spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                indicesArray[2 * idx] = 0;
                indicesArray[2 * idx + 1] = histogramResolution - 1;
            }
            return indicesArray;
        }

        private Coords[] innerDecompose(int[] indicesArray, int remainderServerNO, int splitLevel)
        {
            Coords[] partition;
            if (remainderServerNO == 1)
            {
                partition = determinePartitionWhenRemainderServerNOIsZero(indicesArray);
            }
            else
            {
                partition = determinePartitionWhenRemainderServerNOIsLargerThanZero(indicesArray, remainderServerNO,
                    splitLevel);
            }
            return partition;
        }

        private Coords[] determinePartitionWhenRemainderServerNOIsZero(int[] indicesArray)
        {
            Coords[] partition;
            int heftOfRegion = (int)heftArray.GetValue(indicesArray);
            Coords coords = new Coords
            {
                IndicesArray = indicesArray,
                HeftOfRegion = heftOfRegion
            };
            partition = new Coords[] { coords };
            return partition;
        }

        private Coords[] determinePartitionWhenRemainderServerNOIsLargerThanZero(int[] indicesArray, 
            int remainderServerNO, int splitLevel)
        {
            // alternate dimension index based on split level
            int splitDimIdx = splitLevel % spaceDimension;
            int minHeftDiffBetweenParts = int.MaxValue;
            int[] minFirstPartIndicesArray = new int[2 * spaceDimension];
            int[] minSecondPartIndicesArray = new int[2 * spaceDimension];
            determineMinHeftDiff(indicesArray, splitDimIdx, ref minHeftDiffBetweenParts, ref minFirstPartIndicesArray, 
                ref minSecondPartIndicesArray);
            Coords[] firstPartPartition = innerDecompose(minFirstPartIndicesArray, remainderServerNO / 2, 
                splitLevel + 1);
            Coords[] secondPartPartition = innerDecompose(minSecondPartIndicesArray, remainderServerNO / 2, 
                splitLevel + 1);
            Coords[] partition = new Coords[remainderServerNO];
            firstPartPartition.CopyTo(partition, 0);
            secondPartPartition.CopyTo(partition, firstPartPartition.Length);
            return partition;
        }

        private void determineMinHeftDiff(int[] indicesArray, int splitDimIdx, ref int minHeftDiffBetweenParts, ref int[] minFirstPartIndicesArray, ref int[] minSecondPartIndicesArray)
        {
            for (int componentInSplitDim = indicesArray[2 * splitDimIdx];
                componentInSplitDim < indicesArray[2 * splitDimIdx + 1]; componentInSplitDim++)
            {
                int[] firstPartIndicesArray, secondPartIndicesArray;
                transformator.splitIndicesArrays(spaceDimension, splitDimIdx, indicesArray,
                componentInSplitDim, out firstPartIndicesArray, out secondPartIndicesArray);
                int firstPartHeftOfRegion = (int)heftArray.GetValue(firstPartIndicesArray);
                int secondPartHeftOfRegion = (int)heftArray.GetValue(secondPartIndicesArray);
                int heftDiffBetweenParts = Math.Abs(firstPartHeftOfRegion - secondPartHeftOfRegion);
                if (heftDiffBetweenParts < minHeftDiffBetweenParts)
                {
                    minHeftDiffBetweenParts = heftDiffBetweenParts;
                    minFirstPartIndicesArray = firstPartIndicesArray;
                    minSecondPartIndicesArray = secondPartIndicesArray;
                }
            }
        }
    }
}
