using HistoSegmentationApp.ArrayPartition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistoSegmentationApp.JensenShannonDiv
{
    public class FrequencyComputer
    {
        private Array array;
        private IndexTransformator transformator;
        private double[] cellFillScale;
        private int scaleNumber;

        public FrequencyComputer(Array array, IndexTransformator transformator, int cellMaxValue, 
            int scaleNumber)
        {
            this.array = array;
            this.transformator = transformator;
            this.scaleNumber = scaleNumber;
            this.cellFillScale = new double[scaleNumber];
            double step = (double)cellMaxValue / (double)scaleNumber;
            for (int idx = 0; idx < scaleNumber; idx++)
            {
                this.cellFillScale[idx] = idx * step;
            }
        }

        public Array createFrequencyArray(int spaceDimension, int histogramResolution)
        {
            int[] lengthsFrequencyArray = new int[2 * spaceDimension];
            for (int idx = 0; idx < 2 * spaceDimension; idx++)
            {
                lengthsFrequencyArray[idx] = histogramResolution;
            }
            Array frequencyArray = Array.CreateInstance(typeof(double[]), lengthsFrequencyArray);
            fillFrequencyArray(spaceDimension, histogramResolution, frequencyArray);
            return frequencyArray;
        }

        private void fillFrequencyArray(int spaceDimension, int histogramResolution, Array frequencyArray)
        {
            int cellNO = (int)Math.Pow(histogramResolution, spaceDimension);
            int[] outerIndicesArray = new int[spaceDimension];
            int[] innerIndicesArray = new int[spaceDimension];
            int[] windowIndicesArray = new int[spaceDimension];
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
                        double[] frequencies = determineFrequenciesForRegion(spaceDimension, histogramResolution,
                            outerIndicesArray, innerIndicesArray, windowIndicesArray, outerCellIdx, innerCellIdx,
                            cells);
                        frequencyArray.SetValue(frequencies, frequencyArrayIndices);
                    }
                }
            }
        }

        private double[] determineFrequenciesForRegion(int spaceDimension, int histogramResolution, 
            int[] outerIndicesArray, int[] innerIndicesArray, int[] windowIndicesArray,
            int outerCellIdx, int innerCellIdx, int cellsInRegion)
        {
            double[] frequencies = new double[scaleNumber];
            for (int windowIdx = outerCellIdx; windowIdx <= innerCellIdx; windowIdx++)
            {
                transformator.transformCellIdxToIndicesArray(histogramResolution, windowIndicesArray,
                    windowIdx);
                bool validWindowArrayIndices = transformator.validateIndicesArrays(spaceDimension,
                    outerIndicesArray, innerIndicesArray, windowIndicesArray);
                if (validWindowArrayIndices)
                {
                    double cellValue = (int)array.GetValue(windowIndicesArray);
                    for (int frequenciesIdx = 0; frequenciesIdx < scaleNumber; frequenciesIdx++)
                    {
                        if (((frequenciesIdx < scaleNumber - 1)
                            && (cellValue < cellFillScale[frequenciesIdx + 1])
                            && (cellFillScale[frequenciesIdx] <= cellValue))
                            || (frequenciesIdx == scaleNumber - 1))
                        {
                            frequencies[frequenciesIdx] += (1.0 / (double)cellsInRegion);
                            break;
                        }
                    }
                }
            }
            return frequencies;
        }
    }
}
