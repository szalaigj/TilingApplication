using System;

namespace OldCellsToServersApp.ArrayPartition
{
    public class Divider
    {
        private Array array;
        private Array heftArray;
        private Array tileNumberArray;
        private Array partitionArray;
        private Array diffSumArray;
        private Array maxDiffArray;
        private IndexTransformator transformator;
        private int spaceDimension;
        private int histogramResolution;
        private int serverNO;
        private double delta;
        private double usedDelta;
        private int strategyCode;
        private int slidingWindowSize;

        public Divider(Array array, Array heftArray, IndexTransformator transformator, int spaceDimension,
            int histogramResolution, int serverNO, double delta, double usedDelta, int strategyCode, 
            int slidingWindowSize)
        {
            this.array = array;
            this.heftArray = heftArray;
            this.transformator = transformator;
            this.spaceDimension = spaceDimension;
            this.histogramResolution = histogramResolution;
            this.serverNO = serverNO;
            this.delta = delta;
            this.usedDelta = usedDelta;
            this.strategyCode = strategyCode;
            this.slidingWindowSize = slidingWindowSize;
            int[] lengthsTileNumberArray = new int[2 * spaceDimension];
            for (int idx = 0; idx < 2 * spaceDimension; idx++)
            {
                lengthsTileNumberArray[idx] = histogramResolution;
            }
            this.tileNumberArray = Array.CreateInstance(typeof(int), lengthsTileNumberArray);
            this.partitionArray = Array.CreateInstance(typeof(Coords[]), lengthsTileNumberArray);
            this.diffSumArray = Array.CreateInstance(typeof(double), lengthsTileNumberArray);
            this.maxDiffArray = Array.CreateInstance(typeof(double), lengthsTileNumberArray);
        }

        public int determineNeededTileNumber(out Coords[] partition)
        {
            int[] indicesArray = new int[2 * spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                indicesArray[2 * idx] = 0;
                indicesArray[2 * idx + 1] = histogramResolution - 1;
            }
            int neededTileNumber = innerDetermineNeededTileNumber(indicesArray, out partition);
            foreach (var coords in partition)
            {
                int[] coordsIndicesArray = coords.IndicesArray;
                int heftOfRegion = (int)heftArray.GetValue(coordsIndicesArray);
                int[] maxCellIndices;
                int maxCellValue = transformator.determineMaxCellValueAndIdx(spaceDimension, histogramResolution, array,
                    coordsIndicesArray, out maxCellIndices);
                int[] centerOfMassIndices = transformator.determineRoundedCenterOfMassIdx(spaceDimension, histogramResolution,
                    array, heftOfRegion, coordsIndicesArray);
                int crossBorderHeft = transformator.determineCrossBorderHeft(spaceDimension, histogramResolution, array,
                    coordsIndicesArray, centerOfMassIndices);
                coords.MaxCellValue = maxCellValue;
                coords.MaxCellIndices = maxCellIndices;
                coords.CenterOfMassIndices = centerOfMassIndices;
                coords.CrossBorderHeft = crossBorderHeft;
            }
            return neededTileNumber;
        }

        private int innerDetermineNeededTileNumber(int[] indicesArray, out Coords[] partition)
        {
            int neededTileNumber;
            int currentTileNumber = (int)tileNumberArray.GetValue(indicesArray);
            if (currentTileNumber != 0)
            {
                neededTileNumber = currentTileNumber;
                partition = (Coords[])partitionArray.GetValue(indicesArray);
            }
            else
            {
                neededTileNumber = fillTileNumberArray(indicesArray, out partition);
            }
            return neededTileNumber;
        }

        private int fillTileNumberArray(int[] indicesArray, out Coords[] partition)
        {
            int neededTileNumber;
            int heftOfRegion = (int)heftArray.GetValue(indicesArray);
            if (heftOfRegion <= usedDelta)
            {
                neededTileNumber = 1;
                partition = new Coords[neededTileNumber];
                Coords coords = new Coords
                {
                    IndicesArray = indicesArray,
                    HeftOfRegion = heftOfRegion
                };
                partition[0] = coords;
                partitionArray.SetValue(partition, indicesArray);
            }
            else
            {
                neededTileNumber = fillTileNumberWhenRegionHeftIsLargerThenUsedDelta(indicesArray, out partition);
            }
            tileNumberArray.SetValue(neededTileNumber, indicesArray);
            return neededTileNumber;
        }

        private int fillTileNumberWhenRegionHeftIsLargerThenUsedDelta(int[] indicesArray, out Coords[] partition)
        {
            int neededTileNumber;
            neededTileNumber = int.MaxValue;
            partition = (Coords[])partitionArray.GetValue(indicesArray);
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
                        int[] firstPartIndicesArray, secondPartIndicesArray;
                        Coords[] firstPartPartition, secondPartPartition;
                        transformator.splitIndicesArrays(spaceDimension, splitDimIdx, indicesArray,
                            movingIndicesArray, out firstPartIndicesArray, out secondPartIndicesArray);
                        int neededTileNumberForFirstPart = innerDetermineNeededTileNumber(firstPartIndicesArray,
                            out firstPartPartition);
                        int neededTileNumberForSecondPart = innerDetermineNeededTileNumber(secondPartIndicesArray,
                            out secondPartPartition);
                        int currentTileNO = neededTileNumberForFirstPart + neededTileNumberForSecondPart;

                        useChosenStrategy(indicesArray, ref partition, ref neededTileNumber, movingIndicesArray,
                            splitDimIdx, firstPartIndicesArray, secondPartIndicesArray, firstPartPartition,
                            secondPartPartition, currentTileNO);
                    }
                }
            }
            return neededTileNumber;
        }

        private void useChosenStrategy(int[] indicesArray, ref Coords[] partition, ref int neededTileNumber,
            int[] movingIndicesArray, int splitDimIdx, int[] firstPartIndicesArray, int[] secondPartIndicesArray,
            Coords[] firstPartPartition, Coords[] secondPartPartition, int currentTileNO)
        {
            if (strategyCode == 0)
            {
                // Optimized for clustering strategy:
                applySideDifferenceStrategy(indicesArray, ref partition, ref neededTileNumber,
                    firstPartPartition, secondPartPartition, currentTileNO, firstPartIndicesArray,
                    secondPartIndicesArray, splitDimIdx, movingIndicesArray);
            }
            else
            {
                //Optimized for load balancing strategy:
                applyMinDiffSumStrategy(indicesArray, ref partition, ref neededTileNumber,
                    firstPartPartition, secondPartPartition, currentTileNO);
            }
        }

        private void applySideDifferenceStrategy(int[] indicesArray, ref Coords[] partition,
            ref int neededTileNumber, Coords[] firstPartPartition, Coords[] secondPartPartition, int currentTileNO,
            int[] firstPartIndicesArray, int[] secondPartIndicesArray, int splitDimIdx, int[] movingIndicesArray)
        {
            if (currentTileNO < neededTileNumber)
            {
                neededTileNumber = currentTileNO;
                partition = new Coords[neededTileNumber];
                firstPartPartition.CopyTo(partition, 0);
                secondPartPartition.CopyTo(partition, firstPartPartition.Length);
                partitionArray.SetValue(partition, indicesArray);
            }
            else if (currentTileNO == neededTileNumber)
            {
                double currentDiff = determineCurrentDiffOfSides(splitDimIdx, indicesArray, movingIndicesArray);
                double maxDiff = (double)maxDiffArray.GetValue(indicesArray);
                if (maxDiff < currentDiff)
                {
                    maxDiffArray.SetValue(currentDiff, indicesArray);
                    partition = new Coords[neededTileNumber];
                    firstPartPartition.CopyTo(partition, 0);
                    secondPartPartition.CopyTo(partition, firstPartPartition.Length);
                    partitionArray.SetValue(partition, indicesArray);
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
        
        // This function applies a strategy when the stored needed tile number equals to the current value of it
        // where we would like to have tiles (multiple of server number) which are close to delta.
        // This strategy will be prosperous for the next phase where the obtained tiles are divided between servers.
        private void applyMinDiffSumStrategy(int[] indicesArray, ref Coords[] partition, 
            ref int neededTileNumber, Coords[] firstPartPartition, Coords[] secondPartPartition, int currentTileNO)
        {
            if (currentTileNO < neededTileNumber)
            {
                neededTileNumber = currentTileNO;
                partition = new Coords[neededTileNumber];
                firstPartPartition.CopyTo(partition, 0);
                secondPartPartition.CopyTo(partition, firstPartPartition.Length);
                partitionArray.SetValue(partition, indicesArray);
                // when the needed tile number is decreased we should set the current value 
                // to current indices array
                if (neededTileNumber >= serverNO)
                {
                    double currentDiffSum = determineCurrentDiffSum(neededTileNumber,
                    firstPartPartition, secondPartPartition);
                    diffSumArray.SetValue(currentDiffSum, indicesArray);
                }
            }
            else if ((currentTileNO == neededTileNumber) && (neededTileNumber >= serverNO))
            {
                double currentDiffSum = determineCurrentDiffSum(neededTileNumber, firstPartPartition, secondPartPartition);
                double diffSum = (double)diffSumArray.GetValue(indicesArray);
                if (currentDiffSum < diffSum)
                {
                    diffSumArray.SetValue(currentDiffSum, indicesArray);
                    partition = new Coords[neededTileNumber];
                    firstPartPartition.CopyTo(partition, 0);
                    secondPartPartition.CopyTo(partition, firstPartPartition.Length);
                    partitionArray.SetValue(partition, indicesArray);
                }
            }
        }

        private double determineCurrentDiffSum(int neededTileNumber, Coords[] firstPartPartition, Coords[] secondPartPartition)
        {
            double[] diffs = new double[neededTileNumber];
            for (int idx = 0; idx < firstPartPartition.Length; idx++)
            {
                diffs[idx] = firstPartPartition[idx].differenceFromDelta(delta);
            }
            for (int idx = 0; idx < secondPartPartition.Length; idx++)
            {
                diffs[idx + firstPartPartition.Length] =
                    secondPartPartition[idx].differenceFromDelta(delta);
            }
            // Sort diffs in descending order:
            Array.Sort<double>(diffs, 
                new Comparison<double>(
                            (d1, d2) => d2.CompareTo(d1))
                );

            int limitForSum = serverNO;
            double currentDiffSum = 0.0;
            for (int idx = 0; idx < limitForSum; idx++)
            {
                currentDiffSum += diffs[idx];
            }
            return currentDiffSum;
        }
    }
}
