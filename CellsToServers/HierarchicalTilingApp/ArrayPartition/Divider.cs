using HierarchicalTilingApp.Transformation;
using System;

namespace HierarchicalTilingApp.ArrayPartition
{
    public class Divider
    {
        private Array heftArray;
        private Array objectiveValueArray;
        private Array partitionArray;
        private Array maxDiffArray;
        private Transformator transformator;
        private int spaceDimension;
        private int histogramResolution;
        private int serverNO;
        private double delta;
        private double diffSum;
        private double initializationValue;

        public Divider(Array heftArray, Transformator transformator, int spaceDimension, int histogramResolution, 
            int serverNO, double delta)
        {
            this.heftArray = heftArray;
            this.transformator = transformator;
            this.spaceDimension = spaceDimension;
            this.histogramResolution = histogramResolution;
            this.serverNO = serverNO;
            this.delta = delta;
            this.initializationValue = -1.0;
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
            this.maxDiffArray = Array.CreateInstance(typeof(double), lengthsMaxDiffArray);
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
            return innerDetermineObjectiveValue(extendedIndicesArray, out partition);
        }

        private double innerDetermineObjectiveValue(int[] extendedIndicesArray, out Coords[] partition)
        {
            double objectiveValue;
            double currentObjectiveValue = (double)objectiveValueArray.GetValue(extendedIndicesArray);
            if (currentObjectiveValue >= 0)
            {
                objectiveValue = currentObjectiveValue;
                partition = (Coords[])partitionArray.GetValue(extendedIndicesArray);
            }
            else
            {
                objectiveValue = fillObjectiveValueArray(extendedIndicesArray, out partition);
            }
            return objectiveValue;
        }

        private double fillObjectiveValueArray(int[] extendedIndicesArray, out Coords[] partition)
        {
            double objectiveValue;
            int[] indicesArray = transformator.determineIndicesArray(spaceDimension, extendedIndicesArray);
            int splitNO = extendedIndicesArray[0];
            if (splitNO == 0)
            {
                int heftOfRegion = (int)heftArray.GetValue(indicesArray);
                Coords coords = new Coords
                {
                    ExtendedIndicesArray = extendedIndicesArray,
                    HeftOfRegion = heftOfRegion
                };
                partition = new Coords[] {coords};
                partitionArray.SetValue(partition, extendedIndicesArray);
                objectiveValue = coords.differenceFromDelta(delta);
            }
            else
            {
                objectiveValue = fillObjectiveValueWhenSplitNOIsLargerThenZero(extendedIndicesArray, indicesArray, 
                    out partition);
            }
            objectiveValueArray.SetValue(objectiveValue, extendedIndicesArray);
            return objectiveValue;
        }

        private double fillObjectiveValueWhenSplitNOIsLargerThenZero(int[] extendedIndicesArray, int[] indicesArray, 
            out Coords[] partition)
        {
            double objectiveValue = double.MaxValue;
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
                    out firstPartPartition);
                double objectiveValueForSecondPart = innerDetermineObjectiveValue(secondPartExtendedIndicesArray,
                    out secondPartPartition);
                
                double currentObjectiveValue = Math.Max(objectiveValueForFirstPart, objectiveValueForSecondPart);
                if (currentObjectiveValue < objectiveValue)
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
