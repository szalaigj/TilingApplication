﻿using OldCellsToServersApp.ArrayPartition;
using System;
using System.Globalization;
using System.IO;

namespace OldCellsToServersApp
{
    public class InputParser
    {
        private IndexTransformator transformator;

        public InputParser(IndexTransformator transformator)
        {
            this.transformator = transformator;
        }

        public bool determineTogetherOrSeparately()
        {
            bool together;
            Console.WriteLine("Would you like to provide a path for the input file which "
                + "contains sizes and histogram data (true or false)?");
            while (!bool.TryParse(Console.ReadLine(), out together))
            {
                Console.WriteLine("Enter true or false:");
            }
            return together;
        }

        public Array parseInputFile(out int spaceDimension, out int histogramResolution, out int serverNO,
            out int pointNO, out double delta, out int strategyCode, out int cellMaxValue, out double deltaCoefficient,
            out int slidingWindowSize)
        {
            Array array;
            Console.WriteLine("Enter the input path and filename:");
            string filename = Console.ReadLine();
            bool exists = File.Exists(filename);
            if(exists)
            {
                string[] lines = File.ReadAllLines(filename);
                spaceDimension = int.Parse(lines[0]);
                histogramResolution = int.Parse(lines[1]);
                serverNO = int.Parse(lines[2]);
                strategyCode = int.Parse(lines[3]);
                string strategyText = determineStrategyText(strategyCode);
                deltaCoefficient = double.Parse(lines[4], CultureInfo.InvariantCulture);
                slidingWindowSize = int.Parse(lines[5]);
                Console.WriteLine("Space dim: {0}, resolution: {1}, server no.: {2}, chosen strategy: {3}, " +
                    "delta coefficient: {4}, sliding window size: {5}", spaceDimension, histogramResolution, serverNO, 
                    strategyText, deltaCoefficient, slidingWindowSize);
                int[] lengthsArray = new int[spaceDimension];
                for (int idx = 0; idx < spaceDimension; idx++)
                {
                    lengthsArray[idx] = histogramResolution;
                }
                array = Array.CreateInstance(typeof(int), lengthsArray);
                int cellNO = (int)Math.Pow(histogramResolution, array.Rank);
                innerParseInputArray(serverNO, histogramResolution, array, cellNO, lines[6], out pointNO, out delta,
                    out cellMaxValue);
            }
            else
            {
                throw new ArgumentException("The path is not valid.");
            }
            return array;
        }

        private string determineStrategyText(int strategyCode)
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

        public void parseInputSizes(out int spaceDimension, out int histogramResolution, out int serverNO,
            out int strategyCode, out double deltaCoefficient, out int slidingWindowSize)
        {
            Console.WriteLine("Enter space (array) dimension:");
            spaceDimension = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter histogram resolution:");
            histogramResolution = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter server number:");
            serverNO = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter strategy code:");
            Console.WriteLine("(0 : Optimized for clustering; 1 : Optimized for load balancing)");
            strategyCode = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter delta coefficient:");
            deltaCoefficient = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
            Console.WriteLine("Enter sliding window size:");
            slidingWindowSize = int.Parse(Console.ReadLine());
        }

        public void parseInputArray(int serverNO, int histogramResolution, Array array,
            out int pointNO, out double delta, out int cellMaxValue)
        {
            int cellNO = (int)Math.Pow(histogramResolution, array.Rank);
            Console.WriteLine("Enter values for array (splitted by character ' '):");
            Console.WriteLine("Example: (space dimension : 3, histogram resolution : 2)");
            Console.WriteLine("Level 0:");
            Console.WriteLine("1 1");
            Console.WriteLine("2 3");
            Console.WriteLine("Level 1:");
            Console.WriteLine("2 3");
            Console.WriteLine("4 1");
            Console.WriteLine("The correct input for this example: 1 1 2 3 2 3 4 1");
            // The array elements for this example will be
            //  array[0, 0, 0]==1
            //  array[0, 0, 1]==1
            //  array[0, 1, 0]==2
            //  array[0, 1, 1]==3
            //  array[1, 0, 0]==2
            //  array[1, 0, 1]==3
            //  array[1, 1, 0]==4
            //  array[1, 1, 1]==1
            // So the highest dimension-related index is the lowest index of the array.
            string line = Console.ReadLine();
            innerParseInputArray(serverNO, histogramResolution, array, cellNO, line, out pointNO, out delta, 
                out cellMaxValue);
        }

        private void innerParseInputArray(int serverNO, int histogramResolution, Array array, int cellNO, string line,
            out int pointNO, out double delta, out int cellMaxValue)
        {
            string[] cells = line.Split(' ');
            pointNO = 0;
            cellMaxValue = 0;
            if (cells.Length == cellNO)
            {
                int[] indicesArray = new int[array.Rank];
                for (int cellIdx = 0; cellIdx < cells.Length; cellIdx++)
                {
                    transformator.transformCellIdxToIndicesArray(histogramResolution, indicesArray, cellIdx);
                    int cellValue = int.Parse(cells[cellIdx]);
                    if (cellMaxValue < cellValue)
                    {
                        cellMaxValue = cellValue;
                    }
                    pointNO += cellValue;
                    array.SetValue(cellValue, indicesArray);
                }
            }
            else
            {
                throw new ArgumentException("The cell number does not equal to the number of typed cells.");
            }
            delta = (double)pointNO / (double)serverNO;
            if (cellMaxValue > delta)
            {
                throw new ArgumentException("There is a cell which has greater heft value than delta. " +
                    "You should try to increase histogram resolution for the original data.");
            }
        }

        public int parseInputTimeout()
        {
            int timeoutSec;
            Console.WriteLine("Enter timeout (sec), if you enter 0 then no timeout will occur:");
            while (!int.TryParse(Console.ReadLine(), out timeoutSec))
            {
                Console.WriteLine("Enter correct timeout (sec):");
            }
            return timeoutSec;
        }

        public bool parseInputDeleteOutputLP()
        {
            bool deleteOutputLP;
            Console.WriteLine("Would you like to delete output lp (model) file after execution (true or false)?");
            while (!bool.TryParse(Console.ReadLine(), out deleteOutputLP))
            {
                Console.WriteLine("Enter true or false:");
            }
            return deleteOutputLP;
        }
    }
}
