using CellsToServersApp.ArrayPartition;
using CellsToServersApp.LPProblem;
using System;
using System.IO;

namespace CellsToServersApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IndexTransformator transformator = new IndexTransformator();
            InputParser inputParser = new InputParser(transformator);
            HeftArrayCreator heftArrayCreator = new HeftArrayCreator(transformator);
            int serverNO;
            int pointNO;
            double delta;
            int neededTileNumber;
            int[] tiles;
            arrayPartitionPhase(transformator, inputParser, heftArrayCreator, 
                out serverNO, out pointNO, out delta, out neededTileNumber, out tiles);
            LPModelFileCreator lpModelFileCreator = new LPModelFileCreator();
            LPSolver lpSolver = new LPSolver();
            lpProblemPhase(inputParser, serverNO, pointNO, delta, neededTileNumber, tiles, 
                lpModelFileCreator, lpSolver);
            Console.WriteLine("Press any key to exit!");
            Console.Read();
        }

        private static void arrayPartitionPhase(IndexTransformator transformator, InputParser inputParser, HeftArrayCreator heftArrayCreator, out int serverNO, out int pointNO, out double delta, out int neededTileNumber, out int[] tiles)
        {
            int spaceDimension;
            int histogramResolution;
            inputParser.parseInputSizes(out spaceDimension, out histogramResolution, out serverNO);
            Console.WriteLine("Space dim: {0}, resolution: {1}, server no.: {2}",
                spaceDimension, histogramResolution, serverNO);
            int[] lengthsArray = new int[spaceDimension];
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                lengthsArray[idx] = histogramResolution;
            }
            Array array = Array.CreateInstance(typeof(int), lengthsArray);
            pointNO = inputParser.parseInputArray(histogramResolution, array);
            Console.WriteLine("Point no.: {0}", pointNO);
            delta = (double)pointNO / (double)serverNO;
            Console.WriteLine("Delta: {0}", delta);
            Array heftArray = heftArrayCreator.createHeftArray(spaceDimension, histogramResolution, array);
            
            Divider divider = new Divider(array, heftArray, transformator,
                spaceDimension, histogramResolution, serverNO, delta);
            Coords[] partition;
            neededTileNumber = divider.determineNeededTileNumber(out partition);
            Console.WriteLine("Needed tile number: {0}", neededTileNumber);
            tiles = new int[neededTileNumber];
            for (int idx = 0; idx < neededTileNumber; idx++)
            {
                tiles[idx] = partition[idx].HeftOfRegion;
                partition[idx].printCoords(spaceDimension, idx + 1);
            }
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
