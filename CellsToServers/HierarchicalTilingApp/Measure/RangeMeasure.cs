using HierarchicalTilingApp.ArrayPartition;
using HierarchicalTilingApp.SumOfSquares;
using HierarchicalTilingApp.Transformation;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.Measure
{
    public class RangeMeasure : SimilarityMeasure<RangeAuxData>, IAverageMeasure
    {
        public RangeMeasure(RangeAuxData auxData, Transformator transformator)
            : base(auxData, transformator)
        {
        }

        public double averageAllMeasures(Coords[] partition)
        {
            double result = 0.0;
            int maxRange = determineMaxRange();
            for (int rangeIdx = 1; rangeIdx <= maxRange; rangeIdx++)
            {
                AuxData.Range = rangeIdx;
                result += computeMeasure(partition);
            }
            result /= (double)maxRange;
            return result;
        }

        private int determineMaxRange()
        {
            int result;
            if (AuxData.SpaceDimension == 2)
            {
                result = (int)(Math.Pow(AuxData.HistogramResolution, 2) + AuxData.HistogramResolution / 2) - 1;
            }
            else if (AuxData.SpaceDimension == 3)
            {
                double temp = Math.Pow(AuxData.HistogramResolution, 3) / 6.0;
                temp += Math.Pow(AuxData.HistogramResolution, 2) / 2.0;
                temp += (double)(AuxData.HistogramResolution) / 3.0;
                result = (int)temp - 1;
            }
            else if (AuxData.SpaceDimension == 4)
            {
                double temp = Math.Pow(AuxData.HistogramResolution, 4) / 24.0;
                temp += Math.Pow(AuxData.HistogramResolution, 3) / 4.0;
                temp += (11.0 / 24.0) * Math.Pow(AuxData.HistogramResolution, 2);
                temp += (double)(AuxData.HistogramResolution) / 4.0;
                result = (int)temp - 1;
            }
            else
            {
                // TODO: (sum_{i_1=1}^{n}sum_{i_2=1}^{i_1}...sum_{i_{D-1}=1}^{i_{D-2}}i_{D-1}) - 1 = X
                // general formula can be given for X based on 
                // https://en.wikipedia.org/wiki/Faulhaber%27s_formula#Faulhaber_polynomials
                throw new NotImplementedException();
            }
            return result;
        }

        public override double computeMeasureForBin(int[] indicesArrayOfBin, int[] indicesArrayOfRegion)
        {
            double measureForBin = 0.0;
            int binValue = (int)AuxData.Histogram.GetValue(indicesArrayOfBin);
            if (binValue > 0)
            {
                Shell[] currentShells = new Shell[AuxData.Range];
                Array.Copy(AuxData.Shells, currentShells, AuxData.Range);
                var dictOfShells = transformator.convertIntPairsOfShellsToListOfIdxArrays(
                        AuxData.HistogramResolution, indicesArrayOfBin, currentShells);
                measureForBin = iterateOverShells(indicesArrayOfRegion, measureForBin, dictOfShells);
            }
            return measureForBin;
        }

        private double iterateOverShells(int[] indicesArrayOfRegion, double measureForBin, Dictionary<int, List<int[]>> dictOfShells)
        {
            int pointsInServer = 0;
            int pointsOutServer = 0;
            foreach (var shellIdx in dictOfShells.Keys)
            {
                List<int[]> idxArraysOnCurrentShell = dictOfShells[shellIdx];
                foreach (var idxArray in idxArraysOnCurrentShell)
                {
                    int currentBinValue = (int)AuxData.Histogram.GetValue(idxArray);
                    if (isIdxArrayInTheRegion(idxArray, indicesArrayOfRegion))
                    {
                        pointsInServer += currentBinValue;
                    }
                    else
                    {
                        pointsOutServer += currentBinValue;
                    }
                }
                measureForBin = (double)pointsInServer / (double)(pointsInServer + pointsOutServer);
            }
            return measureForBin;
        }
    }
}
