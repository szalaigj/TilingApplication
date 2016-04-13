using HierarchicalTilingApp.ArrayPartition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchicalTilingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Transformator transformator = new Transformator();
            InputParser inputParser = new InputParser(transformator);
            HeftArrayCreator heftArrayCreator = new HeftArrayCreator(transformator);
            int serverNO;
            int pointNO;
            double delta;
            int spaceDimension;
            int histogramResolution;
            int strategyCode;
            int slidingWindowSize;
            Array array;
            bool together = inputParser.determineTogetherOrSeparately();
            if (together)
            {
                array = inputParser.parseInputFile(out spaceDimension, out histogramResolution,
                    out serverNO, out pointNO, out delta, out strategyCode, out slidingWindowSize);
            }
            else
            {
                parseInputSeparately(inputParser, out serverNO, out pointNO, out delta, out spaceDimension,
                    out histogramResolution, out strategyCode, out array, out slidingWindowSize);
            }
            Console.WriteLine("Point no.: {0}", pointNO);
            Console.WriteLine("Delta: {0}", delta);
            Array heftArray = heftArrayCreator.createHeftArray(spaceDimension, histogramResolution, array);

            Divider divider = new Divider(heftArray, transformator, spaceDimension, histogramResolution, serverNO, delta,
                strategyCode, slidingWindowSize);
            Coords[] partition;
            int neededBound = divider.determineNeededBound(out partition);
            Console.WriteLine("Needed bound: {0}", neededBound);
            Console.WriteLine("Sum of differences between tile hefts and delta: {0}", divider.getDiffSum());
            writeOutTiles(serverNO, spaceDimension, partition);
            writeOutServers(serverNO, partition);
            Console.WriteLine("Press any key to exit!");
            Console.Read();
        }

        private static void parseInputSeparately(InputParser inputParser, out int serverNO, out int pointNO,
            out double delta, out int spaceDimension, out int histogramResolution, out int strategyCode,
            out Array array, out int slidingWindowSize)
        {
            inputParser.parseInputSizes(out spaceDimension, out histogramResolution, out serverNO, out strategyCode,
                out slidingWindowSize);
            string strategyText = determineStrategyText(strategyCode);
            Console.WriteLine("Space dim: {0}, resolution: {1}, server no.: {2}, chosen strategy: {3}, " +
                    "sliding window size: {4}", spaceDimension, histogramResolution, serverNO, strategyText, 
                    slidingWindowSize);
            int[] lengthsArray = new int[spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                lengthsArray[idx] = histogramResolution;
            }
            array = Array.CreateInstance(typeof(int), lengthsArray);
            inputParser.parseInputArray(serverNO, histogramResolution, array, out pointNO, out delta);
        }

        private static string determineStrategyText(int strategyCode)
        {
            string strategyText;
            if (strategyCode == 0)
            {
                strategyText = "Optimized for clustering";
            }
            else
            {
                strategyText = "Optimized for load balancing";
            }
            return strategyText;
        }

        private static void writeOutTiles(int serverNO, int spaceDimension, Coords[] partition)
        {
            StringBuilder strBldr = new StringBuilder();
            for (int idx = 0; idx < serverNO; idx++)
            {
                partition[idx].printCoords(spaceDimension, idx + 1);
                partition[idx].writeToStringBuilder(spaceDimension, strBldr);
            }
            string tilesOutput = @"c:\temp\data\tiles.dat";
            System.IO.File.WriteAllText(tilesOutput, strBldr.ToString());
        }

        private static void writeOutServers(int serverNO, Coords[] partition)
        {
            StringBuilder strBldr = new StringBuilder();
            for (int tileIdx = 0; tileIdx < serverNO; tileIdx++)
            {
                strBldr.Append(partition[tileIdx].HeftOfRegion).Append(" " + tileIdx);
                strBldr.AppendLine();
            }
            string serversOutput = @"c:\temp\data\servers.dat";
            System.IO.File.WriteAllText(serversOutput, strBldr.ToString());
        }
    }
}
