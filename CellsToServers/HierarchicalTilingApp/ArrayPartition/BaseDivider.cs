using HierarchicalTilingApp.Measure;
using HierarchicalTilingApp.SumOfSquares;
using HierarchicalTilingApp.Transformation;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.ArrayPartition
{
    public abstract class BaseDivider
    {
        protected Array heftArray;
        protected Array objectiveValueArray;
        protected Array partitionArray;
        protected Array hasEnoughBinsArray;
        protected Transformator transformator;
        protected KNNMeasure kNNMeasure;
        protected LoadBalancingMeasure lbMeasure;
        protected int spaceDimension;
        protected int histogramResolution;
        protected int serverNO;
        protected double delta;
        protected double diffSum;
        protected double kNNMeasCoeff;
        protected double lbMeasCoeff;

        public BaseDivider(Array array, Array heftArray, Transformator transformator, int spaceDimension, 
            int histogramResolution, int serverNO, double delta, int pointNO, int kNN, Shell[] shells,
            double kNNMeasCoeff, double lbMeasCoeff)
        {
            this.heftArray = heftArray;
            this.transformator = transformator;
            this.spaceDimension = spaceDimension;
            this.histogramResolution = histogramResolution;
            this.serverNO = serverNO;
            this.delta = delta;
            this.kNNMeasCoeff = kNNMeasCoeff;
            this.lbMeasCoeff = lbMeasCoeff;
            int[] lengthsObjectiveValueArray = new int[2 * spaceDimension + 1];
            lengthsObjectiveValueArray[0] = serverNO;
            for (int idx = 1; idx <= 2 * spaceDimension; idx++)
            {
                lengthsObjectiveValueArray[idx] = histogramResolution;
            }
            this.objectiveValueArray = Array.CreateInstance(typeof(double), lengthsObjectiveValueArray);
            this.partitionArray = Array.CreateInstance(typeof(Coords[]), lengthsObjectiveValueArray);
            this.hasEnoughBinsArray = Array.CreateInstance(typeof(bool), lengthsObjectiveValueArray);
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

        public abstract double determineObjectiveValue(out Coords[] partition);

        protected int[] determineExtendedIndicesArray()
        {
            int[] extendedIndicesArray = new int[2 * spaceDimension + 1];
            extendedIndicesArray[0] = serverNO - 1;
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                extendedIndicesArray[2 * idx + 1] = 0;
                extendedIndicesArray[2 * idx + 2] = histogramResolution - 1;
            }
            return extendedIndicesArray;
        }

        protected double determineCurrentDiffSum(Coords[] partition)
        {
            double diffSum = 0.0;
            for (int idx = 0; idx < partition.Length; idx++)
            {
                diffSum += partition[idx].differenceFromDelta(delta);
            }
            return diffSum;
        }

        protected void setPartitionByParts(int[] extendedIndicesArray, Coords[] partition,
            Coords[] firstPartPartition, Coords[] secondPartPartition)
        {
            firstPartPartition.CopyTo(partition, 0);
            secondPartPartition.CopyTo(partition, firstPartPartition.Length);
            partitionArray.SetValue(partition, extendedIndicesArray);
        }
    }
}
