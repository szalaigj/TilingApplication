using System;
using System.Collections.Generic;
using KnuthRuleBinNumsApp.Utils;
using System.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using MathNet.Numerics;

namespace KnuthRuleBinNumsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            InputParser inputParser = new InputParser();
            HistogramComputer histogramComputer = new HistogramComputer();
            KnuthPosteriorComputer knuthPosteriorComputer = new KnuthPosteriorComputer();
            LMethod lMethod = new LMethod();
            FurthestPointFromLineMethod furthestPointFromLineMethod = new FurthestPointFromLineMethod();
            string filename;
            int spaceDimension, pointNO;
            double[] minElems, maxElems;
            List<DataRow> data = inputParser.parseInputFile(out filename, out spaceDimension, out pointNO, 
                out minElems, out maxElems);
            int serverNO = inputParser.parseServerNO();
            double posteriorSum = 0.0;
            int upperBoundOfPostVals = (int)(Math.Sqrt(pointNO));
            //int upperBoundOfPostVals = 100;
            double[] posteriorValues = new double[upperBoundOfPostVals - 1];
            // The central difference approximations of second derivative of posterior:
            double[] centralDiffs = new double[upperBoundOfPostVals - 1];
            int initialHistRes = 2;
            for (int histogramResolution = initialHistRes; histogramResolution <= upperBoundOfPostVals; histogramResolution++)
            {
                int[] binHefts = histogramComputer.buildHistogram(histogramResolution, data, spaceDimension, 
                    minElems, maxElems);
                //writeOutBinHefts(binHefts, histogramResolution);
                writeOutInputSizes(inputParser, filename, spaceDimension, histogramResolution, serverNO, binHefts);
                double posteriorValue = knuthPosteriorComputer.evaluate(Math.Pow(histogramResolution, spaceDimension), 
                    pointNO, binHefts);
                int idx = histogramResolution - initialHistRes;
                posteriorValues[idx] = posteriorValue;
                determineCentralDiff(upperBoundOfPostVals, posteriorValues, centralDiffs, initialHistRes, histogramResolution, idx);
                Console.WriteLine("Histogram resolution: {0}, posterior value: {1}, central diff: {2}", 
                    histogramResolution, posteriorValue, centralDiffs[idx]);
                posteriorSum += posteriorValue;
            }
            Console.WriteLine("Posterior mean: {0}", (posteriorSum) / ((double)posteriorValues.Length));
            int maxCentralDiffIdx = determineMaxCentralDiffIdx(pointNO, centralDiffs);
            Console.WriteLine("Maximum central difference: {0}", maxCentralDiffIdx + initialHistRes);
            double[] xData = Enumerable.Range(initialHistRes, upperBoundOfPostVals - 1).Select(x => (double)x).ToArray();
            int kneePointIdx = lMethod.findBestKneePoint(xData, posteriorValues, xData.Length);
            Console.WriteLine("Knee point (L method): {0}", kneePointIdx + initialHistRes);
            kneePointIdx = lMethod.iterativeRefinementOfTheKnee(xData, posteriorValues);
            Console.WriteLine("Refined knee point (L method): {0}", kneePointIdx + initialHistRes);
            kneePointIdx = furthestPointFromLineMethod.findBestKneePoint(xData, posteriorValues);
            Console.WriteLine("Knee point ('furthest point from line' method): {0}", kneePointIdx + initialHistRes);
            writeOutPosteriorValues(posteriorValues);
            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }

        private static void determineCentralDiff(int upperBoundOfPostVals, double[] posteriorValues, double[] centralDiffs, int initialHistRes, int histogramResolution, int idx)
        {
            if ((histogramResolution == initialHistRes) || (histogramResolution == upperBoundOfPostVals))
            {
                centralDiffs[idx] = 0.0;
            }
            else
            {
                centralDiffs[idx] = posteriorValues[idx - 1] - 2 * posteriorValues[idx] + posteriorValues[idx + 1];
            }
        }

        private static int determineMaxCentralDiffIdx(int pointNO, double[] centralDiffs)
        {
            int maxCentralDiffIdx = 0;
            double maxCentralDiff = 0.0;
            for (int centralDiffIdx = 0; centralDiffIdx < (int)(Math.Sqrt(pointNO)) - 1; centralDiffIdx++)
            {
                if (Math.Abs(centralDiffs[centralDiffIdx]) > maxCentralDiff)
                {
                    maxCentralDiff = Math.Abs(centralDiffs[centralDiffIdx]);
                    maxCentralDiffIdx = centralDiffIdx;
                }
            }
            return maxCentralDiffIdx;
        }

        private static void writeOutBinHefts(int[] binHefts, int histogramResolution)
        {
            StringBuilder strBldr = new StringBuilder();
            foreach (var binHeft in binHefts)
            {
                strBldr.Append(binHeft).Append(" ");
            }
            string hstgramOutput = @"c:\temp\data\hstgram_for_input_res_" + histogramResolution + ".dat";
            System.IO.File.WriteAllText(hstgramOutput, strBldr.ToString());
        }

        private static void writeOutInputSizes(InputParser inputParser, string filename, int spaceDimension, 
            int histogramResolution, int serverNO, int[] binHefts)
        {
            StringBuilder strBldr = new StringBuilder();
            strBldr.AppendLine(spaceDimension.ToString());
            strBldr.AppendLine(histogramResolution.ToString());
            strBldr.AppendLine(serverNO.ToString());
            for (int binIdx = 0; binIdx < binHefts.Length - 1; binIdx++)
            {
                strBldr.Append(binHefts[binIdx]).Append(" ");
            }
            strBldr.Append(binHefts[binHefts.Length - 1]);
            filename = Path.GetFileNameWithoutExtension(filename);
            string inputSizesOutput = @"c:\temp\data\" + filename + "_input_sizes_serv_" + serverNO 
                + "_res_" + histogramResolution + ".dat";
            System.IO.File.WriteAllText(inputSizesOutput, strBldr.ToString());
        }

        private static void writeOutPosteriorValues(double[] posteriorValues)
        {
            StringBuilder strBldr = new StringBuilder();
            CultureInfo ci = new CultureInfo("en-GB", true);
            foreach (var posteriorValue in posteriorValues)
            {
                strBldr.AppendFormat(ci, "{0}", posteriorValue).AppendLine();
                //strBldr.Append(posteriorValue).AppendLine();
            }
            string posteriorValuesOutput = @"c:\temp\data\posterior_values.dat";
            System.IO.File.WriteAllText(posteriorValuesOutput, strBldr.ToString());
        }
    }
}
