using HierarchicalTilingApp.ArrayPartition;
using HierarchicalTilingApp.SumOfSquares;
using HierarchicalTilingApp.Transformation;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.Measure
{
    public class KNNMeasure : SimilarityMeasure<KNNAuxData>
    {
        public KNNMeasure(KNNAuxData auxData, Transformator transformator)
            : base(auxData, transformator)
        {
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
    }
}
