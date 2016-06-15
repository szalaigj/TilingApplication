using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnuthRuleBinNumsApp.Utils
{
    public class HistogramComputer
    {
        /// <summary>
        /// This method returns the bin hefts in the order in which the highest dimension-related index 
        /// is the lowest index of the histogram.
        /// 
        /// E.g.: (space dimension : 3, histogram resolution : 2)
        /// If the usual histogram is the following ...
        /// Level 0:
        /// 1 1
        /// 2 3
        /// Level 1:
        /// 2 3
        /// 4 1
        /// ... then the output of this method will be {1, 1, 2, 3, 2, 3, 4, 1}.
        /// </summary>
        public int[] buildHistogram(int histogramResolution, List<DataRow> data, int spaceDimension,
            double[] minElems, double[] maxElems)
        {
            int[] binHefts = new int[(int)Math.Pow(histogramResolution, spaceDimension)];
            foreach (var dataRow in data)
            {
                buildHistogramForDataRowTuple(histogramResolution, spaceDimension, minElems, maxElems, 
                    binHefts, dataRow.Tuple);
            }
            return binHefts;
        }

        public void buildHistogramForDataRowTuple(int histogramResolution, int spaceDimension, 
            double[] minElems, double[] maxElems, int[] binHefts, double[] dataRowTuple)
        {
            int binHeftIdx = 0;
            for (int coordIdx = 0; coordIdx < spaceDimension; coordIdx++)
            {
                int normedCoordIdx;
                if (dataRowTuple[coordIdx] == maxElems[coordIdx])
                    // the expression in else branch would be histogramResolution
                    // so we need to change assignment in this case
                    normedCoordIdx = histogramResolution - 1;
                else
                    normedCoordIdx = (int)(((dataRowTuple[coordIdx] - minElems[coordIdx])
                    / (maxElems[coordIdx] - minElems[coordIdx])) * histogramResolution);
                binHeftIdx += normedCoordIdx * (int)Math.Pow(histogramResolution, coordIdx);
            }
            binHefts[binHeftIdx]++;
        }
    }
}
