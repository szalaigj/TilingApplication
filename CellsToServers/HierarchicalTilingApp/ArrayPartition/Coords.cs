using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchicalTilingApp.ArrayPartition
{
    public class Coords
    {
        public int[] ExtendedIndicesArray { get; set; }
        public int HeftOfRegion { get; set; }

        public double differenceFromDelta(double delta)
        {
            return Math.Abs(delta - HeftOfRegion);
        }

        public void printCoords(int spaceDimension, int serialNO)
        {
            Console.Write("{0}. tile: [", serialNO);
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                Console.Write(" {0} {1} ", ExtendedIndicesArray[2 * idx + 1], ExtendedIndicesArray[2 * idx + 2]);
            }
            Console.WriteLine("] : {0} heft", HeftOfRegion);
        }

        public void writeToStringBuilder(int spaceDimension, StringBuilder strBldr)
        {
            strBldr.Append(HeftOfRegion);
            for (int idx = 0; idx < spaceDimension; idx++)
            {
                strBldr.Append(" ").Append(ExtendedIndicesArray[2 * idx + 1]).Append(" ").Append(ExtendedIndicesArray[2 * idx + 2]);
            }
            strBldr.AppendLine();
        }
    }
}
