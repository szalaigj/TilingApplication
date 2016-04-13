using System;
using System.Collections.Generic;

namespace BinsToServersIntLPApp.Histogram
{
    public class BinsCreator
    {
        private IndexTransformator transformator;

        public BinsCreator(IndexTransformator transformator)
        {
            this.transformator = transformator;
        }

        public Bin[] createBinsFromHistogram(int spaceDimension, int histogramResolution, Array array)
        {
            int cellNO = (int)Math.Pow(histogramResolution, spaceDimension);
            Bin[] bins = new Bin[cellNO];
            int heft;
            for (int cellIdx = 0; cellIdx < cellNO; cellIdx++)
            {
                int[] indicesArray = new int[spaceDimension];
                transformator.transformCellIdxToIndicesArray(histogramResolution, indicesArray, cellIdx);
                heft = (int)array.GetValue(indicesArray);
                bins[cellIdx] = new Bin
                {
                    IndicesArray = indicesArray,
                    Heft = heft
                };
            }
            return bins;
        }
    }
}
