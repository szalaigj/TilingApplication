using HierarchicalTilingApp.Measure;
using HierarchicalTilingApp.SumOfSquares;
using HierarchicalTilingApp.Transformation;
using System;

namespace HierarchicalTilingApp.ArrayPartition
{
    public class Divider
    {
        private Array heftArray;
        private Array objectiveValueArray;
        private Array partitionArray;
        private Array hasEnoughBinsArray;
        private Array maxDiffArray;
        private Transformator transformator;
        KNNMeasure kNNMeasure;
        LoadBalancingMeasure lbMeasure;
        private int spaceDimension;
        private int histogramResolution;
        private int serverNO;
        private double delta;
        private double diffSum;
        private double initializationValue;
        private double kNNMeasCoeff;
        private double lbMeasCoeff;

        public Divider(Array array, Array heftArray, Transformator transformator, int spaceDimension, 
            int histogramResolution, int serverNO, double delta, int pointNO, int kNN, Shell[] shells,
            double kNNMeasCoeff, double lbMeasCoeff)
        {
            this.heftArray = heftArray;
            this.transformator = transformator;
            this.spaceDimension = spaceDimension;
            this.histogramResolution = histogramResolution;
            this.serverNO = serverNO;
            this.delta = delta;
            this.initializationValue = -1.0;
            this.kNNMeasCoeff = kNNMeasCoeff;
            this.lbMeasCoeff = lbMeasCoeff;
            int[] lengthsMaxDiffArray = new int[2 * spaceDimension];
            int[] lengthsObjectiveValueArray = new int[2 * spaceDimension + 1];
            lengthsObjectiveValueArray[0] = serverNO;
            for (int idx = 1; idx <= 2 * spaceDimension; idx++)
            {
                lengthsObjectiveValueArray[idx] = histogramResolution;
                lengthsMaxDiffArray[idx - 1] = histogramResolution;
            }
            this.objectiveValueArray = Array.CreateInstance(typeof(double), lengthsObjectiveValueArray);
            transformator.initializeObjectiveValueArray(this.spaceDimension, this.histogramResolution, this.serverNO, 
                this.initializationValue, this.objectiveValueArray);
            this.partitionArray = Array.CreateInstance(typeof(Coords[]), lengthsObjectiveValueArray);
            this.hasEnoughBinsArray = Array.CreateInstance(typeof(bool), lengthsObjectiveValueArray);
            this.maxDiffArray = Array.CreateInstance(typeof(double), lengthsMaxDiffArray);
            setMeasureInstances(array, pointNO, kNN, shells);
        }

        private void setMeasureInstances(Array array, int pointNO, int kNN, Shell[] shells)
        {
            KNNAuxData kNNAuxData = new KNNAuxData()
            {
                SpaceDimension = this.spaceDimension,
                HistogramResolution = this.histogramResolution,
                ServerNO = this.serverNO,
                PointNO = pointNO,
                KNN = kNN,
                Histogram = array,
                Shells = shells
            };
            this.kNNMeasure = new KNNMeasure(kNNAuxData, this.transformator);
            
            LoadBalancingAuxData lbAuxData = new LoadBalancingAuxData()
            {
                ServerNO = this.serverNO,
                PointNO = pointNO,
                Delta = this.delta
            };
            this.lbMeasure = new LoadBalancingMeasure(lbAuxData, this.transformator);
        }

        public double getDiffSum()
        {
            return diffSum;
        }

        public double determineObjectiveValue(out Coords[] partition)
        {
            int[] extendedIndicesArray = new int[2 * spaceDimension + 1];
            extendedIndicesArray[0] = serverNO - 1;
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                extendedIndicesArray[2 * idx + 1] = 0;
                extendedIndicesArray[2 * idx + 2] = histogramResolution - 1;
            }
            bool hasEnoughBins;
            double objectiveValue = innerDetermineObjectiveValue(extendedIndicesArray, out partition, out hasEnoughBins);
            if (!hasEnoughBins)
            {
                throw new NotEnoughBinsException();
            }
            objectiveValue = objectiveValue / (double)serverNO;
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
                        fillObjectiveValueWhenMovingIdxArrayIsValid(extendedIndicesArray, indicesArray, ref partition, 
                            ref objectiveValue, splitNO, movingIndicesArray, splitDimIdx);
                    }
                }
            }
            return objectiveValue;
        }

        private void fillObjectiveValueWhenMovingIdxArrayIsValid(int[] extendedIndicesArray, int[] indicesArray,
            ref Coords[] partition, ref double objectiveValue, int splitNO, int[] movingIndicesArray, int splitDimIdx)
        {
            int[] firstPartIndicesArray, secondPartIndicesArray;
            int[] firstPartExtendedIndicesArray = new int[2 * spaceDimension + 1];
            int[] secondPartExtendedIndicesArray = new int[2 * spaceDimension + 1];
            Coords[] firstPartPartition, secondPartPartition;
            bool hasEnoughBinsForFirstPart, hasEnoughBinsForSecondPart;
            transformator.splitIndicesArrays(spaceDimension, splitDimIdx, indicesArray,
                movingIndicesArray, out firstPartIndicesArray, out secondPartIndicesArray);
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
                    if (splitNO == serverNO - 1)
                    {
                        diffSum = determineCurrentDiffSum(firstPartPartition, secondPartPartition);
                    }
                }
            }
        }

        private double determineCurrentDiffSum(Coords[] firstPartPartition, Coords[] secondPartPartition)
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
