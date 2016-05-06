using RecursiveBisectionApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecursiveBisectionApp
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
            int spaceDimension;
            int histogramResolution;
            Array array;
            bool together = inputParser.determineTogetherOrSeparately();
            if (together)
            {
                array = inputParser.parseInputFile(out spaceDimension, out histogramResolution,
                    out serverNO, out pointNO);
            }
            else
            {
                parseInputSeparately(inputParser, out serverNO, out pointNO, out spaceDimension,
                    out histogramResolution, out array);
            }
            Console.WriteLine("Point no.: {0}", pointNO);
            Array heftArray = heftArrayCreator.createHeftArray(spaceDimension, histogramResolution, array);
            BinaryDecomposer binaryDecomposer = new BinaryDecomposer(array, heftArray, transformator, spaceDimension, 
                histogramResolution, serverNO, pointNO);
            Coords[] partition = binaryDecomposer.decompose();
            writeOutTiles(serverNO, spaceDimension, partition);
            writeOutServers(serverNO, partition);
            Console.WriteLine("Press any key to exit!");
            Console.Read();
        }

        private static void parseInputSeparately(InputParser inputParser, out int serverNO, out int pointNO, 
            out int spaceDimension, out int histogramResolution, out Array array)
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
            inputParser.parseInputArray(serverNO, histogramResolution, array, out pointNO);
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
