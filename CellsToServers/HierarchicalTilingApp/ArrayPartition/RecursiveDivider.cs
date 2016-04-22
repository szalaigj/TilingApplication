using HierarchicalTilingApp.Measure;
using HierarchicalTilingApp.SumOfSquares;
using HierarchicalTilingApp.Transformation;
using System;

namespace HierarchicalTilingApp.ArrayPartition
{
    public class RecursiveDivider : BaseDivider
    {
        private double initializationValue;

        public RecursiveDivider(Array array, Array heftArray, Transformator transformator, int spaceDimension, 
            int histogramResolution, int serverNO, double delta, int pointNO, int kNN, Shell[] shells,
            double kNNMeasCoeff, double lbMeasCoeff)
            : base(array, heftArray, transformator, spaceDimension,
                histogramResolution, serverNO, delta, pointNO, kNN, shells, kNNMeasCoeff, lbMeasCoeff)
        {
            this.initializationValue = -1.0;
            transformator.initializeObjectiveValueArray(this.initializationValue, this.objectiveValueArray);
        }

        public override double determineObjectiveValue(out Coords[] partition)
        {
            int[] extendedIndicesArray = determineExtendedIndicesArray();
            bool hasEnoughBins;
            double objectiveValue = innerDetermineObjectiveValue(extendedIndicesArray, out partition, out hasEnoughBins);
            if (!hasEnoughBins)
            {
                throw new NotEnoughBinsException();
            }
            objectiveValue = objectiveValue / (double)serverNO;
            diffSum = determineCurrentDiffSum(partition);
            double measureOfKNN = kNNMeasure.computeMeasure(partition);
            Console.WriteLine("k-NN measure of the partition: {0}", measureOfKNN);
            double measureOfLB = lbMeasure.computeMeasure(partition);
            Console.WriteLine("Load balancing measure of the partition: {0}", measureOfLB);
            return objectiveValue;
        }

        private double innerDetermineObjectiveValue(int[] extendedIndicesArray, out Coords[] partition, 
            out bool hasEnoughBins)
        {
            double objectiveValue;
            double currentObjectiveValue = (double)objectiveValueArray.GetValue(extendedIndicesArray);
            if (currentObjectiveValue >= 0)
            {
                objectiveValue = currentObjectiveValue;
                partition = (Coords[])partitionArray.GetValue(extendedIndicesArray);
                hasEnoughBins = (bool)hasEnoughBinsArray.GetValue(extendedIndicesArray);
            }
            else
            {
                objectiveValue = fillObjectiveValueArray(extendedIndicesArray, out partition, out hasEnoughBins);
            }
            return objectiveValue;
        }

        private double fillObjectiveValueArray(int[] extendedIndicesArray, out Coords[] partition,
            out bool hasEnoughBins)
        {
            double objectiveValue = 0.0;
            int[] indicesArray = transformator.determineIndicesArray(spaceDimension, extendedIndicesArray);
            int splitNO = extendedIndicesArray[0];
            if (splitNO == 0)
            {
                hasEnoughBins = true;
                partition = fillObjectiveValueWhenSplitNOIsZero(extendedIndicesArray, ref objectiveValue, indicesArray);
            }
            else
            {
                hasEnoughBins = transformator.validateRegionHasEnoughBins(spaceDimension, indicesArray, splitNO);
                if (hasEnoughBins)
                    objectiveValue = fillObjectiveValueWhenSplitNOIsLargerThenZero(extendedIndicesArray, indicesArray,
                    out partition);
                else
                    partition = null;
            }
            objectiveValueArray.SetValue(objectiveValue, extendedIndicesArray);
            hasEnoughBinsArray.SetValue(hasEnoughBins, extendedIndicesArray);
            return objectiveValue;
        }

        private Coords[] fillObjectiveValueWhenSplitNOIsZero(int[] extendedIndicesArray, ref double objectiveValue, 
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
            return partition;
        }

        private double fillObjectiveValueWhenSplitNOIsLargerThenZero(int[] extendedIndicesArray, int[] indicesArray, 
            out Coords[] partition)
        {
            double objectiveValue = 0.0;
            int splitNO = extendedIndicesArray[0];
            partition = (Coords[])partitionArray.GetValue(extendedIndicesArray);
            for (int splitDimIdx = 0; splitDimIdx < spaceDimension; splitDimIdx++)
            {
                for (int componentInSplitDim = indicesArray[2 * splitDimIdx];
                    componentInSplitDim < indicesArray[2 * splitDimIdx + 1]; componentInSplitDim++)
                {
                    fillObjectiveValueForSplitComponent(extendedIndicesArray, indicesArray, ref partition,
                            ref objectiveValue, splitNO, componentInSplitDim, splitDimIdx);
                }
            }
            return objectiveValue;
        }

        private void fillObjectiveValueForSplitComponent(int[] extendedIndicesArray, int[] indicesArray,
            ref Coords[] partition, ref double objectiveValue, int splitNO, int componentInSplitDim, int splitDimIdx)
        {
            int[] firstPartIndicesArray, secondPartIndicesArray;
            int[] firstPartExtendedIndicesArray = new int[2 * spaceDimension + 1];
            int[] secondPartExtendedIndicesArray = new int[2 * spaceDimension + 1];
            Coords[] firstPartPartition, secondPartPartition;
            bool hasEnoughBinsForFirstPart, hasEnoughBinsForSecondPart;
            transformator.splitIndicesArrays(spaceDimension, splitDimIdx, indicesArray,
                componentInSplitDim, out firstPartIndicesArray, out secondPartIndicesArray);
            for (int firstSplitNO = 0; firstSplitNO <= splitNO - 1; firstSplitNO++)
            {
                int secondSplitNO = (splitNO - 1) - firstSplitNO;
                firstPartIndicesArray.CopyTo(firstPartExtendedIndicesArray, 1);
                firstPartExtendedIndicesArray[0] = firstSplitNO;
                secondPartIndicesArray.CopyTo(secondPartExtendedIndicesArray, 1);
                secondPartExtendedIndicesArray[0] = secondSplitNO;
                double objectiveValueForFirstPart = innerDetermineObjectiveValue(firstPartExtendedIndicesArray,
                    out firstPartPartition, out hasEnoughBinsForFirstPart);
                double objectiveValueForSecondPart = innerDetermineObjectiveValue(secondPartExtendedIndicesArray,
                    out secondPartPartition, out hasEnoughBinsForSecondPart);

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
                    partition = new Coords[splitNO + 1];
                    setPartitionByParts(extendedIndicesArray, partition, firstPartPartition, secondPartPartition);
                }
            }
        }
    }
}
