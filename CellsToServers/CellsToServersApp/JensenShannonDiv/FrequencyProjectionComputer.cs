using CellsToServersApp.ArrayPartition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellsToServersApp.JensenShannonDiv
{
    public class FrequencyProjectionComputer
    {
        private Array array;
        private Array heftArray;
        private IndexTransformator transformator;
        private double[] cellFillScale;
        private int scaleNumber;
        private int spaceDimension;
        private int histogramResolution;

        public FrequencyProjectionComputer(Array array, Array heftArray, IndexTransformator transformator,
            int spaceDimension, int histogramResolution, int pointNO, int scaleNumber)
        {
            this.array = array;
            this.heftArray = heftArray;
            this.transformator = transformator;
            this.spaceDimension = spaceDimension;
            this.histogramResolution = histogramResolution;
            this.scaleNumber = scaleNumber;
            this.cellFillScale = new double[scaleNumber];
            double step = (double)pointNO / (double)scaleNumber;
            for (int idx = 0; idx < scaleNumber; idx++)
            {
                this.cellFillScale[idx] = idx * step;
            }
        }

        public Array createFrequencyArray()
        {
            //Array[] frequencyProjectionArray = new Array[spaceDimension];
            //for (int idx = 0; idx < spaceDimension; idx++)
            //{
            //    int[] lengthsFrequencyArray = new int[2 * spaceDimension];
            //    for (int subIdx = 0; subIdx < 2 * spaceDimension; subIdx++)
            //    {
            //        lengthsFrequencyArray[subIdx] = histogramResolution;
            //    }
            //    frequencyProjectionArray[idx] = Array.CreateInstance(typeof(double[]), lengthsFrequencyArray);
            //}

            int[] lengthsFrequencyArray = new int[2 * spaceDimension + 1];
            lengthsFrequencyArray[0] = spaceDimension;
            for (int idx = 1; idx < 2 * spaceDimension + 1; idx++)
            {
               lengthsFrequencyArray[idx] = histogramResolution;
            }
            Array frequencyProjectionArray = Array.CreateInstance(typeof(double[]), lengthsFrequencyArray);
            fillFrequencyProjectionArray(frequencyProjectionArray);
            return frequencyProjectionArray;
        }

        private void fillFrequencyProjectionArray(Array frequencyProjectionArray)
        {
            int cellNO = (int)Math.Pow(histogramResolution, spaceDimension);
            int[] outerIndicesArray = new int[spaceDimension];
            int[] innerIndicesArray = new int[spaceDimension];
            for (int outerCellIdx = 0; outerCellIdx < cellNO; outerCellIdx++)
            {
                transformator.transformCellIdxToIndicesArray(histogramResolution, outerIndicesArray, outerCellIdx);
                for (int innerCellIdx = outerCellIdx; innerCellIdx < cellNO; innerCellIdx++)
                {
                    transformator.transformCellIdxToIndicesArray(histogramResolution, innerIndicesArray, innerCellIdx);
                    int[] frequencyArrayIndices;
                    int cells;
                    bool validFrequencyArrayIndeces = transformator.mergeIndicesArrays(spaceDimension,
                        outerIndicesArray, innerIndicesArray, out frequencyArrayIndices, out cells);
                    if (validFrequencyArrayIndeces)
                    {
                        for (int splitDimIdx = 0; splitDimIdx < spaceDimension; splitDimIdx++)
                        {
                            double[] frequencies = new double[scaleNumber];
                            fillFrequencyProjectionArrayForSplitDim(frequencyProjectionArray, cellNO, 
                                frequencyArrayIndices, frequencies, splitDimIdx);
                        }
                    }
                }
            }
        }

        private void fillFrequencyProjectionArrayForSplitDim(Array frequencyProjectionArray, int cellNO,
            int[] frequencyArrayIndices, double[] frequencies, int splitDimIdx)
        {
            int lowerBoundForSplitDim = frequencyArrayIndices[2 * splitDimIdx];
            int upperBoundForSplitDim = frequencyArrayIndices[2 * splitDimIdx + 1];
            int[] projectionArray = transformator.aggregateProjectedIndicesArrays(spaceDimension, histogramResolution,
                cellNO, lowerBoundForSplitDim, upperBoundForSplitDim, splitDimIdx, frequencyArrayIndices, heftArray);
            int cellsInProjectedRegion = (upperBoundForSplitDim - lowerBoundForSplitDim + 1);
            for (int projectionArrayIdx = 0; projectionArrayIdx < cellsInProjectedRegion; projectionArrayIdx++)
            {
                double projectedCellValue = (double)projectionArray[projectionArrayIdx];
                for (int frequenciesIdx = 0; frequenciesIdx < scaleNumber; frequenciesIdx++)
                {
                    if (((frequenciesIdx < scaleNumber - 1)
                        && (projectedCellValue < cellFillScale[frequenciesIdx + 1])
                        && (cellFillScale[frequenciesIdx] <= projectedCellValue))
                        || (frequenciesIdx == scaleNumber - 1))
                    {
                        frequencies[frequenciesIdx] += (1.0 / (double)cellsInProjectedRegion);
                        break;
                    }
                }
            }
            int[] frequencyExtendedArrayIndices = new int[2 * spaceDimension + 1];
            frequencyArrayIndices.CopyTo(frequencyExtendedArrayIndices, 1);
            frequencyExtendedArrayIndices[0] = splitDimIdx;
            frequencyProjectionArray.SetValue(frequencies, frequencyExtendedArrayIndices);
        }
    }
}
