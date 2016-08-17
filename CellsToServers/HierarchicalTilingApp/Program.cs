using HierarchicalTilingApp.ArrayPartition;
using HierarchicalTilingApp.Measure;
using HierarchicalTilingApp.SumOfSquares;
using HierarchicalTilingApp.Transformation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchicalTilingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            IntTupleEqualityComparer comparer = new IntTupleEqualityComparer();
            CornacchiaMethod cornacchiaMethod = new CornacchiaMethod(comparer);
            BacktrackingMethod backtrackingMethod = new BacktrackingMethod(cornacchiaMethod);
            ShellBuilder shellBuilder = new ShellBuilder(backtrackingMethod);
            Transformator transformator = new Transformator(shellBuilder);
            InputParser inputParser = new InputParser(transformator);
            HeftArrayCreator heftArrayCreator = new HeftArrayCreator(transformator);
            //int kNN = 274;
            double kNNMeasCoeff = 1.0;//0.1;
            double lbMeasCoeff = 0.0;//0.9;

            int serverNO;
            int pointNO;
            double delta;
            int spaceDimension;
            int histogramResolution;
            Array array;
            bool together = inputParser.determineTogetherOrSeparately();
            if (together)
            {
                array = inputParser.parseInputFile(out spaceDimension, out histogramResolution,
                    out serverNO, out pointNO, out delta);
            }
            else
            {
                parseInputSeparately(inputParser, out serverNO, out pointNO, out delta, out spaceDimension,
                    out histogramResolution, out array);
            }
            int kNN = (int)Math.Ceiling(delta);
            int maxShellNO = transformator.determineMaxRange(spaceDimension, histogramResolution);
            Shell[] shellsForKNN = shellBuilder.createShells(maxShellNO, spaceDimension);
            int maxRange = transformator.determineMaxRange(spaceDimension, histogramResolution / 2);
            Shell[] shellsForRange = shellBuilder.createShells(maxRange, spaceDimension);
            Console.WriteLine("Point no.: {0}", pointNO);
            Console.WriteLine("Delta: {0}", delta);
            Console.WriteLine("kNN measurement coefficient: {0}", kNNMeasCoeff);
            Console.WriteLine("Load balancing measurement coefficient: {0}", lbMeasCoeff);
            
            Array heftArray = heftArrayCreator.createHeftArray(spaceDimension, histogramResolution, array);

            IterativeDivider divider = new IterativeDivider(array, heftArray, transformator, spaceDimension,
                histogramResolution, serverNO, delta, pointNO, kNN, maxRange, shellsForKNN, shellsForRange, 
                kNNMeasCoeff, lbMeasCoeff);
            Coords[] partition;
            double objectiveValue = divider.determineObjectiveValue(out partition);
            Console.WriteLine("Objective value: {0}", objectiveValue);
            Console.WriteLine("Sum of differences between tile hefts and delta: {0}", divider.getDiffSum());
            writeOutTiles(serverNO, spaceDimension, partition);
            writeOutServers(serverNO, partition);
            writeOutCellsToServers(histogramResolution, serverNO, partition);

            stopwatch.Stop();
            // Write hours, minutes and seconds.
            Console.WriteLine("Elapsed time: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

            Console.WriteLine("Press any key to exit!");
            Console.Read();
        }

        private static void parseInputSeparately(InputParser inputParser, out int serverNO, out int pointNO,
            out double delta, out int spaceDimension, out int histogramResolution, out Array array)
        {
            inputParser.parseInputSizes(out spaceDimension, out histogramResolution, out serverNO);
            Console.WriteLine("Space dim: {0}, resolution: {1}, server no.: {2}", spaceDimension, histogramResolution, 
                serverNO);
            int[] lengthsArray = new int[spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                lengthsArray[idx] = histogramResolution;
            }
            array = Array.CreateInstance(typeof(int), lengthsArray);
            inputParser.parseInputArray(serverNO, histogramResolution, array, out pointNO, out delta);
        }

        private static void writeOutTiles(int serverNO, int spaceDimension, Coords[] partition)
        {
            StringBuilder strBldr = new StringBuilder();
            for (int idx = 0; idx < serverNO; idx++)
            {
                partition[idx].printCoords(spaceDimension, idx + 1);
                partition[idx].writeToStringBuilder(spaceDimension, strBldr);
            }
            string tilesOutput = @"c:\temp\data\hier_tiling\tiles.dat";
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
            string serversOutput = @"c:\temp\data\hier_tiling\servers.dat";
            System.IO.File.WriteAllText(serversOutput, strBldr.ToString());
        }

        private static void writeOutCellsToServers(int histogramResolution, int serverNO, Coords[] partition)
        {
            StringBuilder strBldr = new StringBuilder();
            for (int serverIdx = 0; serverIdx < serverNO; serverIdx++)
            {
                int[] extendedIndicesArray = partition[serverIdx].ExtendedIndicesArray;
                for (int x = extendedIndicesArray[1]; x <= extendedIndicesArray[2]; x++)
                {
                    for (int y = extendedIndicesArray[3]; y <= extendedIndicesArray[4]; y++)
                    {
                        int cellIdx = x * histogramResolution + y;
                        strBldr.Append(cellIdx);
                        if ((x != extendedIndicesArray[2]) || (y != extendedIndicesArray[4]))
                        {
                            strBldr.Append(" ");
                        }
                    }
                }
                strBldr.AppendLine();
            }
            string serversOutput = @"c:\temp\data\hier_tiling\cells_to_servers.dat";
            System.IO.File.WriteAllText(serversOutput, strBldr.ToString());
        }
    }
}
