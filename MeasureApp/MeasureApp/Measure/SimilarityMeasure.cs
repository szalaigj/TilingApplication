using MeasureApp.Data;
using MeasureApp.Transformation;
using System;
using System.Collections.Generic;

namespace MeasureApp.Measure
{
    public abstract class SimilarityMeasure<T> : BaseMeasure<T>
        where T : SimilarityAuxData
    {
        public SimilarityMeasure(T auxData, Transformator transformator)
            : base(auxData, transformator)
        {
        }

        public override double computeMeasureForRegion(BinGroup binGroup)
        {
            double measureForRegion = 0.0;
            int binNOInRegionWithoutZeroHeft = 0;
            foreach (var vertexIdx in binGroup.BinList)
            {
                int[] indicesArrayOfBin = new int[AuxData.SpaceDimension];
                transformator.transformCellIdxToIndicesArray(AuxData.HistogramResolution, indicesArrayOfBin, vertexIdx);
                int binValue = (int)AuxData.Histogram.GetValue(indicesArrayOfBin);
                if (binValue > 0)
                {
                    binNOInRegionWithoutZeroHeft++;
                    measureForRegion += computeMeasureForBin(indicesArrayOfBin, binGroup.BinList);
                }
            }
            measureForRegion = measureForRegion / (double)binNOInRegionWithoutZeroHeft;
            return measureForRegion;
        }

        protected bool isIdxArrayInTheRegion(int[] idxArray, List<int> binList)
        {
            bool result = false;
            int targetBinIdx = transformator.transformIndicesArrayToCellIdx(AuxData.HistogramResolution, idxArray);
            foreach (var binIdx in binList)
            {
                if (targetBinIdx == binIdx)
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
