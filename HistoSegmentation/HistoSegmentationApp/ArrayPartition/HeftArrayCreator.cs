using System;

namespace HistoSegmentationApp.ArrayPartition
{
    public class HeftArrayCreator
    {
        private IndexTransformator transformator;

        public HeftArrayCreator(IndexTransformator transformator)
        {
            this.transformator = transformator;
        }
        public Array createHeftArray(int spaceDimension, int histogramResolution, Array array)
        {
            int[] lengthsHeftArray = new int[2 * spaceDimension];
            for (int idx = 0; idx < 2 * spaceDimension; idx++)
            {
                lengthsHeftArray[idx] = histogramResolution;
            }
            Array heftArray = Array.CreateInstance(typeof(int), lengthsHeftArray);
            fillHeftArray(spaceDimension, histogramResolution, array, heftArray);
            return heftArray;
        }

        private void fillHeftArray(int spaceDimension, int histogramResolution, Array array, Array heftArray)
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
                    int[] heftArrayIndeces;
                    int cellPoints;
                    bool validHeftArrayIndeces = transformator.mergeIndicesArrays(spaceDimension, outerIndicesArray,
                        innerIndicesArray, out heftArrayIndeces, out cellPoints);
                    if (validHeftArrayIndeces)
                    {
                        int cellValue = 0;
                        for (int windowIdx = outerCellIdx; windowIdx <= innerCellIdx; windowIdx++)
                        {
                            transformator.transformCellIdxToIndicesArray(histogramResolution, windowIndicesArray, windowIdx);
                            bool validSummableArrayIndeces = transformator.validateIndicesArrays(spaceDimension,
                                outerIndicesArray, innerIndicesArray, windowIndicesArray);
                            if (validSummableArrayIndeces)
                            {
                                cellValue += (int)array.GetValue(windowIndicesArray);
                            }
                        }
                        heftArray.SetValue(cellValue, heftArrayIndeces);
                    }
                }
            }
        }
    }
}
