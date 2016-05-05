using System;
using System.Collections.Generic;
using System.IO;

namespace RecursiveBisectionApp.Utils
{
    public class InputParser
    {
        private Transformator transformator;

        public InputParser(Transformator transformator)
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
            out int pointNO)
        {
            Array array;
            Console.WriteLine("Enter the input path and filename:");
            string filename = Console.ReadLine();
            bool exists = File.Exists(filename);
            if (exists)
            {
                string[] lines = File.ReadAllLines(filename);
                spaceDimension = int.Parse(lines[0]);
                histogramResolution = int.Parse(lines[1]);
                serverNO = int.Parse(lines[2]);
                Console.WriteLine("Space dim: {0}, resolution: {1}, server no.: {2}", spaceDimension,
                    histogramResolution, serverNO);
                int[] lengthsArray = new int[spaceDimension];
                for (int idx = 0; idx < spaceDimension; idx++)
                {
                    lengthsArray[idx] = histogramResolution;
                }
                array = Array.CreateInstance(typeof(int), lengthsArray);
                int cellNO = (int)Math.Pow(histogramResolution, array.Rank);
                innerParseInputArray(serverNO, histogramResolution, array, cellNO, lines[3], out pointNO);
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

        public void parseInputSizes(out int spaceDimension, out int histogramResolution, out int serverNO)
        {
            Console.WriteLine("Enter space (array) dimension:");
            spaceDimension = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter histogram resolution:");
            histogramResolution = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter server number:");
            serverNO = int.Parse(Console.ReadLine());
        }

        public void parseInputArray(int serverNO, int histogramResolution, Array array,
            out int pointNO)
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
            innerParseInputArray(serverNO, histogramResolution, array, cellNO, line, out pointNO);
        }

        private void innerParseInputArray(int serverNO, int histogramResolution, Array array, int cellNO, string line,
            out int pointNO)
        {
            string[] cells = line.Split(' ');
            pointNO = 0;
            int cellMaxValue = 0;
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
        }
    }
}
