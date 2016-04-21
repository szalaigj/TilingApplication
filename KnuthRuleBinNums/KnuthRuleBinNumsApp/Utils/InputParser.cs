using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace KnuthRuleBinNumsApp.Utils
{
    public class InputParser
    {
        private char delimiter = ' ';

        public List<DataRow> parseInputFile(out string filename, out int spaceDimension, out int pointNO, 
            out double[] minElems, out double[] maxElems)
        {
            List<DataRow> data = new List<DataRow>();
            Console.WriteLine("Enter input path and filename:");
            filename = Console.ReadLine();
            bool exists = File.Exists(filename);
            if (exists)
            {
                Console.WriteLine("Enter the space dimension:");
                spaceDimension = int.Parse(Console.ReadLine());
                minElems = Enumerable.Repeat(double.MaxValue, spaceDimension).ToArray();
                maxElems = Enumerable.Repeat(double.MinValue, spaceDimension).ToArray();
                var lines = File.ReadLines(filename);
                int lineIdx = 0;
                foreach (var line in lines)
                {
                    //double[] coords = Array.ConvertAll(line.Split(delimiter), Double.Parse);
                    string[] lineParts = line.Split(delimiter);
                    if (lineParts.Length != spaceDimension)
                        throw new ArgumentException("The line " + lineIdx + " has invalid dimension!");
                    double[] coords = new double[spaceDimension];
                    handleNewLine(lineParts, coords, minElems, maxElems);
                    DataRow dataRow = new DataRow();
                    dataRow.Tuple = coords;
                    data.Add(dataRow);
                    lineIdx++;
                }
                pointNO = lineIdx;
            }
            else
                throw new ArgumentException("The path is invalid.");
            return data;
        }

        private static void handleNewLine(string[] lineParts, double[] coords, double[] minElems, double[] maxElems)
        {
            for (int coordIdx = 0; coordIdx < lineParts.Length; coordIdx++)
            {
                coords[coordIdx] = Double.Parse(lineParts[coordIdx], NumberStyles.Float, CultureInfo.InvariantCulture);
                if (coords[coordIdx] < minElems[coordIdx])
                {
                    minElems[coordIdx] = coords[coordIdx];
                }
                if (coords[coordIdx] > maxElems[coordIdx])
                {
                    maxElems[coordIdx] = coords[coordIdx];
                }
            }
        }

        public int parseServerNO()
        {
            Console.WriteLine("Enter server number:");
            int serverNO = int.Parse(Console.ReadLine());
            return serverNO;
        }
    }
}
