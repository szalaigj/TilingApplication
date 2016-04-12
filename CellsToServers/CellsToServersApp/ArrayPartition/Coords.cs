using System;
using System.Text;

namespace CellsToServersApp.ArrayPartition
{
    public class Coords
    {
        public int[] IndicesArray { get; set; }
        public int HeftOfRegion { get; set; }
        public int MaxCellValue { get; set; }
        public int[] MaxCellIndices { get; set; }
        public int[] CenterOfMassIndices { get; set; }
        public int CrossBorderHeft { get; set; }

        public double differenceFromDelta(double delta)
        {
            return delta - HeftOfRegion;
        }

        public void printCoords(int spaceDimension, int serialNO)
        {
            Console.Write("{0}. tile: [", serialNO);
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                Console.Write(" {0} {1} ", IndicesArray[2 * idx], IndicesArray[2 * idx + 1]);
            }
            Console.Write("] : {0} heft; max cell value: {1}, max cell indices:", HeftOfRegion, MaxCellValue);
            foreach (var idx in MaxCellIndices)
            {
                Console.Write(" " + idx);
            }
            Console.Write(", center of mass indices:");
            foreach (var idx in CenterOfMassIndices)
            {
                Console.Write(" " + idx);
            }
            Console.WriteLine(", cross border heft: {0}", CrossBorderHeft);
        }

        public void writeToStringBuilder(int spaceDimension, StringBuilder strBldr)
        {
            strBldr.Append(HeftOfRegion);
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                strBldr.Append(" ").Append(IndicesArray[2 * idx]).Append(" ").Append(IndicesArray[2 * idx + 1]);
            }
            strBldr.AppendLine();
        }
    }
}
