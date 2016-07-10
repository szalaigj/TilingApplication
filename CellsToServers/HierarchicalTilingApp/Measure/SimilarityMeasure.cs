using HierarchicalTilingApp.ArrayPartition;
using HierarchicalTilingApp.Transformation;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.Measure
{
    public abstract class SimilarityMeasure<T> : BaseMeasure<T>
        where T : SimilarityAuxData
    {
        public SimilarityMeasure(T auxData, Transformator transformator)
            : base(auxData, transformator)
        {
        }

        public override double computeMeasureForRegion(Coords coords)
        {
            double measureForRegion = 0.0;
            int[] indicesArrayOfRegion =
                    transformator.determineIndicesArray(AuxData.SpaceDimension, coords.ExtendedIndicesArray);
            int binNOInRegionWithoutZeroHeft = 0;
            for (int[] indicesArrayOfBin = transformator.determineFirstIndicesArray(indicesArrayOfRegion);
                    indicesArrayOfBin != null;
                    indicesArrayOfBin = transformator.determineNextIndicesArray(indicesArrayOfRegion, indicesArrayOfBin))
            {
                int binValue = (int)AuxData.Histogram.GetValue(indicesArrayOfBin);
                if (binValue > 0)
                {
                    binNOInRegionWithoutZeroHeft++;
                    measureForRegion += computeMeasureForBin(indicesArrayOfBin, indicesArrayOfRegion);
                }
            }
            measureForRegion = measureForRegion / (double)binNOInRegionWithoutZeroHeft;
            return measureForRegion;
        }

        protected bool isIdxArrayInTheRegion(int[] idxArray, int[] indicesArrayOfRegion)
        {
            bool result = true;
            for (int idx = 0; idx < idxArray.Length; idx++)
            {
                int lowerBound = indicesArrayOfRegion[2 * idx];
                int upperBound = indicesArrayOfRegion[2 * idx + 1];
                if (!((lowerBound <= idxArray[idx]) && (idxArray[idx] <= upperBound)))
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
