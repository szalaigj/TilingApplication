using CellsToServersApp.ArrayPartition;
using System;

namespace CellsToServersApp
{
    public class InputParser
    {
        private IndexTransformator transformator;

        public InputParser(IndexTransformator transformator)
        {
            this.transformator = transformator;
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

        public int parseInputArray(int histogramResolution, Array array)
        {
            int pointNO = 0;
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
            string[] cells = line.Split(' ');
            if (cells.Length == cellNO)
            {
                int[] indicesArray = new int[array.Rank];
                for (int cellIdx = 0; cellIdx < cells.Length; cellIdx++)
                {
                    transformator.transformCellIdxToIndicesArray(histogramResolution, indicesArray, cellIdx);
                    int cellValue = int.Parse(cells[cellIdx]);
                    pointNO += cellValue;
                    array.SetValue(cellValue, indicesArray);
                }
            }
            else
            {
                throw new ArgumentException("The cell number does not equal to the number of typed cells.");
            }
            return pointNO;
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
