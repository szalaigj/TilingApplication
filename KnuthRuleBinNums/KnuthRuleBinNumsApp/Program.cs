using System;
using System.Collections.Generic;
using KnuthRuleBinNumsApp.Utils;
using System.Text;
using System.Globalization;
using System.IO;

namespace KnuthRuleBinNumsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            InputParser inputParser = new InputParser();
            HistogramComputer histogramComputer = new HistogramComputer();
            KnuthPosteriorComputer knuthPosteriorComputer = new KnuthPosteriorComputer();
            string filename;
            int spaceDimension, pointNO;
            double[] minElems, maxElems;
            List<DataRow> data = inputParser.parseInputFile(out filename, out spaceDimension, out pointNO, 
                out minElems, out maxElems);
            int serverNO = inputParser.parseServerNO();
            double posteriorSum = 0.0;
            double[] posteriorValues = new double[(int)(Math.Sqrt(pointNO)) - 1];
            for (int histogramResolution = 2; histogramResolution <= Math.Sqrt(pointNO); histogramResolution++)
            {
                int[] binHefts = histogramComputer.buildHistogram(histogramResolution, data, spaceDimension, 
                    minElems, maxElems);
                //writeOutBinHefts(binHefts, histogramResolution);
                writeOutInputSizes(inputParser, filename, spaceDimension, histogramResolution, serverNO, binHefts);
                double posteriorValue = knuthPosteriorComputer.evaluate(Math.Pow(histogramResolution, spaceDimension), 
                    pointNO, binHefts);
                posteriorValues[histogramResolution - 2] = posteriorValue;
                Console.WriteLine("Histogram resolution: {0}, posterior value: {1}", 
                    histogramResolution, posteriorValue);
                posteriorSum += posteriorValue;
            }
            writeOutPosteriorValues(posteriorValues);
            Console.WriteLine("Posterior mean: {0}", (posteriorSum) / (Math.Floor(Math.Sqrt(pointNO)) - 1));
            Console.WriteLine("Press any key to continue...");
            Console.Read();
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
