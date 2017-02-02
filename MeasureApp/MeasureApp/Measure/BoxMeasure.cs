using MeasureApp.Data;
using MeasureApp.Transformation;
using System;
using System.Collections.Generic;

namespace MeasureApp.Measure
{
    public class BoxMeasure : BaseMeasure<BoxAuxData>, IAverageMeasure
    {
        public BoxMeasure(BoxAuxData auxData, Transformator transformator)
            : base(auxData, transformator)
        {
        }

        public double averageAllMeasures(BinGroup[] partition)
        {
            double result = 0.0;
            for (int[] outerIndicesArray = transformator.determineFirstIndicesArray(AuxData.Histogram);
                    outerIndicesArray != null;
                    outerIndicesArray = transformator.determineNextIndicesArray(AuxData.Histogram, outerIndicesArray))
            {
                for (int[] innerIndicesArray = outerIndicesArray;
                    innerIndicesArray != null;
                    innerIndicesArray = transformator.determineNextIndicesArray(AuxData.Histogram, outerIndicesArray, innerIndicesArray))
                {
                    AuxData.IndicesArrayOfQueryRegion = transformator.mergeIndicesArrays(AuxData.SpaceDimension,
                        outerIndicesArray, innerIndicesArray);
                    AuxData.VolumeOfQueryRegion = computeVolumeOfQueryRegion();
                    result += computeMeasure(partition);
                }
            }
            result *= (Math.Pow(2, AuxData.SpaceDimension) / (Math.Pow(AuxData.HistogramResolution, AuxData.SpaceDimension) *
                Math.Pow(AuxData.HistogramResolution + 1, AuxData.SpaceDimension)));
            return result;
        }

        private double computeVolumeOfQueryRegion()
        {
            double volumeOfQueryRegion = 1.0;
            for (int dimIdx = 0; dimIdx < AuxData.SpaceDimension; dimIdx++)
            {
                int lowerBoundForCurrentDim = AuxData.IndicesArrayOfQueryRegion[2 * dimIdx];
                int upperBoundForCurrentDim = AuxData.IndicesArrayOfQueryRegion[2 * dimIdx + 1];
                volumeOfQueryRegion *= (upperBoundForCurrentDim - lowerBoundForCurrentDim + 1);
            }
            return volumeOfQueryRegion;
        }

        public override double computeMeasureForRegion(BinGroup binGroup)
        {
            int heftOfIntersection = computeHeftOfIntersection(binGroup.BinList, AuxData.IndicesArrayOfQueryRegion);
            return 1 - (double)heftOfIntersection / (double)binGroup.Heft;
        }

        private int computeHeftOfIntersection(List<int> binList, int[] indicesArrayOfQueryRegion)
        {
            int heftOfIntersection = 0;
            foreach (var binIdx in binList)
            {
                bool intersected = true;
                int[] indicesArray = new int[AuxData.Histogram.Rank];
                transformator.transformCellIdxToIndicesArray(AuxData.HistogramResolution, indicesArray, binIdx);
                for (int dimIdx = 0; dimIdx < AuxData.SpaceDimension; dimIdx++)
                {
                    int lowerBoundForCurrentDimOfQueryRegion = indicesArrayOfQueryRegion[2 * dimIdx];
                    int upperBoundForCurrentDimOfQueryRegion = indicesArrayOfQueryRegion[2 * dimIdx + 1];
                    if ((indicesArray[dimIdx] < lowerBoundForCurrentDimOfQueryRegion) ||
                    (upperBoundForCurrentDimOfQueryRegion < indicesArray[dimIdx]))
                    {
                        intersected = false;
                        break;
                    }
                }
                if (intersected)
                {
                    heftOfIntersection += (int)AuxData.Histogram.GetValue(indicesArray);
                }
            }
            return heftOfIntersection;
        }

        public override double computeMeasureForBin(int[] indicesArrayOfBin, List<int> binList)
        {
            // This method is unused for box measure
            throw new NotImplementedException();
        }
    }
}
 