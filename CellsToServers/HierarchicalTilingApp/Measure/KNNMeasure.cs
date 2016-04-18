using HierarchicalTilingApp.ArrayPartition;
using HierarchicalTilingApp.SumOfSquares;
using HierarchicalTilingApp.Transformation;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.Measure
{
    public class KNNMeasure : BaseMeasure<KNNAuxData>
    {
        public KNNMeasure(KNNAuxData auxData, Transformator transformator)
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

        public override double computeMeasureForBin(int[] indicesArrayOfBin, int[] indicesArrayOfRegion)
        {
            double measureForBin = 0.0;
            int binValue = (int)AuxData.Histogram.GetValue(indicesArrayOfBin);
            if (binValue > 0)
            {
                int kNN = (AuxData.KNN < AuxData.PointNO) ? AuxData.KNN : AuxData.PointNO - 1;
                int nnInServer = binValue - 1;
                int nnOutserver = 0;
                if (kNN - binValue + 1 > 0)
                {
                    Shell[] currentShells = new Shell[kNN - binValue + 1];
                    Array.Copy(AuxData.Shells, currentShells, kNN - binValue + 1);
                    var dictOfShells = transformator.convertIntPairsOfShellsToListOfIdxArrays(
                        AuxData.HistogramResolution, indicesArrayOfBin, AuxData.Shells);
                    iterateOverShells(indicesArrayOfRegion, kNN, ref nnInServer, ref nnOutserver, dictOfShells);
                }
                //nnInServer should not be greater than (kNN - nnOutserver) because nnOutserver has been 'commited':
                nnInServer = (nnInServer <= kNN - nnOutserver) ? nnInServer : kNN - nnOutserver;
                measureForBin = (double)nnInServer / (double)kNN;
            }
            return measureForBin;
        }

        private void iterateOverShells(int[] indicesArrayOfRegion, int kNN, ref int nnInServer, ref int nnOutserver, 
            Dictionary<int, List<int[]>> dictOfShells)
        {
            foreach (var shellIdx in dictOfShells.Keys)
            {
                int tmpNNOutServer = 0;
                List<int[]> idxArraysOnCurrentShell = dictOfShells[shellIdx];
                updateNNCountsWithBinsOnCurrentShell(indicesArrayOfRegion, ref nnInServer, ref tmpNNOutServer,
                    idxArraysOnCurrentShell);
                if (nnInServer + nnOutserver >= kNN)
                {
                    break;
                }
                else
                {
                    if (nnInServer + nnOutserver + tmpNNOutServer >= kNN)
                    {
                        nnOutserver = kNN - nnInServer;
                        break;
                    }
                    else
                    {
                        nnOutserver += tmpNNOutServer;
                    }
                }
            }
        }

        private void updateNNCountsWithBinsOnCurrentShell(int[] indicesArrayOfRegion, ref int nnInServer, 
            ref int tmpNNOutServer, List<int[]> idxArraysOnCurrentShell)
        {
            foreach (var idxArray in idxArraysOnCurrentShell)
            {
                int currentBinValue = (int)AuxData.Histogram.GetValue(idxArray);
                if (isIdxArrayInTheRegion(idxArray, indicesArrayOfRegion))
                {
                    nnInServer += currentBinValue;
                }
                else
                {
                    tmpNNOutServer += currentBinValue;
                }
            }
        }

        private bool isIdxArrayInTheRegion(int[] idxArray, int[] indicesArrayOfRegion)
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
