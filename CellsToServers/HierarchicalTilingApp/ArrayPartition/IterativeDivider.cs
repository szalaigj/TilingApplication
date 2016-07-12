using HierarchicalTilingApp.Measure;
using HierarchicalTilingApp.SumOfSquares;
using HierarchicalTilingApp.Transformation;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.ArrayPartition
{
    public class IterativeDivider : BaseDivider
    {
        public IterativeDivider(Array array, Array heftArray, Transformator transformator, int spaceDimension,
            int histogramResolution, int serverNO, double delta, int pointNO, int kNN, int maxRange,
            Shell[] shellsForKNN, Shell[] shellsForRange, double kNNMeasCoeff, double lbMeasCoeff)
            : base(array, heftArray, transformator, spaceDimension, histogramResolution, serverNO, delta, 
                pointNO, kNN, maxRange, shellsForKNN, shellsForRange, kNNMeasCoeff, lbMeasCoeff)
        {
        }

        public override double determineObjectiveValue(out Coords[] partition)
        {
            int[] extendedIndicesArray = determineExtendedIndicesArray();

            fillObjectiveValueArray();
            
            bool hasEnoughBins = (bool)hasEnoughBinsArray.GetValue(extendedIndicesArray);
            double objectiveValue = (double)objectiveValueArray.GetValue(extendedIndicesArray);
            if (!hasEnoughBins)
            {
                throw new NotEnoughBinsException();
            }
            objectiveValue = objectiveValue / (double)serverNO;
            partition = (Coords[])partitionArray.GetValue(extendedIndicesArray);
            diffSum = determineCurrentDiffSum(partition);
            double measureOfKNN = kNNMeasure.computeMeasure(partition);
            Console.WriteLine("k-NN measure of the partition: {0}", measureOfKNN);
            double measureOfRange = rangeMeasure.averageAllMeasures(partition);
            Console.WriteLine("Range measure of the partition: {0}", measureOfRange);
            double measureOfLB = lbMeasure.computeMeasure(partition);
            Console.WriteLine("Load balancing measure of the partition: {0}", measureOfLB);
            double measureOfBox = boxMeasure.averageAllMeasures(partition);
            Console.WriteLine("Box measure of the partition: {0}", measureOfBox);
            return objectiveValue;
        }

        private void fillObjectiveValueArray()
        {
            for (int splitNO = 0; splitNO < serverNO; splitNO++)
            {
                for (int[] outerIndicesArray = new int[spaceDimension];
                    outerIndicesArray != null;
                    outerIndicesArray = determineNextIndicesArray(outerIndicesArray))
                {
                    for (int[] innerIndicesArray = outerIndicesArray;
                        innerIndicesArray != null;
                        innerIndicesArray = determineNextIndicesArray(innerIndicesArray))
                    {
                        int[] extendedIndicesArray = transformator.mergeIndicesArrays(spaceDimension, splitNO,
                            outerIndicesArray, innerIndicesArray);
                        double objectiveValue = 0.0;
                        bool hasEnoughBins;
                        int[] indicesArray = transformator.determineIndicesArray(spaceDimension, extendedIndicesArray);
                        if (splitNO == 0)
                        {
                            hasEnoughBins = true;
                            objectiveValue = fillObjectiveValueWhenSplitNOIsZero(extendedIndicesArray, objectiveValue,
                                indicesArray);
                        }
                        else
                        {
                            hasEnoughBins = transformator.validateRegionHasEnoughBins(spaceDimension, indicesArray, splitNO);
                            objectiveValue = fillObjectiveValueWhenSplitNOIsLargerThenZero(extendedIndicesArray,
                                objectiveValue, indicesArray, splitNO);
                        }
                        objectiveValueArray.SetValue(objectiveValue, extendedIndicesArray);
                        hasEnoughBinsArray.SetValue(hasEnoughBins, extendedIndicesArray);
                    }
                }
            }
        }

        private int[] determineNextIndicesArray(int[] previousIndicesArray)
        {
            int[] nextIndicesArray = new int[spaceDimension];
            previousIndicesArray.CopyTo(nextIndicesArray, 0);
            for (int dimIdx = spaceDimension - 1; dimIdx >= 0; --dimIdx)
            {
                nextIndicesArray[dimIdx]++;
                if (nextIndicesArray[dimIdx] < histogramResolution)
                    return nextIndicesArray;
                nextIndicesArray[dimIdx] = 0;
            }
            return null;
        }

        private double fillObjectiveValueWhenSplitNOIsZero(int[] extendedIndicesArray, double objectiveValue, 
            int[] indicesArray)
        {
            Coords[] partition;
            int heftOfRegion = (int)heftArray.GetValue(indicesArray);
            Coords coords = new Coords
            {
                ExtendedIndicesArray = extendedIndicesArray,
                HeftOfRegion = heftOfRegion
            };
            partition = new Coords[] { coords };
            partitionArray.SetValue(partition, extendedIndicesArray);
            // If the heft of the current region is zero the objective value is zero 
            // because region with zero heft should not belong to a server.
            if (heftOfRegion != 0)
                objectiveValue = kNNMeasCoeff * kNNMeasure.computeMeasureForRegion(coords) +
                lbMeasCoeff * lbMeasure.computeMeasureForRegion(coords);
            return objectiveValue;
        }

        private double fillObjectiveValueWhenSplitNOIsLargerThenZero(int[] extendedIndicesArray, double objectiveValue, 
            int[] indicesArray, int splitNO)
        {
            for (int splitDimIdx = 0; splitDimIdx < spaceDimension; splitDimIdx++)
            {
                for (int componentInSplitDim = indicesArray[2 * splitDimIdx];
                    componentInSplitDim < indicesArray[2 * splitDimIdx + 1]; componentInSplitDim++)
                {
                    objectiveValue = fillObjectiveValueForSplitComponent(extendedIndicesArray, objectiveValue, 
                        indicesArray, splitNO, splitDimIdx, componentInSplitDim);
                }
            }
            return objectiveValue;
        }

        private double fillObjectiveValueForSplitComponent(int[] extendedIndicesArray, double objectiveValue, 
            int[] indicesArray, int splitNO, int splitDimIdx, int componentInSplitDim)
        {
            int[] firstPartIndicesArray, secondPartIndicesArray;
            transformator.splitIndicesArrays(spaceDimension, splitDimIdx, indicesArray,
                componentInSplitDim, out firstPartIndicesArray, out secondPartIndicesArray);
            for (int firstSplitNO = 0; firstSplitNO <= splitNO - 1; firstSplitNO++)
            {
                int secondSplitNO = (splitNO - 1) - firstSplitNO;
                objectiveValue = fillObjectiveValueForFixedSplitNOComposition(extendedIndicesArray, objectiveValue, 
                    splitNO, firstPartIndicesArray, secondPartIndicesArray, firstSplitNO, secondSplitNO);
            }
            return objectiveValue;
        }

        private double fillObjectiveValueForFixedSplitNOComposition(int[] extendedIndicesArray, double objectiveValue, 
            int splitNO, int[] firstPartIndicesArray, int[] secondPartIndicesArray, int firstSplitNO, int secondSplitNO)
        {
            double objectiveValueForFirstPart, objectiveValueForSecondPart;
            Coords[] firstPartPartition, secondPartPartition;
            bool hasEnoughBinsForFirstPart, hasEnoughBinsForSecondPart;
            getValuesFromParts(firstPartIndicesArray, secondPartIndicesArray, firstSplitNO, secondSplitNO, 
                out objectiveValueForFirstPart, out firstPartPartition, out hasEnoughBinsForFirstPart, 
                out objectiveValueForSecondPart, out secondPartPartition, out hasEnoughBinsForSecondPart);

            double currentObjectiveValue = 0.0;
            // If the objective value of a part of the current region is zero
            // then the current objective value is zero because this case is applied
            // when a part of the current region has zero heft which would belong to a server.
            if ((objectiveValueForFirstPart != 0.0) && (objectiveValueForSecondPart != 0.0))
            {
                currentObjectiveValue = objectiveValueForFirstPart + objectiveValueForSecondPart;
            }
            if ((currentObjectiveValue >= objectiveValue)
                && hasEnoughBinsForFirstPart && hasEnoughBinsForSecondPart)
            {
                objectiveValue = currentObjectiveValue;
                Coords[] partition = new Coords[splitNO + 1];
                setPartitionByParts(extendedIndicesArray, partition, firstPartPartition, secondPartPartition);
            }
            return objectiveValue;
        }

        private void getValuesFromParts(int[] firstPartIndicesArray, int[] secondPartIndicesArray, int firstSplitNO, 
            int secondSplitNO, out double objectiveValueForFirstPart, out Coords[] firstPartPartition, 
            out bool hasEnoughBinsForFirstPart, out double objectiveValueForSecondPart, 
            out Coords[] secondPartPartition, out bool hasEnoughBinsForSecondPart)
        {
            int[] firstPartExtendedIndicesArray = new int[2 * spaceDimension + 1];
            int[] secondPartExtendedIndicesArray = new int[2 * spaceDimension + 1];
            firstPartIndicesArray.CopyTo(firstPartExtendedIndicesArray, 1);
            firstPartExtendedIndicesArray[0] = firstSplitNO;
            secondPartIndicesArray.CopyTo(secondPartExtendedIndicesArray, 1);
            secondPartExtendedIndicesArray[0] = secondSplitNO;
            objectiveValueForFirstPart = (double)objectiveValueArray.GetValue(firstPartExtendedIndicesArray);
            firstPartPartition = (Coords[])partitionArray.GetValue(firstPartExtendedIndicesArray);
            hasEnoughBinsForFirstPart = (bool)hasEnoughBinsArray.GetValue(firstPartExtendedIndicesArray);
            objectiveValueForSecondPart = (double)objectiveValueArray.GetValue(secondPartExtendedIndicesArray);
            secondPartPartition = (Coords[])partitionArray.GetValue(secondPartExtendedIndicesArray);
            hasEnoughBinsForSecondPart = (bool)hasEnoughBinsArray.GetValue(secondPartExtendedIndicesArray);
        }
    }
}
