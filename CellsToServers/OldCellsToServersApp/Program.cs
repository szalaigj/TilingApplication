using OldCellsToServersApp.ArrayPartition;
using OldCellsToServersApp.LPProblem;
using System;
using System.IO;
using System.Text;

namespace OldCellsToServersApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // IMPORTANT NOTE:
            //     please check the Debug or Release folder contains lpsolve55.dll and build on x86 platform.
            IndexTransformator transformator = new IndexTransformator();
            InputParser inputParser = new InputParser(transformator);
            HeftArrayCreator heftArrayCreator = new HeftArrayCreator(transformator);
            int serverNO;
            int pointNO;
            double delta;
            int neededTileNumber;
            int[] tiles;
            //try
            //{
                arrayPartitionPhase(transformator, inputParser, heftArrayCreator,
                out serverNO, out pointNO, out delta, out neededTileNumber, out tiles);
                LPModelFileCreator lpModelFileCreator = new LPModelFileCreator();
                LPSolver lpSolver = new LPSolver();
                lpProblemPhase(inputParser, serverNO, pointNO, delta, neededTileNumber, tiles,
                    lpModelFileCreator, lpSolver);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERROR: " + ex.Message);
            //}
            Console.WriteLine("Press any key to exit!");
            Console.Read();
        }

        private static void arrayPartitionPhase(IndexTransformator transformator, InputParser inputParser, 
            HeftArrayCreator heftArrayCreator, out int serverNO, out int pointNO, out double delta, 
            out int neededTileNumber, out int[] tiles)
        {
            int spaceDimension;
            int histogramResolution;
            int strategyCode;
            int cellMaxValue;
            double deltaCoefficient;
            int slidingWindowSize;
            Array array;
            bool together = inputParser.determineTogetherOrSeparately();
            if (together)
            {
                array = inputParser.parseInputFile(out spaceDimension, out histogramResolution,
                    out serverNO, out pointNO, out delta, out strategyCode, out cellMaxValue,
                    out deltaCoefficient, out slidingWindowSize);
            }
            else
            {
                parseInputSeparately(inputParser, out serverNO, out pointNO, out delta, out spaceDimension,
                    out histogramResolution, out strategyCode, out cellMaxValue, out array,
                    out deltaCoefficient, out slidingWindowSize);
            }
            Console.WriteLine("Point no.: {0}", pointNO);
            Console.WriteLine("Delta: {0}", delta);
            double usedDelta = delta * deltaCoefficient;
            Console.WriteLine("The used delta: {0}", usedDelta);
            Array heftArray = heftArrayCreator.createHeftArray(spaceDimension, histogramResolution, array);

            Divider divider = new Divider(array, heftArray, transformator, spaceDimension, histogramResolution,
                serverNO, delta, usedDelta, strategyCode, slidingWindowSize);
            Coords[] partition;
            neededTileNumber = divider.determineNeededTileNumber(out partition);
            Console.WriteLine("Needed tile number: {0}", neededTileNumber);
            tiles = writeOutTiles(neededTileNumber, spaceDimension, partition);
        }

        private static void parseInputSeparately(InputParser inputParser, out int serverNO, out int pointNO, 
            out double delta, out int spaceDimension, out int histogramResolution, out int strategyCode,
            out int cellMaxValue, out Array array, out double deltaCoefficient, out int slidingWindowSize)
        {
            inputParser.parseInputSizes(out spaceDimension, out histogramResolution, out serverNO, out strategyCode,
                out deltaCoefficient, out slidingWindowSize);
            string strategyText = determineStrategyText(strategyCode);
            Console.WriteLine("Space dim: {0}, resolution: {1}, server no.: {2}, chosen strategy: {3}, " +
                    "delta coefficient: {4}, sliding window size: {5}", spaceDimension, histogramResolution, serverNO,
                    strategyText, deltaCoefficient, slidingWindowSize);
            int[] lengthsArray = new int[spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                lengthsArray[idx] = histogramResolution;
            }
            array = Array.CreateInstance(typeof(int), lengthsArray);
            inputParser.parseInputArray(serverNO, histogramResolution, array, out pointNO, out delta, 
                out cellMaxValue);
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

        private static int[] writeOutTiles(int neededTileNumber, int spaceDimension, Coords[] partition)
        {
            int[] tiles;
            tiles = new int[neededTileNumber];
            StringBuilder strBldr = new StringBuilder();
            for (int idx = 0; idx < neededTileNumber; idx++)
            {
                tiles[idx] = partition[idx].HeftOfRegion;
                partition[idx].printCoords(spaceDimension, idx + 1);
                partition[idx].writeToStringBuilder(spaceDimension, strBldr);
            }
            string tilesOutput = @"c:\temp\data\tiles.dat";
            System.IO.File.WriteAllText(tilesOutput, strBldr.ToString());
            return tiles;
        }

        private static void lpProblemPhase(InputParser inputParser, int serverNO, int pointNO, double delta,
            int neededTileNumber, int[] tiles, LPModelFileCreator lpModelFileCreator, LPSolver lpSolver)
        {
            int timeoutSec = inputParser.parseInputTimeout();
            bool deleteOutputLP = inputParser.parseInputDeleteOutputLP();

            string outputFilename = lpModelFileCreator.createOutputLPFile(serverNO, neededTileNumber, pointNO,
                tiles, delta);

            lpSolver.solveLP(serverNO, neededTileNumber, tiles, timeoutSec, outputFilename);

            if (deleteOutputLP)
            {
                File.Delete(outputFilename);
            }
        }
    }
}
