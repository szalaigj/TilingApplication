using CellsToServersApp.JensenShannonDiv;
using System;

namespace CellsToServersApp.ArrayPartition
{
    public class Divider
    {
        private Array array;
        private Array heftArray;
        private Array frequencyArray;
        private Array frequencyProjectionArray;
        private Array tileNumberArray;
        private Array partitionArray;
        private Array diffSumArray;
        private Array maxDivArray;
        private Array minDiffArray;
        private IndexTransformator transformator;
        private JenShaDivComputer jenShaDivComputer;
        private int spaceDimension;
        private int histogramResolution;
        private int serverNO;
        private double delta;

        public Divider(Array array, Array heftArray, Array frequencyArray, Array frequencyProjectionArray,
            IndexTransformator transformator, JenShaDivComputer jenShaDivComputer, int spaceDimension, 
            int histogramResolution, int serverNO, double delta)
        {
            this.array = array;
            this.heftArray = heftArray;
            this.frequencyArray = frequencyArray;
            this.frequencyProjectionArray = frequencyProjectionArray;
            this.transformator = transformator;
            this.jenShaDivComputer = jenShaDivComputer;
            this.spaceDimension = spaceDimension;
            this.histogramResolution = histogramResolution;
            this.serverNO = serverNO;
            this.delta = delta;
            int[] lengthsTileNumberArray = new int[2 * spaceDimension];
            for (int idx = 0; idx < 2 * spaceDimension; idx++)
            {
                lengthsTileNumberArray[idx] = histogramResolution;
            }
            this.tileNumberArray = Array.CreateInstance(typeof(int), lengthsTileNumberArray);
            this.partitionArray = Array.CreateInstance(typeof(Coords[]), lengthsTileNumberArray);
            this.diffSumArray = Array.CreateInstance(typeof(double), lengthsTileNumberArray);
            this.maxDivArray = Array.CreateInstance(typeof(double), lengthsTileNumberArray);
            this.minDiffArray = Array.CreateInstance(typeof(double), lengthsTileNumberArray);
        }

        public int determineNeededTileNumber(out Coords[] partition)
        {
            int[] indicesArray = new int[2 * spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                indicesArray[2 * idx] = 0;
                indicesArray[2 * idx + 1] = histogramResolution - 1;
            }
            return innerDetermineNeededTileNumber(indicesArray, out partition);
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
            if (heftOfRegion <= delta)
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
                neededTileNumber = fillTileNumberWhenRegionHeftIsLargerThenDelta(indicesArray, out partition);
            }
            tileNumberArray.SetValue(neededTileNumber, indicesArray);
            return neededTileNumber;
        }

        private int fillTileNumberWhenRegionHeftIsLargerThenDelta(int[] indicesArray, out Coords[] partition)
        {
            int neededTileNumber;
            neededTileNumber = int.MaxValue;
            partition = (Coords[])partitionArray.GetValue(indicesArray);
            //int heftDifferenceOfParts = (int)heftArray.GetValue(indicesArray);
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
                        //int firstPartHeftOfRegion = (int)heftArray.GetValue(firstPartIndicesArray);
                        int neededTileNumberForSecondPart = innerDetermineNeededTileNumber(secondPartIndicesArray,
                            out secondPartPartition);
                        //int secondPartHeftOfRegion = (int)heftArray.GetValue(secondPartIndicesArray);
                        int currentTileNO = neededTileNumberForFirstPart + neededTileNumberForSecondPart;

                        //applySideDifferenceStrategy(indicesArray, ref partition, ref neededTileNumber,
                        //    firstPartPartition, secondPartPartition, currentTileNO, firstPartIndicesArray,
                        //    secondPartIndicesArray, splitDimIdx, movingIndicesArray);

                        //Delta-approximation strategy:
                        applyTheMoreDeltaApproxStrategy(indicesArray, ref partition, ref neededTileNumber,
                            firstPartPartition, secondPartPartition, currentTileNO);

                        //applyJensenShannonWithProjectionStrategy(indicesArray, ref partition, ref neededTileNumber,
                        //    firstPartPartition, secondPartPartition, currentTileNO, firstPartIndicesArray, 
                        //    secondPartIndicesArray, splitDimIdx);

                        //applyJensenShannonStrategy(indicesArray, ref partition, ref neededTileNumber,
                        //    firstPartPartition, secondPartPartition, currentTileNO,
                        //    firstPartIndicesArray, secondPartIndicesArray);

                        //Unbalanced splitting selection strategy:
                        // applyUnbalancedSplittingSelectionStrategy(indicesArray, ref partition, ref neededTileNumber, 
                        //    ref heftDifferenceOfParts, firstPartPartition, 
                        //    secondPartPartition, firstPartHeftOfRegion, secondPartHeftOfRegion, currentTileNO);
                    }
                }
            }
            return neededTileNumber;
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
                double currentDiff = determineCurrentDiffOfSides(spaceDimension, splitDimIdx, 
                    indicesArray, movingIndicesArray);
                double minDiff = (double)minDiffArray.GetValue(indicesArray);
                if (currentDiff < minDiff)
                {
                    minDiffArray.SetValue(currentDiff, indicesArray);
                    partition = new Coords[neededTileNumber];
                    firstPartPartition.CopyTo(partition, 0);
                    secondPartPartition.CopyTo(partition, firstPartPartition.Length);
                    partitionArray.SetValue(partition, indicesArray);
                }
            }
        }

        private double determineCurrentDiffOfSides(int spaceDimension, int splitDimIdx, 
            int[] indicesArray, int[] movingIndicesArray)
        {
            int[] firstSideOfSplit, secondSideOfSplit;
            transformator.determineSidesOfSplit(spaceDimension, splitDimIdx, indicesArray, movingIndicesArray, 
                out firstSideOfSplit, out secondSideOfSplit);
            int heftFirstSideOfSplit = (int)heftArray.GetValue(firstSideOfSplit);
            int heftSecondSideOfSplit = (int)heftArray.GetValue(secondSideOfSplit);
            return Math.Abs(heftFirstSideOfSplit - heftSecondSideOfSplit);
        }

        private void applyJensenShannonWithProjectionStrategy(int[] indicesArray, ref Coords[] partition,
            ref int neededTileNumber, Coords[] firstPartPartition, Coords[] secondPartPartition, int currentTileNO,
            int[] firstPartIndicesArray, int[] secondPartIndicesArray, int splitDimIdx)
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
                double currentDiv = determineCurrentDiv(firstPartIndicesArray, secondPartIndicesArray, splitDimIdx);
                double maxDiv = (double)maxDivArray.GetValue(indicesArray);
                if (maxDiv < currentDiv)
                {
                    maxDivArray.SetValue(currentDiv, indicesArray);
                    partition = new Coords[neededTileNumber];
                    firstPartPartition.CopyTo(partition, 0);
                    secondPartPartition.CopyTo(partition, firstPartPartition.Length);
                    partitionArray.SetValue(partition, indicesArray);
                }
            }
        }

        private double determineCurrentDiv(int[] firstPartIndicesArray, int[] secondPartIndicesArray, int splitDimIdx)
        {
            double firstPartHeftOfRegion = (double)((int)heftArray.GetValue(firstPartIndicesArray));
            double secondPartHeftOfRegion = (double)((int)heftArray.GetValue(secondPartIndicesArray));
            double firstWeight = firstPartHeftOfRegion / (firstPartHeftOfRegion + secondPartHeftOfRegion);
            double secondWeight = secondPartHeftOfRegion / (firstPartHeftOfRegion + secondPartHeftOfRegion);
            int[] firstPartExtendedIndicesArray = new int[firstPartIndicesArray.Length + 1];
            firstPartExtendedIndicesArray[0] = splitDimIdx;
            firstPartIndicesArray.CopyTo(firstPartExtendedIndicesArray, 1);
            int[] secondPartExtendedIndicesArray = new int[secondPartIndicesArray.Length + 1];
            secondPartExtendedIndicesArray[0] = splitDimIdx;
            secondPartIndicesArray.CopyTo(secondPartExtendedIndicesArray, 1);
            double[] firstPartFrequenciesOfRegion = (double[])frequencyProjectionArray.
                GetValue(firstPartExtendedIndicesArray);
            double[] secondPartFrequenciesOfRegion = (double[])frequencyProjectionArray.
                GetValue(secondPartExtendedIndicesArray);
            double currentDiv = jenShaDivComputer.computeDivergence(firstPartFrequenciesOfRegion, 
                secondPartFrequenciesOfRegion, firstWeight, secondWeight);
            return currentDiv;
        }

        private void applyJensenShannonStrategy(int[] indicesArray, ref Coords[] partition, 
            ref int neededTileNumber, Coords[] firstPartPartition, Coords[] secondPartPartition, int currentTileNO, 
            int[] firstPartIndicesArray, int[] secondPartIndicesArray)
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
                double currentDiv = determineCurrentDiv(firstPartIndicesArray, secondPartIndicesArray);
                double maxDiv = (double)maxDivArray.GetValue(indicesArray);
                if (maxDiv < currentDiv)
                {
                    maxDivArray.SetValue(currentDiv, indicesArray);
                    partition = new Coords[neededTileNumber];
                    firstPartPartition.CopyTo(partition, 0);
                    secondPartPartition.CopyTo(partition, firstPartPartition.Length);
                    partitionArray.SetValue(partition, indicesArray);
                }
            }
        }

        private double determineCurrentDiv(int[] firstPartIndicesArray, int[] secondPartIndicesArray)
        {
            double firstPartHeftOfRegion = (double)((int)heftArray.GetValue(firstPartIndicesArray));
            double secondPartHeftOfRegion = (double)((int)heftArray.GetValue(secondPartIndicesArray));
            double firstWeight = firstPartHeftOfRegion / (firstPartHeftOfRegion + secondPartHeftOfRegion);
            double secondWeight = secondPartHeftOfRegion / (firstPartHeftOfRegion + secondPartHeftOfRegion);
            double[] firstPartFrequenciesOfRegion = (double[])frequencyArray.GetValue(firstPartIndicesArray);
            double[] secondPartFrequenciesOfRegion = (double[])frequencyArray.GetValue(secondPartIndicesArray);
            double currentDiv = jenShaDivComputer.computeDivergence(firstPartFrequenciesOfRegion, secondPartFrequenciesOfRegion,
                firstWeight, secondWeight);
            return currentDiv;
        }


        // This function applies a strategy when the stored needed tile number equals to the current value of it
        // where we would like to have tiles (multiple of server number) which are close to delta.
        // This strategy will be prosperous for the next phase where the obtained tiles are divided between servers.
        private void applyTheMoreDeltaApproxStrategy(int[] indicesArray, ref Coords[] partition, 
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
                double currentDiffSum = determineCurrentDiffSum(neededTileNumber,
                    firstPartPartition, secondPartPartition);
                diffSumArray.SetValue(currentDiffSum, indicesArray);
            }
            else if (currentTileNO == neededTileNumber)
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
            // if the tile number is still less than the server number we sums all differences
            int limitForSum = (neededTileNumber < serverNO) ? neededTileNumber : serverNO;
            double currentDiffSum = 0.0;
            for (int idx = 0; idx < limitForSum; idx++)
            {
                currentDiffSum += diffs[idx];
            }
            return currentDiffSum;
        }

        // UNBALANCED SPLITTING SELECTION STRATEGY
        //private void applyUnbalancedSplittingSelectionStrategy(int[] indicesArray, ref Coords[] partition, ref int neededTileNumber, ref int heftDifferenceOfParts, Coords[] firstPartPartition, Coords[] secondPartPartition, int firstPartHeftOfRegion, int secondPartHeftOfRegion, int currentTileNO)
        //{
        //    int currentHeftDifferenceOfParts = Math.Abs(firstPartHeftOfRegion - secondPartHeftOfRegion);
        //    if ((currentTileNO < neededTileNumber) ||
        //        // Or if the current tile number equals to needed tile number...
        //        ((currentTileNO == neededTileNumber) &&
        //        // ...select the most 'load UNbalanced splitting'
        //        (currentHeftDifferenceOfParts > heftDifferenceOfParts)))
        //    {
        //        neededTileNumber = currentTileNO;
        //        heftDifferenceOfParts = currentHeftDifferenceOfParts;
        //        partition = new Coords[neededTileNumber];
        //        firstPartPartition.CopyTo(partition, 0);
        //        secondPartPartition.CopyTo(partition, firstPartPartition.Length);
        //        partitionArray.SetValue(partition, indicesArray);
        //    }
        //}
    }
}
