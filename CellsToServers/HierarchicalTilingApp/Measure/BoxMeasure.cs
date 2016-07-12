using HierarchicalTilingApp.ArrayPartition;
using HierarchicalTilingApp.Transformation;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.Measure
{
    public class BoxMeasure : BaseMeasure<BoxAuxData>, IAverageMeasure
    {
        public BoxMeasure(BoxAuxData auxData, Transformator transformator)
            : base(auxData, transformator)
        {
        }

        public double averageAllMeasures(Coords[] partition)
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

        public override double computeMeasureForRegion(Coords coords)
        {
            //double entireVolume = Math.Pow(AuxData.HistogramResolution, AuxData.SpaceDimension);
            //int[] indicesArrayOfServer = transformator.determineIndicesArray(AuxData.SpaceDimension, 
            //    coords.ExtendedIndicesArray);
            //int heftOfIntersection = computeHeftOfIntersection(indicesArrayOfServer, AuxData.IndicesArrayOfQueryRegion);
            //return (entireVolume * heftOfIntersection) / (AuxData.VolumeOfQueryRegion * coords.HeftOfRegion);

            int[] indicesArrayOfServer = transformator.determineIndicesArray(AuxData.SpaceDimension, 
                coords.ExtendedIndicesArray);
            int heftOfIntersection = computeHeftOfIntersection(indicesArrayOfServer, AuxData.IndicesArrayOfQueryRegion);
            return 1 - (double)heftOfIntersection / (double)coords.HeftOfRegion;
        }

        private int computeHeftOfIntersection(int[] indicesArrayOfServer, int[] indicesArrayOfQueryRegion)
        {
            int heftOfIntersection = 0;
            bool intersected = true;
            int[] indicesArrayOfIntersection = new int[2 * AuxData.SpaceDimension];
            for (int dimIdx = 0; dimIdx < AuxData.SpaceDimension; dimIdx++)
            {
                int lowerBoundForCurrentDimOfServer = indicesArrayOfServer[2 * dimIdx];
                int upperBoundForCurrentDimOfServer = indicesArrayOfServer[2 * dimIdx + 1];
                int lowerBoundForCurrentDimOfQueryRegion = indicesArrayOfQueryRegion[2 * dimIdx];
                int upperBoundForCurrentDimOfQueryRegion = indicesArrayOfQueryRegion[2 * dimIdx + 1];
                if ((upperBoundForCurrentDimOfServer < lowerBoundForCurrentDimOfQueryRegion) ||
                    (upperBoundForCurrentDimOfQueryRegion < lowerBoundForCurrentDimOfServer))
                {
                    intersected = false;
                    break;
                }
                indicesArrayOfIntersection[2 * dimIdx] = Math.Max(lowerBoundForCurrentDimOfServer,
                    lowerBoundForCurrentDimOfQueryRegion);
                indicesArrayOfIntersection[2 * dimIdx + 1] = Math.Min(upperBoundForCurrentDimOfServer,
                    upperBoundForCurrentDimOfQueryRegion);
            }
            if (intersected)
            {
                heftOfIntersection = (int)AuxData.HeftArray.GetValue(indicesArrayOfIntersection);
            }
            return heftOfIntersection;
        }

        public override double computeMeasureForBin(int[] indicesArrayOfBin, int[] indicesArrayOfRegion)
        {
            // This method is unused for load balancing measure
            throw new NotImplementedException();
        }
    }
}
 