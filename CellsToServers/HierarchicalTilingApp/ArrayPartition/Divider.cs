using System;

namespace HierarchicalTilingApp.ArrayPartition
{
    public class Divider
    {
        private Array heftArray;
        private Array boundArray;
        private Array partitionArray;
        private Array maxDiffArray;
        private Transformator transformator;
        private int spaceDimension;
        private int histogramResolution;
        private int serverNO;
        private double delta;
        private int strategyCode;
        private int slidingWindowSize;
        private double diffSum;
        private int initializationValue;

        public Divider(Array heftArray, Transformator transformator, int spaceDimension, int histogramResolution, 
            int serverNO, double delta, int strategyCode, int slidingWindowSize)
        {
            this.heftArray = heftArray;
            this.transformator = transformator;
            this.spaceDimension = spaceDimension;
            this.histogramResolution = histogramResolution;
            this.serverNO = serverNO;
            this.delta = delta;
            this.strategyCode = strategyCode;
            this.slidingWindowSize = slidingWindowSize;
            this.initializationValue = -1;
            int[] lengthsMaxDiffArray = new int[2 * spaceDimension];
            int[] lengthsBoundArray = new int[2 * spaceDimension + 1];
            lengthsBoundArray[0] = serverNO;
            for (int idx = 1; idx <= 2 * spaceDimension; idx++)
            {
                lengthsBoundArray[idx] = histogramResolution;
                lengthsMaxDiffArray[idx - 1] = histogramResolution;
            }
            this.boundArray = Array.CreateInstance(typeof(int), lengthsBoundArray);
            transformator.initializeBoundArray(this.spaceDimension, this.histogramResolution, this.serverNO, 
                this.initializationValue, this.boundArray);
            this.partitionArray = Array.CreateInstance(typeof(Coords[]), lengthsBoundArray);
            this.maxDiffArray = Array.CreateInstance(typeof(double), lengthsMaxDiffArray);
        }

        public double getDiffSum()
        {
            return diffSum;
        }

        public int determineNeededBound(out Coords[] partition)
        {
            int[] extendedIndicesArray = new int[2 * spaceDimension + 1];
            extendedIndicesArray[0] = serverNO - 1;
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                extendedIndicesArray[2 * idx + 1] = 0;
                extendedIndicesArray[2 * idx + 2] = histogramResolution - 1;
            }
            return innerDetermineNeededBound(extendedIndicesArray, out partition);
        }

        private int innerDetermineNeededBound(int[] extendedIndicesArray, out Coords[] partition)
        {
            int neededBound;
            int currentBound = (int)boundArray.GetValue(extendedIndicesArray);
            if (currentBound != initializationValue)
            {
                neededBound = currentBound;
                partition = (Coords[])partitionArray.GetValue(extendedIndicesArray);
            }
            else
            {
                neededBound = fillBoundArray(extendedIndicesArray, out partition);
            }
            return neededBound;
        }

        private int fillBoundArray(int[] extendedIndicesArray, out Coords[] partition)
        {
            int neededBound;
            int[] indicesArray = transformator.determineIndicesArray(spaceDimension, extendedIndicesArray);
            int splitNO = extendedIndicesArray[0];
            if (splitNO == 0)
            {
                int heftOfRegion = (int)heftArray.GetValue(indicesArray);
                Coords coords = new Coords
                {
                    IndicesArray = extendedIndicesArray,
                    HeftOfRegion = heftOfRegion
                };
                partition = new Coords[] {coords};
                partitionArray.SetValue(partition, extendedIndicesArray);
                neededBound = heftOfRegion;
            }
            else
            {
                neededBound = fillBoundWhenSplitNOIsLargerThenZero(extendedIndicesArray, indicesArray, out partition);
            }
            boundArray.SetValue(neededBound, extendedIndicesArray);
            return neededBound;
        }

        private int fillBoundWhenSplitNOIsLargerThenZero(int[] extendedIndicesArray, int[] indicesArray, 
            out Coords[] partition)
        {
            int neededBound = int.MaxValue;
            int splitNO = extendedIndicesArray[0];
            partition = (Coords[])partitionArray.GetValue(extendedIndicesArray);
            int cellNO = (int)Math.Pow(histogramResolution, spaceDimension);
            int[] movingIndicesArray = new int[spaceDimension];
            for (int movingIdx = 0; movingIdx < cellNO; movingIdx++)
            {
                transformator.transformCellIdxToIndicesArray(histogramResolution, movingIndicesArray, movingIdx);
                for (int splitDimIdx = 0; splitDimIdx < spaceDimension; splitDimIdx++)
                {
                    bool validMovingIndicesArray = transformator.validateIndicesArrays(spaceDimension, splitDimIdx,
                        indicesArray, movingIndicesArray);
                    if (validMovingIndicesArray)
                    {
                        fillBoundWhenMovingIdxArrayIsValid(extendedIndicesArray, indicesArray, ref partition, 
                            ref neededBound, splitNO, movingIndicesArray, splitDimIdx);
                    }
                }
            }
            return neededBound;
        }

        private void fillBoundWhenMovingIdxArrayIsValid(int[] extendedIndicesArray, int[] indicesArray, 
            ref Coords[] partition, ref int neededBound, int splitNO, int[] movingIndicesArray, int splitDimIdx)
        {
            int[] firstPartIndicesArray, secondPartIndicesArray;
            int[] firstPartExtendedIndicesArray = new int[2 * spaceDimension + 1];
            int[] secondPartExtendedIndicesArray = new int[2 * spaceDimension + 1];
            Coords[] firstPartPartition, secondPartPartition;
            transformator.splitIndicesArrays(spaceDimension, splitDimIdx, indicesArray,
                movingIndicesArray, out firstPartIndicesArray, out secondPartIndicesArray);
            for (int firstSplitNO = 0; firstSplitNO <= splitNO - 1; firstSplitNO++)
            {
                int secondSplitNO = (splitNO - 1) - firstSplitNO;
                firstPartIndicesArray.CopyTo(firstPartExtendedIndicesArray, 1);
                firstPartExtendedIndicesArray[0] = firstSplitNO;
                secondPartIndicesArray.CopyTo(secondPartExtendedIndicesArray, 1);
                secondPartExtendedIndicesArray[0] = secondSplitNO;
                int neededBoundForFirstPart = innerDetermineNeededBound(firstPartExtendedIndicesArray,
                    out firstPartPartition);
                int neededBoundForSecondPart = innerDetermineNeededBound(secondPartExtendedIndicesArray,
                    out secondPartPartition);
                int currentBound = Math.Max(neededBoundForFirstPart, neededBoundForSecondPart);

                useChosenStrategy(extendedIndicesArray, indicesArray, ref partition, ref neededBound,
                    movingIndicesArray, splitDimIdx, firstPartIndicesArray, secondPartIndicesArray,
                    firstPartPartition, secondPartPartition, currentBound, splitNO);
            }
        }

        private void useChosenStrategy(int[] extendedIndicesArray, int[] indicesArray, ref Coords[] partition, 
            ref int neededBound, int[] movingIndicesArray, int splitDimIdx, int[] firstPartIndicesArray, 
            int[] secondPartIndicesArray, Coords[] firstPartPartition, Coords[] secondPartPartition,
            int currentBound, int splitNO)
        {
            if (strategyCode == 0)
            {
                // Optimized for clustering strategy:
                applySideDifferenceStrategy(extendedIndicesArray, indicesArray, ref partition, ref neededBound,
                    firstPartPartition, secondPartPartition, currentBound, splitNO, firstPartIndicesArray,
                    secondPartIndicesArray, splitDimIdx, movingIndicesArray);
            }
            else
            {
                //Optimized for load balancing strategy:
                applyMinDiffSumStrategy(extendedIndicesArray, ref partition, ref neededBound, firstPartPartition, 
                    secondPartPartition, currentBound, splitNO);
            }
        }

        private void applySideDifferenceStrategy(int[] extendedIndicesArray, int[] indicesArray, ref Coords[] partition,
            ref int neededBound, Coords[] firstPartPartition, Coords[] secondPartPartition, int currentBound, int splitNO,
            int[] firstPartIndicesArray, int[] secondPartIndicesArray, int splitDimIdx, int[] movingIndicesArray)
        {
            if (currentBound < neededBound)
            {
                neededBound = currentBound;
                partition = new Coords[splitNO + 1];
                setPartitionByParts(extendedIndicesArray, partition, firstPartPartition, secondPartPartition);
            }
            else if (currentBound == neededBound)
            {
                double currentDiff = determineCurrentDiffOfSides(splitDimIdx, indicesArray, movingIndicesArray);
                double maxDiff = (double)maxDiffArray.GetValue(indicesArray);
                if ((maxDiff < currentDiff) && (splitNO == serverNO - 1))
                {
                    maxDiffArray.SetValue(currentDiff, indicesArray);
                    setPartitionByParts(extendedIndicesArray, partition, firstPartPartition, secondPartPartition);
                }
            }
        }

        private double determineCurrentDiffOfSides(int splitDimIdx, int[] indicesArray, int[] movingIndicesArray)
        {
            int[] firstSideOfSplit, secondSideOfSplit;
            transformator.determineSidesOfSplit(spaceDimension, splitDimIdx, slidingWindowSize, histogramResolution,
                indicesArray, movingIndicesArray, out firstSideOfSplit, out secondSideOfSplit);
            int heftFirstSideOfSplit = (int)heftArray.GetValue(firstSideOfSplit);
            int heftSecondSideOfSplit = (int)heftArray.GetValue(secondSideOfSplit);
            return Math.Abs(heftFirstSideOfSplit - heftSecondSideOfSplit);
        }
        
        private void applyMinDiffSumStrategy(int[] extendedIndicesArray, ref Coords[] partition, ref int neededBound,
            Coords[] firstPartPartition, Coords[] secondPartPartition, int currentBound, int splitNO)
        {
            if (currentBound < neededBound)
            {
                neededBound = currentBound;
                partition = new Coords[splitNO + 1];
                setPartitionByParts(extendedIndicesArray, partition, firstPartPartition, secondPartPartition);
                if (splitNO == serverNO - 1)
                {
                    diffSum = determineCurrentDiffSum(neededBound, firstPartPartition, secondPartPartition);
                }
            }
            else if ((currentBound == neededBound) && (splitNO == serverNO - 1))
            {
                double currentDiffSum = determineCurrentDiffSum(neededBound, firstPartPartition, secondPartPartition);
                if (currentDiffSum < diffSum)
                {
                    diffSum = currentDiffSum;
                    setPartitionByParts(extendedIndicesArray, partition, firstPartPartition, secondPartPartition);
                }
            }
        }

        private double determineCurrentDiffSum(int neededTileNumber, Coords[] firstPartPartition, 
            Coords[] secondPartPartition)
        {
            double currentDiffSum = 0.0;
            for (int idx = 0; idx < firstPartPartition.Length; idx++)
            {
                currentDiffSum += firstPartPartition[idx].differenceFromDelta(delta);
            }
            for (int idx = 0; idx < secondPartPartition.Length; idx++)
            {
                currentDiffSum += secondPartPartition[idx].differenceFromDelta(delta);
            }
            return currentDiffSum;
        }

        private void setPartitionByParts(int[] extendedIndicesArray, Coords[] partition,
            Coords[] firstPartPartition, Coords[] secondPartPartition)
        {
            firstPartPartition.CopyTo(partition, 0);
            secondPartPartition.CopyTo(partition, firstPartPartition.Length);
            partitionArray.SetValue(partition, extendedIndicesArray);
        }
    }
}
