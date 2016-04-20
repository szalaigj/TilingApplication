using System;
using System.Collections.Generic;
using KnuthRuleBinNumsApp.Utils;

namespace KnuthRuleBinNumsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            InputParser inputParser = new InputParser();
            HistogramComputer histogramComputer = new HistogramComputer();
            KnuthPosteriorComputer knuthPosteriorComputer = new KnuthPosteriorComputer();
            int spaceDimension, pointNO;
            double[] minElems, maxElems;
            List<DataRow> data = inputParser.parseInputFile(out spaceDimension, out pointNO, out minElems, out maxElems);
            for (int histogramResolution = 2; histogramResolution <= Math.Sqrt(pointNO); histogramResolution++)
            {
                int[] binHefts = histogramComputer.buildHistogram(histogramResolution, data, spaceDimension, 
                    minElems, maxElems);
                double posteriorValue = knuthPosteriorComputer.evaluate(Math.Pow(histogramResolution, spaceDimension), 
                    pointNO, binHefts);
                Console.WriteLine("Histogram resolution: {0}, posterior value: {1}", 
                    histogramResolution, posteriorValue);
            }
            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }
    }
}
