using MeasureApp.Data;
using MeasureApp.SumOfSquares;
using MeasureApp.Transformation;
using System;
using System.Collections.Generic;

namespace MeasureApp.Measure
{
    public class RangeMeasure : SimilarityMeasure<RangeAuxData>, IAverageMeasure
    {
        public RangeMeasure(RangeAuxData auxData, Transformator transformator)
            : base(auxData, transformator)
        {
        }

        public double averageAllMeasures(BinGroup[] partition)
        {
            double result = 0.0;
            for (int rangeIdx = 1; rangeIdx <= AuxData.MaxRange; rangeIdx++)
            {
                AuxData.Range = rangeIdx;
                result += computeMeasure(partition);
            }
            result /= (double)AuxData.MaxRange;
            return result;
        }

        public override double computeMeasureForBin(int[] indicesArrayOfBin, List<int> binList)
        {
            double measureForBin = 0.0;
            int binValue = (int)AuxData.Histogram.GetValue(indicesArrayOfBin);
            if (binValue > 0)
            {
                Shell[] currentShells = new Shell[AuxData.Range];
                Array.Copy(AuxData.Shells, currentShells, AuxData.Range);
                var dictOfShells = transformator.convertIntPairsOfShellsToListOfIdxArrays(
                        AuxData.HistogramResolution, indicesArrayOfBin, currentShells);
                measureForBin = iterateOverShells(binList, dictOfShells);
            }
            return measureForBin;
        }

        private double iterateOverShells(List<int> binList, Dictionary<int, List<int[]>> dictOfShells)
        {
            int pointsInServer = 0;
            int pointsOutServer = 0;
            foreach (var shellIdx in dictOfShells.Keys)
            {
                List<int[]> idxArraysOnCurrentShell = dictOfShells[shellIdx];
                foreach (var idxArray in idxArraysOnCurrentShell)
                {
                    int currentBinValue = (int)AuxData.Histogram.GetValue(idxArray);
                    if (isIdxArrayInTheRegion(idxArray, binList))
                    {
                        pointsInServer += currentBinValue;
                    }
                    else
                    {
                        pointsOutServer += currentBinValue;
                    }
                }
            }
            double measureForBin = (double)pointsInServer / (double)(pointsInServer + pointsOutServer);
            return measureForBin;
        }
    }
}
