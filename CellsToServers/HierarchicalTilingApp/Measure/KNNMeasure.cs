﻿using HierarchicalTilingApp.ArrayPartition;
using HierarchicalTilingApp.Transformation;
using System;
using System.Collections.Generic;

namespace HierarchicalTilingApp.Measure
{
    public class KNNMeasure : BaseMeasure
    {
        private Transformator transformator;

        public KNNMeasure(DefaultAuxData auxData, Transformator transformator)
        {
            this.transformator = transformator;
            this.AuxData = auxData;
        }

        public override double computeMeasure(Coords[] partition)
        {
            double measure = 0.0;
            foreach (var coords in partition)
            {
                int[] indicesArrayOfRegion = 
                    transformator.determineIndicesArray(AuxData.SpaceDimension, coords.ExtendedIndicesArray);
                measure += computeMeasureForRegion(indicesArrayOfRegion);
            }
            measure = measure / (double)AuxData.ServerNO;
            return measure;
        }

        public override double computeMeasureForRegion(int[] indicesArrayOfRegion)
        {
            double measureForRegion = 0.0;
            if (AuxData.SpaceDimension == 2)
            {
                int[] indicesArrayOfBin = new int[2];
                int binNOInRegionWithoutZeroHeft = 0;
                for (int x = indicesArrayOfRegion[0]; x <= indicesArrayOfRegion[1]; x++)
                {
                    for (int y = indicesArrayOfRegion[2]; y <= indicesArrayOfRegion[3]; y++)
                    {
                        indicesArrayOfBin[0] = x;
                        indicesArrayOfBin[1] = y;
                        int binValue = (int)AuxData.Histogram.GetValue(indicesArrayOfBin);
                        if (binValue > 0)
                        {
                            binNOInRegionWithoutZeroHeft++;
                            measureForRegion += computeMeasureForBin(indicesArrayOfBin, indicesArrayOfRegion);
                        }
                    }
                }
                measureForRegion = measureForRegion / (double)binNOInRegionWithoutZeroHeft;
                return measureForRegion;
            }
            else
            {
                // TODO: implementation for higher dimension case
                throw new NotImplementedException();
                //int cellNO = (int)Math.Pow(AuxData.HistogramResolution, AuxData.SpaceDimension);
                //int[] indicesArrayOfBin = new int[AuxData.SpaceDimension];
                //for (int movingIdx = 0; movingIdx < cellNO; movingIdx++)
                //{
                //    transformator.transformCellIdxToIndicesArray(AuxData.HistogramResolution, indicesArrayOfBin, movingIdx);
                //    bool validMovingIndicesArray = transformator.validateIndicesArrays(AuxData.SpaceDimension,
                //        indicesArrayOfRegion, indicesArrayOfBin);
                //    if (validMovingIndicesArray)
                //    {
                //        measureForRegion += computeMeasureForBin(indicesArrayOfBin);
                //    }
                //}
            }
        }

        public override double computeMeasureForBin(int[] indicesArrayOfBin, int[] indicesArrayOfRegion)
        {
            if (AuxData.SpaceDimension == 2)
            {
                return computeMeasureForBinTwoDimCase(indicesArrayOfBin, indicesArrayOfRegion);
            }
            else
            {
                // TODO: implementation for higher dimension case
                throw new NotImplementedException();
            }
        }

        private double computeMeasureForBinTwoDimCase(int[] indicesArrayOfBin, int[] indicesArrayOfRegion)
        {
            double measureForBin = 0.0;
            int binValue = (int)AuxData.Histogram.GetValue(indicesArrayOfBin);
            if (binValue > 0)
            {
                int kNN = (AuxData.KNN < AuxData.PointNO) ? AuxData.KNN : AuxData.PointNO - 1;
                int nnInServer = binValue - 1;
                int nnOutserver = 0;
                var dictOfShells = transformator.determineShellIdxArraysInTwoDimSpace(AuxData.HistogramResolution,
                    indicesArrayOfBin, kNN - binValue + 1);
                iterateOverShells(indicesArrayOfRegion, kNN, ref nnInServer, ref nnOutserver, dictOfShells);
                //nnInServer should not be greater than (kNN - nnOutserver) because nnOutserver has been 'commited':
                nnInServer = (nnInServer <= kNN - nnOutserver) ? nnInServer : kNN - nnOutserver;
                measureForBin = (double)nnInServer / (double)kNN;
            }
            return measureForBin;
        }

        private void iterateOverShells(int[] indicesArrayOfRegion, int kNN, ref int nnInServer, ref int nnOutserver, 
            Dictionary<int, List<int[]>> dictOfShells)
        {
            for (int shellIdx = 0; shellIdx < dictOfShells.Count; shellIdx++)
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

        private static bool isIdxArrayInTheRegion(int[] idxArray, int[] indicesArrayOfRegion)
        {
            return (indicesArrayOfRegion[0] <= idxArray[0]) && (idxArray[0] <= indicesArrayOfRegion[1]) && 
                (indicesArrayOfRegion[2] <= idxArray[1]) && (idxArray[1] <= indicesArrayOfRegion[3]);
        }
    }
}