using MeasureApp.SumOfSquares;
using MeasureApp.Transformation;
using System;
using System.Collections.Generic;

namespace MeasureApp.Measure
{
    public class KNNMeasure : SimilarityMeasure<KNNAuxData>
    {
        public KNNMeasure(KNNAuxData auxData, Transformator transformator)
            : base(auxData, transformator)
        {
        }

        public override double computeMeasureForBin(int[] indicesArrayOfBin, List<int> binList)
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
                    var dictOfShells = transformator.convertIntPairsOfShellsToListOfIdxArrays(
                        AuxData.HistogramResolution, indicesArrayOfBin, AuxData.Shells);
                    iterateOverShells(binList, kNN, ref nnInServer, ref nnOutserver, dictOfShells);
                }
                //nnInServer should not be greater than (kNN - nnOutserver) because nnOutserver has been 'commited':
                nnInServer = (nnInServer <= kNN - nnOutserver) ? nnInServer : kNN - nnOutserver;
                measureForBin = (double)nnInServer / (double)kNN;
            }
            return measureForBin;
        }

        private void iterateOverShells(List<int> binList, int kNN, ref int nnInServer, ref int nnOutserver, 
            Dictionary<int, List<int[]>> dictOfShells)
        {
            foreach (var shellIdx in dictOfShells.Keys)
            {
                int tmpNNOutServer = 0;
                List<int[]> idxArraysOnCurrentShell = dictOfShells[shellIdx];
                updateNNCountsWithBinsOnCurrentShell(binList, ref nnInServer, ref tmpNNOutServer,
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

        private void updateNNCountsWithBinsOnCurrentShell(List<int> binList, ref int nnInServer, 
            ref int tmpNNOutServer, List<int[]> idxArraysOnCurrentShell)
        {
            foreach (var idxArray in idxArraysOnCurrentShell)
            {
                int currentBinValue = (int)AuxData.Histogram.GetValue(idxArray);
                if (isIdxArrayInTheRegion(idxArray, binList))
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
