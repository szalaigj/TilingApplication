using SpectralClusteringApplication.SumOfSquares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectralClusteringApplication
{
    public class KNNMeasure
    {
        private IndexTransformator transformator;
        private Array histogram;
        private Shell[] shells;
        private int spaceDimension;
        private int histogramResolution;
        private int serverNO;
        private int pointNO;
        private int kNN;

        public KNNMeasure(IndexTransformator transformator, Array histogram, Shell[] shells, int spaceDimension, 
            int histogramResolution, int serverNO, int pointNO, int kNN)
        {
            this.transformator = transformator;
            this.histogram = histogram;
            this.shells = shells;
            this.spaceDimension = spaceDimension;
            this.histogramResolution = histogramResolution;
            this.serverNO = serverNO;
            this.pointNO = pointNO;
            this.kNN = kNN;
        }

        public double computeMeasure(SpectralTreeNode[] spectralTreeLeaves)
        {
            double measure = 0.0;
            foreach (var spectralTreeLeaf in spectralTreeLeaves)
            {
                measure += computeMeasureForRegion(spectralTreeLeaf);
            }
            measure = measure / (double)serverNO;
            return measure;
        }

        public double computeMeasureForRegion(SpectralTreeNode spectralTreeLeaf)
        {
            double measureForRegion = 0.0;
            int binNOInRegionWithoutZeroHeft = 0;
            foreach (var vertexIdx in spectralTreeLeaf.VertexList)
            {
                int[] indicesArrayOfBin = new int[spaceDimension];
                transformator.transformCellIdxToIndicesArray(histogramResolution, indicesArrayOfBin, vertexIdx);
                int binValue = (int)histogram.GetValue(indicesArrayOfBin);
                if (binValue > 0)
                {
                    binNOInRegionWithoutZeroHeft++;
                    measureForRegion += computeMeasureForBin(indicesArrayOfBin, spectralTreeLeaf);
                }
            }
            measureForRegion = measureForRegion / (double)binNOInRegionWithoutZeroHeft;
            return measureForRegion;
        }

        public double computeMeasureForBin(int[] indicesArrayOfBin, SpectralTreeNode spectralTreeLeaf)
        {
            double measureForBin = 0.0;
            int binValue = (int)histogram.GetValue(indicesArrayOfBin);
            int currentKNN = (kNN < pointNO) ? kNN : pointNO - 1;
                int nnInServer = binValue - 1;
                int nnOutserver = 0;
                if (currentKNN - binValue + 1 > 0)
                {
                    Shell[] currentShells = new Shell[currentKNN - binValue + 1];
                    Array.Copy(shells, currentShells, currentKNN - binValue + 1);
                    var dictOfShells = transformator.convertIntPairsOfShellsToListOfIdxArrays(
                        histogramResolution, indicesArrayOfBin, shells);
                    iterateOverShells(spectralTreeLeaf, currentKNN, ref nnInServer, ref nnOutserver, dictOfShells);
                }
                //nnInServer should not be greater than (kNN - nnOutserver) because nnOutserver has been 'commited':
                nnInServer = (nnInServer <= currentKNN - nnOutserver) ? nnInServer : currentKNN - nnOutserver;
                measureForBin = (double)nnInServer / (double)currentKNN;
            return measureForBin;
        }

        private void iterateOverShells(SpectralTreeNode spectralTreeLeaf, int kNN, ref int nnInServer, ref int nnOutserver,
            Dictionary<int, List<int[]>> dictOfShells)
        {
            foreach (var shellIdx in dictOfShells.Keys)
            {
                int tmpNNOutServer = 0;
                List<int[]> idxArraysOnCurrentShell = dictOfShells[shellIdx];
                updateNNCountsWithBinsOnCurrentShell(spectralTreeLeaf, ref nnInServer, ref tmpNNOutServer,
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

        private void updateNNCountsWithBinsOnCurrentShell(SpectralTreeNode spectralTreeLeaf, ref int nnInServer,
            ref int tmpNNOutServer, List<int[]> idxArraysOnCurrentShell)
        {
            foreach (var idxArray in idxArraysOnCurrentShell)
            {
                int currentBinValue = (int)histogram.GetValue(idxArray);
                if (isIdxArrayInTheSpectralTreeLeaf(idxArray, spectralTreeLeaf))
                {
                    nnInServer += currentBinValue;
                }
                else
                {
                    tmpNNOutServer += currentBinValue;
                }
            }
        }

        private bool isIdxArrayInTheSpectralTreeLeaf(int[] idxArray, SpectralTreeNode spectralTreeLeaf)
        {
            bool result = false;
            int targetVertexIdx = transformator.transformIndicesArrayToCellIdx(histogramResolution, idxArray);
            foreach (var vertexIdx in spectralTreeLeaf.VertexList)
            {
                if (targetVertexIdx == vertexIdx)
                {
                    result = true;
                }
            }
            return result;
        }

    }
}
