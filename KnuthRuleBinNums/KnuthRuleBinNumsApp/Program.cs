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
            int spaceDimension, pointNO, serverNO, upperBoundOfPostVals, initialHistRes;
            double[] minElems, maxElems, posteriorValues, centralDiffs;
            double posteriorSum;
            Console.WriteLine("How would you like to execute histogram building? " + 
                "Would you like to apply stream mode?");
            Console.WriteLine("(Please choose this mode only if you have prior information about" +
                "object number and bounding box)");
            bool streamMode = bool.Parse(Console.ReadLine());
            if (streamMode)
            {
                handleStreamMode(inputParser, histogramComputer, knuthPosteriorComputer, out filename, out spaceDimension,
                    out pointNO, out serverNO, out upperBoundOfPostVals, out initialHistRes, out minElems, out maxElems, 
                    out posteriorValues, out centralDiffs, out posteriorSum);
            }
            else
            {
                handleNormalMode(inputParser, histogramComputer, knuthPosteriorComputer, out filename, out spaceDimension,
                    out pointNO, out serverNO, out upperBoundOfPostVals, out initialHistRes, out minElems, out maxElems,
                    out posteriorValues, out centralDiffs, out posteriorSum);
            }
            writeOutStats(lMethod, furthestPointFromLineMethod, pointNO, upperBoundOfPostVals, initialHistRes, 
                posteriorValues, centralDiffs, posteriorSum);
            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }

        private static void handleStreamMode(InputParser inputParser, HistogramComputer histogramComputer, 
            KnuthPosteriorComputer knuthPosteriorComputer, out string filename, out int spaceDimension, 
            out int pointNO, out int serverNO, out int upperBoundOfPostVals, out int initialHistRes, 
            out double[] minElems, out double[] maxElems, out double[] posteriorValues, out double[] centralDiffs, 
            out double posteriorSum)
        {
            inputParser.parseInputFileStreamMode(out filename, out spaceDimension, out pointNO,
                out minElems, out maxElems);
            initializeVariables(inputParser, pointNO, out serverNO, out upperBoundOfPostVals, out posteriorValues,
                out centralDiffs, out initialHistRes);
            posteriorSum = buildHistogramsStreamMode(inputParser, histogramComputer, knuthPosteriorComputer,
                filename, spaceDimension, pointNO, minElems, maxElems, serverNO, upperBoundOfPostVals,
                posteriorValues, centralDiffs, initialHistRes);
        }

        private static void handleNormalMode(InputParser inputParser, HistogramComputer histogramComputer, 
            KnuthPosteriorComputer knuthPosteriorComputer, out string filename, out int spaceDimension, 
            out int pointNO, out int serverNO, out int upperBoundOfPostVals, out int initialHistRes, 
            out double[] minElems, out double[] maxElems, out double[] posteriorValues, out double[] centralDiffs, 
            out double posteriorSum)
        {
            List<DataRow> data = inputParser.parseInputFile(out filename, out spaceDimension, out pointNO,
            out minElems, out maxElems);
            initializeVariables(inputParser, pointNO, out serverNO, out upperBoundOfPostVals, out posteriorValues,
                out centralDiffs, out initialHistRes);
            posteriorSum = buildHistograms(inputParser, histogramComputer, knuthPosteriorComputer, filename,
                spaceDimension, pointNO, minElems, maxElems, serverNO, upperBoundOfPostVals,
                posteriorValues, centralDiffs, initialHistRes, data);
        }

        private static void initializeVariables(InputParser inputParser, int pointNO, out int serverNO,
            out int upperBoundOfPostVals, out double[] posteriorValues, out double[] centralDiffs,
            out int initialHistRes)
        {
            serverNO = inputParser.parseServerNO();
            upperBoundOfPostVals = (int)(Math.Sqrt(pointNO));
            //upperBoundOfPostVals = 150;
            Console.WriteLine(upperBoundOfPostVals);
            posteriorValues = new double[upperBoundOfPostVals - 1];
            // The central difference approximations of second derivative of posterior:
            centralDiffs = new double[upperBoundOfPostVals - 1];
            initialHistRes = 2;
        }

        private static double buildHistogramsStreamMode(InputParser inputParser, HistogramComputer histogramComputer,
            KnuthPosteriorComputer knuthPosteriorComputer, string filename, int spaceDimension, int pointNO,
            double[] minElems, double[] maxElems, int serverNO, int upperBoundOfPostVals,
            double[] posteriorValues, double[] centralDiffs, int initialHistRes)
        {
            double posteriorSum = 0.0;
            for (int histogramResolution = initialHistRes; histogramResolution <= upperBoundOfPostVals;
                histogramResolution++)
            {
                int[] binHefts = new int[(int)Math.Pow(histogramResolution, spaceDimension)];
                var lines = File.ReadLines(filename);
                int lineIdx = 0;
                foreach (var line in lines)
                {
                    string[] lineParts = line.Split(InputParser.delimiter);
                    if (lineParts.Length != spaceDimension)
                        throw new ArgumentException("The line " + lineIdx + " has invalid dimension!");
                    double[] coords = new double[spaceDimension];
                    for (int coordIdx = 0; coordIdx < lineParts.Length; coordIdx++)
                    {
                        coords[coordIdx] = Double.Parse(lineParts[coordIdx], NumberStyles.Float, 
                            CultureInfo.InvariantCulture);
                    }
                    histogramComputer.buildHistogramForDataRowTuple(histogramResolution, spaceDimension, minElems, 
                        maxElems, binHefts, coords);
                    lineIdx++;
                }
                posteriorSum = innerBuildHistograms(inputParser, knuthPosteriorComputer, filename, spaceDimension, 
                    pointNO, serverNO, upperBoundOfPostVals, posteriorValues, centralDiffs, initialHistRes, posteriorSum,
                    histogramResolution, binHefts);
            }
            return posteriorSum;
        }

        private static double buildHistograms(InputParser inputParser, HistogramComputer histogramComputer, 
            KnuthPosteriorComputer knuthPosteriorComputer, string filename, int spaceDimension, int pointNO, 
            double[] minElems, double[] maxElems, int serverNO, int upperBoundOfPostVals, 
            double[] posteriorValues, double[] centralDiffs, int initialHistRes, List<DataRow> data)
        {
            double posteriorSum = 0.0;
            for (int histogramResolution = initialHistRes; histogramResolution <= upperBoundOfPostVals; 
                histogramResolution++)
            {
                int[] binHefts = histogramComputer.buildHistogram(histogramResolution, data, spaceDimension,
                    minElems, maxElems);
                posteriorSum = innerBuildHistograms(inputParser, knuthPosteriorComputer, filename, spaceDimension,
                    pointNO, serverNO, upperBoundOfPostVals, posteriorValues, centralDiffs, initialHistRes, posteriorSum,
                    histogramResolution, binHefts);
            }
            return posteriorSum;
        }

        private static double innerBuildHistograms(InputParser inputParser, KnuthPosteriorComputer knuthPosteriorComputer,
            string filename, int spaceDimension, int pointNO, int serverNO, int upperBoundOfPostVals,
            double[] posteriorValues, double[] centralDiffs, int initialHistRes, double posteriorSum,
            int histogramResolution, int[] binHefts)
        {
            writeOutInputSizes(inputParser, filename, spaceDimension, histogramResolution, serverNO, binHefts);
            double posteriorValue = knuthPosteriorComputer.evaluate(Math.Pow(histogramResolution, spaceDimension),
                pointNO, binHefts);
            int idx = histogramResolution - initialHistRes;
            posteriorValues[idx] = posteriorValue;
            determineCentralDiff(upperBoundOfPostVals, posteriorValues, centralDiffs, initialHistRes,
                histogramResolution, idx);
            Console.WriteLine("Histogram resolution: {0}, posterior value: {1}, central diff: {2}",
                histogramResolution, posteriorValue, centralDiffs[idx]);
            posteriorSum += posteriorValue;
            return posteriorSum;
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
            string hstgramOutput = @"u:\temp\data\hstgram_for_input_res_" + histogramResolution + ".dat";
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
            string inputSizesOutput = @"u:\temp\data\" + filename + "_input_sizes_serv_" + serverNO 
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
            string posteriorValuesOutput = @"u:\temp\data\posterior_values.dat";
            System.IO.File.WriteAllText(posteriorValuesOutput, strBldr.ToString());
        }

        private static void writeOutStats(LMethod lMethod, FurthestPointFromLineMethod furthestPointFromLineMethod, int pointNO, int upperBoundOfPostVals, int initialHistRes, double[] posteriorValues, double[] centralDiffs, double posteriorSum)
        {
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
        }
    }
}
